using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ColorPickerUnity
{
    public static class ColorExtensions
    {
		public static Color RandomHueColor(float saturation, float lightness)
		{
			var hue = Random.Range(0, 360f);
			var hsl = new RGBHSL(hue, saturation, lightness);
			return ColorConverter.ConvertRGBHSL(hsl);
		}
    }
}