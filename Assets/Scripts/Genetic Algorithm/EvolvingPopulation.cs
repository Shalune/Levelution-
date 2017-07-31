using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// remove //TEMP after EE is updated

public class EvolvingPopulation<T> : MonoBehaviour where T: EvolvingEntity, new() {

	public int popSize = 20;
	public List<T> population;
	public bool mutationOn = true;
	public float minMutate = 0.01f;
	public float maxMutate = 0.1f;
	public int mutationsPerGeneration = 1;

	// ------------------------------------------------------------ META STRUCTURE -------------------------------------------------------------

	// ------------------------------------------------------------ POPULATE -------------------------------------------------------------

	// create / populate new generation, default
	public void NewGeneration(){
		NewGeneration (popSize);
	}

	// specific values
	public void NewGeneration(int n){
		popSize = n;

		if (population == null) {
			Populate (n);

		} else {
			List<T> children = new List<T>();
			for (int i = 0; i < n; i++) {
				children.Add( CreateChild () );
			}
			population = children;

			if(mutationOn)
				MutatePopulation ();
		}
	}

	// create new population of evolving entities from scratch
	private void Populate(int n){
		population = new List<T>();

		for (int i = 0; i < n; i++) {
			T newEntity = new T ();
			newEntity.Randomize ();
			population.Add (newEntity);
		}
	}

	// ------------------------------------------------------------ EVALUATE -------------------------------------------------------------

	// evaluate Fitness() of entities individually and then by aggregate
	public void Evaluate(){
		float total = 0f;
		bool firstSet = false;
		float min = 0f;
		float max = 0f;

		foreach (T entity in population) {
			entity.Fitness() ;
			total += entity.Fitness();

			if (!firstSet) {
				min = entity.Fitness();
				max = entity.Fitness();
			} else {
				if (entity.Fitness() > max) {
					max = entity.Fitness();
				}
				if (entity.Fitness() < min) {
					min = entity.Fitness();
				}
			}
		}
		//NormalizeEval (total, min, max);
	}

	private void NormalizeEval(float total, float min, float max){
		foreach (T entity in population) {
			//entity.FitNormalize (total, max, min, true);
		}
	}

	// ------------------------------------------------------------ REPRODUCE -------------------------------------------------------------

	public T CreateChild(){
		T parent0 = RandomFitEntity ();
		T parent1 = RandomFitEntity ();

		/*
		while (parent0 == parent1) {
			parent1 = RandomFitEntity ();
		}*/

		T child = (T)parent0.MateWith (parent1);

		return child;
	}

	public T RandomFitEntity(){

		T result = null;
		int i = 0;
		int maxIterations = 10;

		//int debugJ = 0;

		int cycle = Random.Range (0, population.Count);
		int startVal = cycle;

		while (true) {

			//foreach(T entity in population){
				float roll = Random.Range (0f, 1f) * 2;

				//Debug.Log ("J = " + debugJ + " || roll = " + roll + " || fit = " + entity.Fitness ());
				//debugJ++;


			if (roll < SpreadFitness(population[cycle].Fitness()) + i) {
					result = population[cycle];
					return result;
				} else if (i >= 1) {
					result = population[cycle];
					return result;
				}
			//}

			cycle = (cycle + 1) % population.Count;
			if (cycle == startVal) {
				i+= (1/maxIterations);
			}
		}

		Debug.Log ("Reached end of EvolvingPopulation.RandomFitEntity() which should not be possible");
		return null;
	}

	private float SpreadFitness(float rawFit){
		float change = (1f - rawFit)/2f;
		if (rawFit/2f < change) 
			change = (rawFit/2f) * -1f;

		return rawFit + change;
	}

	public T GetMostFit(){
		T result = null;
		bool maxSet = false;
		float maxFit = 0;

		foreach (T entity in population) {

			//entity.PrintInfo ();

			if (!maxSet) {
				maxSet = true;
				maxFit = entity.Fitness();
				result = entity;

			} else if (entity.Fitness() > maxFit) {
				maxFit = entity.Fitness();
				result = entity;
			}
		}

		return result;
	}

	// ------------------------------------------------------------ MUTATE -------------------------------------------------------------

	private void MutatePopulation(){
		// limit number of mutations to number of traits
		if (mutationsPerGeneration > population[0].numTraits)
			mutationsPerGeneration = population[0].numTraits;

		// for every mutation per generation, and every entity in the population, generate a mutation
		for (int i = 0; i < mutationsPerGeneration; i++) {
			foreach (T entity in population) {

				// choose from traits, amount to mutate, whether negative/positive
				int trait = Random.Range (0, population[0].numTraits);
				float mutation = Random.Range (minMutate, maxMutate);
				if (Random.Range (0, 2) > 0) {
					mutation *= -1;
				}

				entity.Mutate (trait, mutation);
			}
		}
	}
}
