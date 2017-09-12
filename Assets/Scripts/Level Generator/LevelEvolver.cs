using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelTemplateLibrary))]
public class LevelEvolver : GeneticSolver {

	private bool generateLevel = false;
	LevelTemplateLibrary library;
	EvolvingPopulation<LevelData> ePopulation;
	LevelData solution = null;
	bool firstGen = true;

	float previousBestFit = 0f;
	float previousAvgFit = 0f;
	float avgFit = 0f;

	protected override void Init (){
		library = GetComponent<LevelTemplateLibrary> ();

		ePopulation = new EvolvingPopulation<LevelData> ();
		ePopulation.NewGeneration ();
        //DisplayImprovement ();
        GetFitInfo();
		firstGen = false;

		for (int i = 0; i < maxGenerations; i++) {
			ePopulation.NewGeneration ();
			//DisplayImprovement ();
		}
			

		//DisplayImprovement ();
		DisplaySolution ();

		// generate level from solution
		if (generateLevel) {
			LevelGenerator generator = new LevelGenerator ();
			generator.LoadTemplates (library.roomPieces, library.hallPieces);
			generator.GenerateAndInstantiate (solution);
		}
	}

	private void DisplayImprovement(){
		GetFitInfo ();
		float changeBest = solution.Fitness () - previousBestFit;
		float changeAvg = avgFit - previousAvgFit;
		//Debug.Log (previousBestFit);
		Debug.Log ("Change in best fitness since last generation: " + changeBest);
		Debug.Log ("Average fitness for generation: " + avgFit + "     Change in average fitness since last generation: " + changeAvg);
	}

	private void GetFitInfo(){
		if (!firstGen) {
			//Debug.Log ("getting prev " + solution.Fitness ());
			previousBestFit = solution.Fitness ();
		}
		solution = ePopulation.GetMostFit ();
		GetAvgFit ();
	}

	private void GetAvgFit(){
		previousAvgFit = avgFit;
		avgFit = 0f;
		foreach(LevelData data in ePopulation.population){
			avgFit += data.Fitness ();
		}

		avgFit /= ePopulation.population.Count;
	}

	protected override void DisplayPopulationData (){
		Debug.Log ("One small step");
	}

	protected override void DisplaySolution(){
		Debug.Log ("================ Solution ==============");
		solution.PrintInfo ();
	}
}