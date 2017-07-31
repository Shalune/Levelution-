using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneticSolver : MonoBehaviour {

	public int maxGenerations;
	public float fitSolutionThreshold;

	void Awake(){
		Init ();
	}

	protected abstract void Init ();
	protected abstract void DisplayPopulationData ();
	protected abstract void DisplaySolution();
}