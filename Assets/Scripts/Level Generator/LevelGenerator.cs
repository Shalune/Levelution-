using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	// ODOT - remove MonoBehaviour?
	// ODOT - make part of input or LevelData
	public int maxRooms = 35;
	public LevelData levelParams;
	public float bufferBranching = 0.5f;
	public int reAttemptPlacementThreshold = 20;		// number of times a new piece or exit will attempt to be re-placed to avoid collisions
	public float maxColorVariation = 0.3f;
    public float inverseHallBoostFromWide = 7f;
    public float shrinkLowRollForPiece = 0.75f;


    LevelPiece headNode;
	public float hallProbability = 0.5f;
	public Queue<Vector3> unexpandedExits;
	public List<LevelPiece> levelPieces;
    public List<GameObject> rooms;
	//public List<GameObject> roomTemplates;    //delete once replaced
	//public List<GameObject> hallTemplates;    //delete once replaced

	public void Init(){
		unexpandedExits = new Queue<Vector3> ();
		levelPieces = new List<LevelPiece> ();
        rooms = new List<GameObject>();
	}

    /*      //delete once replaced
	public void LoadTemplates(List<GameObject> rooms, List<GameObject> halls){
		roomTemplates = rooms;
		hallTemplates = halls;
		Debug.Log ("loaded : " + roomTemplates.Count + "  " + hallTemplates.Count);
	}
    */

	public void GenerateAndInstantiate(LevelData inputParams){
		GenerateLevel (inputParams);
		SetupLevel (headNode);
	}

	public LevelPiece GenerateLevel(LevelData inputParams){
		if (unexpandedExits == null || levelPieces == null) {
			Init ();
		}

		levelParams = inputParams;
        CreateStartingPiece();

		while (levelPieces.Count < maxRooms && unexpandedExits.Count != 0) {
            CreatePiece();
		}

		return headNode;
	}


	public void SetupLevel(LevelPiece head){
		Debug.Log ("Final pieces = " + levelPieces.Count);
		// level tree traversal not implemented, uses unsorted list of levelPieces instead
		foreach (LevelPiece piece in levelPieces) {
			Debug.Log ("Setting position to " + (Vector3)piece.pos);
			piece.transform.localPosition = (Vector3)piece.pos; //new Vector3 (piece.pos.x, piece.pos.y, 0);
			//piece.transform.position.Set(piece.pos.x, piece.pos.y, 0f);
			// ODOT - replace with actual level objects for practical use
			piece.transform.localScale = new Vector3 (piece.dimensions.x, piece.dimensions.y, 1);
			piece.GetComponent<Renderer> ().sharedMaterial.SetColor ("_Color", levelParams.baseColor);
		}
	}

	public void CreateStartingPiece(){
        //GameObject roomType = ChoosePiece ();               // to enum
        //headNode = roomType.GetComponent<LevelPiece> ();    // new constructor
        headNode = ChoosePiece();
		headNode.CreateFromEntrance ();
		GenerateExitsFor (headNode);
		ColorPiece (ref headNode);

		CreateRoom (headNode);
		Debug.Log ("Created first room " + headNode.pos + "    " + headNode.dimensions);
		levelPieces.Add (headNode);
	}

	public void CreatePiece(){
		Vector3 startingExit = unexpandedExits.Dequeue();
        //GameObject pieceType = ChoosePiece();                       // to enum
        //LevelPiece piece = pieceType.GetComponent<LevelPiece> ();   // new constructor
        LevelPiece piece = ChoosePiece();
		piece.CreateFromEntrance(startingExit);

		int attempts = 0;

		while (CollidesWithLevel ((Vector2)piece.pos, piece.dimensions)) {
            //pieceType = ChoosePiece();                          // to enum
            //piece = pieceType.GetComponent<LevelPiece> ();      // new constructor
            piece = ChoosePiece();
			piece.CreateFromEntrance(startingExit);

			attempts++;
			if (attempts > reAttemptPlacementThreshold) {
				break;
			}
		}

		GenerateExitsFor(piece);
		ColorPiece (ref piece);

        CreateRoom(piece);
		Debug.Log ("Created new room " + piece.pos + "    " + piece.dimensions);
		levelPieces.Add(piece);
	}

	public void GenerateExitsFor(LevelPiece piece){
		
		int targetExits = piece.minExits;

		while (targetExits < piece.maxExits) {
			float roll = Random.Range (0f, 1f);

			if (roll < levelParams.branchFactor + ((1f - levelParams.branchFactor) * bufferBranching)) {
				targetExits++;
			} else {
				break;
			}
		}

		int numExits = 1;		// accounts for existing entrance
		int attempts = 0;

		Debug.Log ("Target exits  " + targetExits);

		while (numExits < targetExits) {

			Vector3 newExit = piece.GenerateExit ();
			if (!CollidesWithLevel (newExit, piece)) {
				unexpandedExits.Enqueue (newExit);
				Debug.Log ("new exit = " + newExit);
				numExits++;
			} else {
				attempts++;
				if (attempts > reAttemptPlacementThreshold) {
					break;
				}
			}
		}
	}


	// translates exit vectors for use with main method
	// treats them as 1x1 rooms to check
	public bool CollidesWithLevel(Vector3 exit, LevelPiece piece){
		Vector2 adjustedExit = (Vector2)exit + piece.ExitCodeToDirection (exit.z) + piece.pos;
		return CollidesWithLevel(adjustedExit, Vector2.one);
	}

	public bool CollidesWithLevel(Vector2 center, Vector2 dimensions){
		//Debug.Log ("levelPieces count = " + levelPieces.Count);
		foreach (LevelPiece piece in levelPieces) {
			if (piece.CollidesWith (center, dimensions)) {
				return true;
			}
		}

		return false;
	}

	public LevelPiece ChoosePiece(){
		// ODOT change likelihood of room vs hall based on previous
		float roll = Random.Range (0f, 1f);

		if (roll < hallProbability + HallBoostFromWide()) {
			return ChoosePiece (true);
		} else {
			return ChoosePiece ();
		}
	}
		
	public LevelPiece ChoosePiece(bool chooseHall=false)
    {
        if (chooseHall)
            return ChooseHallPiece();
        else
            return ChooseRoomPiece();
	}

    public LevelPiece ChooseHallPiece()
    {
        int i = 0;
        int maxIterations = 10;

        int cycle = Random.Range(0, (int)LevelPiece._hallTypes.NUMELEMENTS);
        int startVal = cycle;

        while (true)
        {
            float roll = Random.Range(0f, 1f);
            if (roll < 0.5f)
            {
                roll -= (0.5f - roll) * shrinkLowRollForPiece;
            }

            //PieceWideFactor(pieceOptions[cycle].GetComponent<LevelPiece>())   // delete when replaced
            if (roll < LevelPiece.GetWideFactor((LevelPiece._hallTypes)cycle) * levelParams.wideFactor)
            {   // change to enum method	
                return LevelPiece.NewPiece((LevelPiece._hallTypes)cycle);
            }
            else if (i >= 1)
            {
                return LevelPiece.NewPiece((LevelPiece._hallTypes)cycle);
            }

            cycle = (cycle + 1) % (int)LevelPiece._hallTypes.NUMELEMENTS;
            if (cycle == startVal)
            {
                i += (1 / maxIterations);
            }
        }

        Debug.Log("Reached end of LevelGenerator.ChoosePiece() which should not be possible");
        return null;
    }

    public LevelPiece ChooseRoomPiece()
    {
        int i = 0;
        int maxIterations = 10;

        int cycle = Random.Range(0, (int)LevelPiece._roomTypes.NUMELEMENTS);
        int startVal = cycle;

        while (true)
        {
            float roll = Random.Range(0f, 1f);
            if (roll < 0.5f)
            {
                roll -= (0.5f - roll) * shrinkLowRollForPiece;
            }

            //PieceWideFactor(pieceOptions[cycle].GetComponent<LevelPiece>())   // delete when replaced
            if (roll < LevelPiece.GetWideFactor((LevelPiece._roomTypes)cycle) * levelParams.wideFactor)
            {   // change to enum method	
                return LevelPiece.NewPiece((LevelPiece._roomTypes)cycle);
            }
            else if (i >= 1)
            {
                return LevelPiece.NewPiece((LevelPiece._roomTypes)cycle);
            }

            cycle = (cycle + 1) % (int)LevelPiece._roomTypes.NUMELEMENTS;
            if (cycle == startVal)
            {
                i += (1 / maxIterations);
            }
        }

        Debug.Log("Reached end of LevelGenerator.ChoosePiece() which should not be possible");
        return null;
    }


    public void CreateRoom(LevelPiece piece)
    {                                                          
        //GameObject newObj = (GameObject)Object.Instantiate ((Object)pieceType); // call new instantiation from LevelPiece
        //LevelPiece newPiece = newObj.GetComponent<LevelPiece> ();               // adjust
        //newPiece = piece;
        GameObject newRoom = new GameObject();
        // initiate newRoom through piece
        rooms.Add(newRoom);
	}

	public void ColorPiece(ref LevelPiece piece){
		float roll = Random.Range (0f, 1f);

		if (roll < levelParams.colorConsistency) {
			piece.levelColor = levelParams.baseColor;

		} else {
			Color newColor = levelParams.baseColor;
			newColor.r = ColorVariation (newColor.r);
			newColor.g = ColorVariation (newColor.g);
			newColor.b = ColorVariation (newColor.b);
            piece.levelColor = newColor;

        }
	}

	public float ColorVariation(float baseVal){
		float min = Mathf.Max (0f, baseVal - maxColorVariation);
		float max = Mathf.Min (1f, baseVal + maxColorVariation);
		return Random.Range(min, max);
	}

	public float PieceWideFactor(LevelPiece piece){                             // move to LevelPiece based on enum
		float max, min;
		if (piece.dimensions.x > piece.dimensions.y) {
			max = piece.dimensions.x;
			min = piece.dimensions.y;
		} else {
			max = piece.dimensions.y;
			min = piece.dimensions.x;
		}

		return (min / max);
	}

	public float HallBoostFromWide(){
		return (1f - levelParams.wideFactor) / inverseHallBoostFromWide;
	}
}
