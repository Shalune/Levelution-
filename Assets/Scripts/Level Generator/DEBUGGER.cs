using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGGER : MonoBehaviour {

	public GameObject testObj;
	public GameObject pieceTemplate;

	private float thing1;

	void Awake(){
		//LevelPiece piece = pieceTemplate
	}

	private void DoStuff(ref float changeThis, float changeBy){
		changeThis += changeBy;
	}

	private void OldTest(){
		LevelData testLevel = new LevelData ();

		testLevel.Randomize ();
		testLevel.PrintInfo ();

		LevelData mateLevel = new LevelData ();
		mateLevel.Randomize ();
		mateLevel.PrintInfo ();

		LevelData child = (LevelData)testLevel.MateWith (mateLevel);
		child.PrintInfo ();


		EvolvingPopulation<LevelData> ePop = new EvolvingPopulation<LevelData> ();
		ePop.NewGeneration ();
		LevelData testPull = ePop.RandomFitEntity ();
		Debug.Log ("testpull");
		testPull.PrintInfo ();

		LevelData bestPull = ePop.GetMostFit ();
		Debug.Log ("bestpull");
		bestPull.PrintInfo ();

		float bestFit = bestPull.Fitness();
		int timesBest = 0;
		/*
		for (int i = 0; i < 1000; i++) {
			testPull = ePop.RandomFitEntity ();
			if (testPull.Fitness() == bestFit) {
				timesBest++;
			}
		}
		*/

		//Debug.Log ("odds of randomly best = " + timesBest);

		child = (LevelData) ePop.CreateChild ();
		child.PrintInfo ();

		ePop.NewGeneration ();
		bestPull = ePop.GetMostFit ();
		bestPull.PrintInfo ();
	}
}
