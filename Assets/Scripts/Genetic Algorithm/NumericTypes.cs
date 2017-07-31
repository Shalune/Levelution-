using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NumericTypes : MonoBehaviour {

	private static List<Type> numTypes = null;

	private static void InitList(){
		numTypes = new List<Type> {typeof(int), 
			typeof(Double), 
			typeof(float), 
			typeof(decimal),
			typeof(Int16), 
			typeof(Int32), 
			typeof(Int64),
			typeof(UInt16), 
			typeof(UInt32), 
			typeof(UInt64)
		};
	}

	public static bool IsNumeric(Type checkType){
		if (numTypes == null) {
			NumericTypes.InitList ();
		}
		return NumericTypes.numTypes.Contains (checkType);
	}
}
