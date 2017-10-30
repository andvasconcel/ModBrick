using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ModBrick
{
	// measurements in millimeters
    public static class ModBrickMetrics
    {
		public const float Unit = 8f;
		public const float FullHeight = Unit*1.2f;
		public const float ThirdHeight = FullHeight/3f;
		public const float Margin = Unit/5f;

		// note to self: rods are placed in the middle of every unit
		public const float KnobHeight = FullHeight/6f;
		public const float KnobRadius = FullHeight/4f;

		// hardcoded
		public const float TubeRadiusInner = 2.5f;
		public const float TubeRadiusOuter = 3.3f;
		public const float TubeHeight = Unit; // in case you want a different height for tubes and rods 

		public const float RodRadius = 1.6f;
    }
}