using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palette : MonoBehaviour {

	public enum _paletteType {MONOCHROME, ANALAGOUS, TRIAD, COMPLEMENTARY, UNSTRUCTURED, RANDOM};
	public _paletteType paletteType;
	public Color baseColor;
	private static float deadColor = -1;

	public static List<Color> GeneratePalette(Color initialColor, _paletteType type){
		List<Color> outputPalette = new List<Color> ();
        Color newColor = new Color();

		if(IsDeadColor(initialColor) || type == _paletteType.RANDOM){
            newColor.r = Random.Range(0f,1f);
            newColor.g = Random.Range(0f,1f);
            newColor.b = Random.Range(0f,1f);
		}

        outputPalette.Add(newColor);
		return outputPalette;
	}

	public static List<Color> GeneratePalette(_paletteType type){
		return GeneratePalette (new Color (deadColor, deadColor, deadColor), type);
	}

	public static List<Color> GeneratePalette(Color initialColor){
		return GeneratePalette (initialColor, _paletteType.RANDOM);
	}

	private static bool IsDeadColor(Color inColor){
		if (inColor.r < 0 || inColor.g < 0 || inColor.b < 0) {
			return true;
		}
		return false;
	}
}
