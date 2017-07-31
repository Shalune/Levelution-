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

	LevelPiece headNode;
	public float hallProbability = 0.5f;
	public Queue<Vector3> unexpandedExits;
	public List<LevelPiece> levelPieces;
	public List<GameObject> roomTemplates;
	public List<GameObject> hallTemplates;

	public void Init(){
		unexpandedExits = new Queue<Vector3> ();
		levelPieces = new List<LevelPiece> ();
	}

	public void LoadTemplates(List<GameObject> rooms, List<GameObject> halls){
		roomTemplates = rooms;
		hallTemplates = halls;
		Debug.Log ("loaded : " + roomTemplates.Count + "  " + hallTemplates.Count);
	}

	public void GenerateAndInstantiate(LevelData inputParams){
		GenerateLevel (inputParams);
		SetupLevel (headNode);
	}

	public LevelPiece GenerateLevel(LevelData inputParams){
		if (unexpandedExits == null || levelPieces == null) {
			Init ();
		}

		levelParams = inputParams;
		CreateStartingRoom ();

		while (levelPieces.Count < maxRooms && unexpandedExits.Count != 0) {
			CreateRoom ();
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

	public void CreateStartingRoom(){
		GameObject roomType = ChoosePiece ();
		headNode = roomType.GetComponent<LevelPiece> ();
		headNode.CreateFromEntrance ();
		GenerateExitsFor (headNode);
		ColorPiece (ref headNode);

		CreatePiece (roomType, headNode);
		Debug.Log ("Created first room " + headNode.pos + "    " + headNode.dimensions);
		levelPieces.Add (headNode);
	}

	public void CreateRoom(){
		Vector3 startingExit = unexpandedExits.Dequeue();
		GameObject pieceType = ChoosePiece();
		LevelPiece piece = pieceType.GetComponent<LevelPiece> ();
		piece.CreateFromEntrance(startingExit);

		int attempts = 0;

		while (CollidesWithLevel ((Vector2)piece.pos, piece.dimensions)) {
			pieceType = ChoosePiece();
			piece = pieceType.GetComponent<LevelPiece> ();
			piece.CreateFromEntrance(startingExit);

			attempts++;
			if (attempts > reAttemptPlacementThreshold) {
				break;
			}
		}

		GenerateExitsFor(piece);
		ColorPiece (ref piece);

		CreatePiece (pieceType, piece);
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

	public GameObject ChoosePiece(bool previousIsRoom = false){
		// ODOT change likelihood of room vs hall vased on previous
		float roll = Random.Range (0f, 1f);

		if (roll < hallProbability + HallBoostFromWide()) {
			return ChoosePiece (hallTemplates);
		} else {
			return ChoosePiece (roomTemplates);
		}
	}
		
	public GameObject ChoosePiece(List<GameObject> pieceOptions){

		GameObject result = null;
		int i = 0;
		int maxIterations = 10;

		int cycle = Random.Range (0, pieceOptions.Count);
		// Debug.Log ("count = " + pieceOptions.Count + " || cycle = " + cycle);
		int startVal = cycle;

		while (true) {
			float roll = Random.Range (0f, 1f);
			if (roll < 0.5f) {
				roll -= (0.5f - roll) * 0.75f;		// ODOT - replace magic numbers
			}

			if (roll < PieceWideFactor(pieceOptions[cycle].GetComponent<LevelPiece>()) * levelParams.wideFactor) {		
				result = pieceOptions[cycle];
				return result;
			} else if (i >= 1) {
				result = pieceOptions[cycle];
				return result;
			}

			cycle = (cycle + 1) % pieceOptions.Count;
			if (cycle == startVal) {
				i+= (1/maxIterations);
			}
		}

		Debug.Log ("Reached end of LevelGenerator.ChoosePiece() which should not be possible");
		return null;
	}

	public void CreatePiece(GameObject pieceType, LevelPiece piece){
		GameObject newObj = (GameObject)Object.Instantiate ((Object)pieceType);
		LevelPiece newPiece = newObj.GetComponent<LevelPiece> ();
		newPiece = piece;
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
		}
	}

	public float ColorVariation(float baseVal){
		float min = Mathf.Max (baseVal * -1, maxColorVariation * -1);
		float max = Mathf.Min (1f - baseVal, maxColorVariation);
		return Random.Range(min, max);
	}

	public float PieceWideFactor(LevelPiece piece){
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
		return (1f - levelParams.wideFactor) / 7f;
	}
}
