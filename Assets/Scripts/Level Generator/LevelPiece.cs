using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPiece : MonoBehaviour {


	//public GameObject gameObj;
	public Vector2 dimensions;
	public Vector2 center;
	public Vector2 pos;
	public int minExits;
	public int maxExits;
	public List<Vector3> exits;
	public List<LevelPiece> children;
	public Color levelColor;
	public bool isHall;


	public Vector2 CreateFromEntrance(Vector3 otherExit, LevelPiece otherPiece){
		
		Vector3 processedInput = otherExit;
		processedInput += (Vector3)otherPiece.pos;

		return CreateFromEntrance (processedInput);
	}

	public Vector2 CreateFromEntrance(){
		return CreateFromEntrance (Vector3.zero);
	}

	// pass Vector3 using world coords, not default local coords
	// for local coords use above
	public Vector2 CreateFromEntrance(Vector3 otherExit){
		Vector2 entrance = new Vector2 (otherExit.x, otherExit.y);
		Vector2 incomingDirection = ExitCodeToDirection (otherExit.z);

		entrance += incomingDirection;


		Vector2 adjustedEntrance = AdjustEntrance (entrance, incomingDirection);
		exits.Add (adjustedEntrance);
		SetPosition (adjustedEntrance, entrance);

		return pos;
	}

	// ODOT - adjust for hall
	public Vector2 AdjustEntrance(Vector2 entrance, Vector2 incomingDirection){
		Vector2 adjustedEntrance = new Vector2 ();

		if (incomingDirection == Vector2.right) {
			adjustedEntrance.x = 0;
			adjustedEntrance.y = Random.Range (0, (int)dimensions.y);

		} else if (incomingDirection == Vector2.left) {
			adjustedEntrance.x = dimensions.x - 1;
			adjustedEntrance.y = Random.Range (0, (int)dimensions.y);

		} else if (incomingDirection == Vector2.up) {
			adjustedEntrance.y = 0;
			adjustedEntrance.x = Random.Range (0, (int)dimensions.x);

		} else if (incomingDirection == Vector2.down) {
			adjustedEntrance.y = dimensions.y - 1;
			adjustedEntrance.x = Random.Range (0, (int)dimensions.x);

		} else {
			Debug.Log ("LevelPiece.AdjustEntrance() received invalid direction vector");
		}

		return adjustedEntrance;
	}

	public void SetPosition(Vector2 adjustedEntrance, Vector2 entrance){
		pos = (center - adjustedEntrance) + entrance + new Vector2(0.5f, 0.5f);
	}

	public bool CollidesWith(LevelPiece otherPiece){
		return CollidesWith (otherPiece.center, otherPiece.dimensions);
	}

	public bool CollidesWith(Vector2 otherCenter, Vector2 otherDim){
		// horizontal overlap
		if (otherCenter.x + 0.5f + otherDim.x/2f > pos.x - dimensions.x/2f ||
			otherCenter.x + 0.5f - otherDim.x/2f < pos.x + dimensions.x/2f) {

			// vertical overlap
			if (otherCenter.y + 0.5f + otherDim.y/2f > pos.y - dimensions.y/2f ||
				otherCenter.y + 0.5f - otherDim.y/2f < pos.y + dimensions.y/2f) {

				return true;
			}
		}


		return false;
	}

	// ODOT - adjust for halls
	public Vector3 GenerateExit(){

		Vector3 newExit = new Vector3 (-1, -1, -1);
		bool xEdgeFirst;
		float roll = Random.Range (0f, 1f);


		// decide x or y first, then whether 0 side or max side
		if (roll < 0.5f) {
			
			xEdgeFirst = true;
			roll = Random.Range (0f, 1f);

			if (roll < 0.5f) {
				newExit.x = 0;
			} else {
				newExit.x = dimensions.x - 1f;
			}
		} else {

			xEdgeFirst = false;
			roll = Random.Range (0f, 1f);

			if (roll < 0.5f) {
				newExit.y = 0;
			} else {
				newExit.y = dimensions.y - 1f;
			}
		}

		// generate remaining variable between 0 and max, exclusive
		if (!isHall) {
			if (xEdgeFirst) {
				newExit.y = Random.Range (1, (int)(dimensions.y - 1));
			} else {
				newExit.x = Random.Range (1, (int)(dimensions.x - 1));
			}
		} else {
			if (xEdgeFirst) {
				newExit.y = Random.Range (0, (int)(dimensions.y - 1));
			} else {
				newExit.x = Random.Range (0, (int)(dimensions.x - 1));
			}
		}

		// assign direction code
		newExit.z = ExitDirectionToCode(ExitDirection ((Vector2)newExit));

		/*
		if (!ValidExit ((Vector2)newExit)) {
			Debug.Log ("LevelPiece.GenerateExit generated an invalid exit point" + newExit + "   levelPiece info " + dimensions);
		}
		*/

		exits.Add (newExit);
		Vector3 result = newExit + (Vector3)pos;	// needs to return world coords, not local
		return result;
	}

	public void AddExit(Vector3 newExit){
		if(ValidExit(newExit))
			exits.Add(newExit);
	}

	public bool ValidExit(Vector3 check){
		return ValidExit ((Vector2)check);
	}

	public bool ValidExit(Vector2 check){
		return ValidExit((int)check.x, (int)check.y);
	}

	public bool ValidExit(int x, int y){
		if (!isHall) {
			if (x < dimensions.x - 1 && x != 0 &&
			    (y == 0 || y == dimensions.y - 1)) {
				return true;
			}

			if (y < dimensions.y - 1 && y != 0 &&
			    (x == 0 || x == dimensions.x - 1)) {
				return true;
			}
		} else {
			if (x < dimensions.x && x >= 0 &&
				(y == 0 || y == dimensions.y - 1)) {
				return true;
			}

			if (y < dimensions.y && y >= 0 &&
				(x == 0 || x == dimensions.x - 1)) {
				return true;
			}
		}

		return false;
	}


	public int ExitDirectionToCode(Vector2 v){
		if (v == Vector2.up)
			return 0;
		if (v == Vector2.right)
			return 1;
		if (v == Vector2.down)
			return 2;
		if (v == Vector2.left)
			return 3;

		Debug.Log ("Reached end of LevelPiece.ExitDirectionToCode, invalid input received.");
		return -1;
	}

	public Vector2 ExitCodeToDirection(float i){
		return ExitCodeToDirection ((int)i);
	}

	public Vector2 ExitCodeToDirection(int i){
		switch (i) {
		case 0:
			return Vector2.up;
			break;
		case 1:
			return Vector2.right;
			break;
		case 2:
			return Vector2.down;
			break;
		case 3:
			return Vector2.left;
			break;
		}

		Debug.Log ("Reached end of LevelPiece.ExitCodeToDirection, invalid input received." + i);
		return new Vector2 (-1, -1);
	}

	public Vector2 ExitDirection(Vector2 check){
		return ExitDirection ((int)check.x, (int)check.y);
	}

	public Vector2 ExitDirection(int x, int y){
		Vector2 noResult = new Vector2(0,0);

		if (x == 0)
			return Vector2.left;

		if (x == dimensions.x - 1)
			return Vector2.right;

		if (y == 0)
			return Vector2.down;

		if (y == dimensions.y - 1)
			return Vector2.up;

		Debug.Log ("Reached end of LevelPiece.ExitDirection, invalid input received.");
		return noResult;
	}
}
