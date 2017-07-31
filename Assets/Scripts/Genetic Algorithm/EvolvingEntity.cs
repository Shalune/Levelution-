using UnityEngine;
using System.Collections;

public abstract class EvolvingEntity : MonoBehaviour {

	public int numTraits;		// MUST SET numtraits in subclasses for Mutate to work in EvolvingPopulation
	protected float fitness;
	protected bool evaluated = false;

	public void Randomize(){
		InitRandom ();
		evaluated = false;
	}

	public float Fitness(bool updateFitness = true){
		if (evaluated) {
			return fitness;
		} else {
			float result = FitEval (updateFitness);
			evaluated = true;
			return result;
		}
	}

	protected abstract float FitEval (bool updateFitness = true);
	protected abstract float FitNormalize (float total, float max, float min = 0f, bool updateFitness = false);
	public abstract EvolvingEntity MateWith (EvolvingEntity otherParent);
	protected abstract void InitRandom ();
	public abstract void PrintInfo ();
	public abstract void Mutate(int traitNumber, float mutateBy);
}
