using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gene {

	public float fitWeight;

	public abstract float WeightedValue ();
};

public class Gene<DataType> : Gene
{
	public DataType value;

	public override float WeightedValue(){
		return 0f;
	}
};