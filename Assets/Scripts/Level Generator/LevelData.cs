using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : EvolvingEntity {

    public int numTraits = 4;

    public float branchFactor;      // 0 = linear	1 = branching
    public float wideFactor;        // 0 = narrow 	1 = wide
    public float colorConsistency;  // 0 = eratic	1 = consistent
    public float colorIntensity;    // 0 = low		1 = high
    public float colorBrightness;   // 0 = low		1 = high

    public Color baseColor;
    // move to palette?
    public enum _colors { RED, GREEN, BLUE };
    private _colors cmin;
    private _colors cmid;
    private _colors cmax;

    protected override float FitEval(bool updateFitness = true) {
        float result = 0f;

        result += Mathf.Abs(0.5f - branchFactor);
        result += Mathf.Abs(0.5f - wideFactor);
        result += Mathf.Abs(0.5f - colorConsistency);
        EvalColors();
        result += Mathf.Abs(0.5f - colorIntensity);
        float brightGoodness = 0;
        if (colorBrightness > 0.5f) {
            brightGoodness = 0.5f;
        } else {
            brightGoodness = colorBrightness;
        }

        //Debug.Log("branch = " + Mathf.Abs (0.5f - branchFactor) + " || wide = " + Mathf.Abs (0.5f - wideFactor) + " || cConsist = " + Mathf.Abs (0.5f - colorConsistency) + " || cIntens = " + Mathf.Abs (0.5f - colorIntensity));

        result *= 2f / (float)numTraits;

        fitness = result;
        return result;
    }

    protected override float FitNormalize(float total, float max, float min = 0f, bool updateFitness = false) {
        float result = (fitness - min) / (max - min);

        if (updateFitness)
            fitness = result;

        return result;
    }

    public void EvalColors() {
        float max = baseColor.r, mid = baseColor.r, min = baseColor.r;
        cmax = cmin = cmid = _colors.RED;

        ColorSetupGreen(ref min, ref max);
        ColorSetupBlue(ref min, ref mid, ref max);

        colorIntensity = max - min;
        //colorBrightness = (baseColor.r + baseColor.g + baseColor.b) / 3f;
    }

    private void ColorSetupGreen(ref float min, ref float max)
    {
        if (baseColor.g > max)
        {
            max = baseColor.g;
            cmax = _colors.GREEN;
        }
        else
        {
            min = baseColor.g;
            cmin = _colors.GREEN;
        }
    }

    private void ColorSetupBlue(ref float min, ref float mid, ref float max)
    {
        if (baseColor.b > max)
        {
            mid = max;
            cmid = cmax;
            max = baseColor.b;
            cmax = _colors.BLUE;
        }
        else if (baseColor.b < min)
        {
            mid = min;
            cmid = cmin;
            min = baseColor.b;
            cmin = _colors.BLUE;
        }
        else
        {
            mid = baseColor.b;
            cmid = _colors.BLUE;
        }
    }

    public override EvolvingEntity MateWith(EvolvingEntity otherParent) {
        LevelData child = new LevelData();
        LevelData mate = (LevelData)otherParent;

        child.branchFactor = PickParentValue(this, mate).branchFactor;
        child.wideFactor = PickParentValue(this, mate).wideFactor;
        child.colorConsistency = PickParentValue(this, mate).colorConsistency;
        child.colorIntensity = PickParentValue(this, mate).colorIntensity;

        child.baseColor.a = 1f;
        child.cmax = PickParentValue(this, mate).cmax;
        LevelData tempParent = PickParentValue(this, mate);
        _colors tempMin = tempParent.cmin;
        if (tempMin == child.cmax)
            tempMin = OtherParent(tempParent, this, mate).cmin;
        if (tempMin == child.cmin)
            child.cmin = LastColor(tempMin, child.cmax);
        else
            child.cmin = tempMin;
        child.GenColor();

        // ODOT - colorBrightness

        // GenColor not working as intended, had to inherit color directly
        child.baseColor = PickParentValue(this, mate).baseColor;

        return child;
    }

    private LevelData PickParentValue(LevelData parent1, LevelData parent2)
    {
        int roll = Random.Range(0, 2);
        if (roll > 0)
            return parent1;
        else
            return parent2;
    }

    private LevelData OtherParent(LevelData input, LevelData parent1, LevelData parent2)
    {
        if (input == parent1)
            return parent2;
        return parent1;
    }

	public _colors LastColor(_colors c1, _colors c2){
		if (c1 == _colors.RED || c2 == _colors.RED) {
			if (c1 == _colors.BLUE || c2 == _colors.BLUE) {
				return _colors.GREEN;
			} else
				return _colors.BLUE;
		}
		return _colors.RED;
	}

	public void GenColor(){

		baseColor = new Color ();
		baseColor.a = 1f;

		float max = -1f, min = -1f;

		// set cmax between colorintensity and colorbrightness
		switch (cmax) {
		case _colors.RED:
			baseColor.r = Random.Range (colorIntensity, colorBrightness);
			max = baseColor.r;
			break;
		case _colors.GREEN:
			baseColor.g = Random.Range (colorIntensity, colorBrightness);
			max = baseColor.g;
			break;
		case _colors.BLUE:
			baseColor.b = Random.Range (colorIntensity, colorBrightness);
			max = baseColor.b;
			break;
		default:
			Debug.Log ("Invalid cmax color in GenColor()");
			break;
		}

		// set cmin between 0 and cmax - colorintensity
		switch (cmin) {
		case _colors.RED:
			baseColor.r = Random.Range (0f, max - colorIntensity);
			min = baseColor.r;
			break;
		case _colors.GREEN:
			baseColor.g = Random.Range (0f, max - colorIntensity);
			min = baseColor.g;
			break;
		case _colors.BLUE:
			baseColor.b = Random.Range (0f, max - colorIntensity);
			min = baseColor.b;
			break;
		default:
			Debug.Log ("Invalid cmax color in GenColor()");
			break;
		}

		switch (cmid) {
		case _colors.RED:
			baseColor.r = Random.Range (min, max);
			break;
		case _colors.GREEN:
			baseColor.g = Random.Range (min, max);
			break;
		case _colors.BLUE:
			baseColor.b = Random.Range (min, max);
			break;
		default:
			Debug.Log ("Invalid cmax color in GenColor()");
			break;
		}
	}

	protected override void InitRandom(){
		branchFactor = Random.Range (0f, 1f);
		wideFactor = Random.Range (0f, 1f);
		colorConsistency = Random.Range (0f, 1f);

		baseColor.r = Random.Range (0f, 1f);
		baseColor.g = Random.Range (0f, 1f);
		baseColor.b = Random.Range (0f, 1f);
		baseColor.a = 1f;
	}
		

	//ODOT - dictionary or enum conversion
	public override void Mutate(int traitNum, float mutateBy){
		switch (traitNum) {
		case 0:
			MutateTrait (ref branchFactor, mutateBy);
			break;
		case 1:
			MutateTrait (ref wideFactor, mutateBy);
			break;
		case 2:
			MutateTrait (ref colorConsistency, mutateBy);
			break;
		case 3:
			MutateTrait (ref colorIntensity, mutateBy);
			break;
		}
	}

	private void MutateTrait(ref float trait, float mutateBy){
		trait += mutateBy;
        Mathf.Clamp01(trait);
	}

	public override void PrintInfo(){
		Debug.Log ("fitness = " + FitEval() + " || branchFactor = " + branchFactor + " || wideFactor = " + wideFactor + " || colorConsistency = " + colorConsistency + " || color = " + baseColor);
	}
}
