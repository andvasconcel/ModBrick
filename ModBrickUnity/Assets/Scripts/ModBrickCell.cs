using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModBrick.Utility;

namespace ModBrick
{
	public class ModBrickCell 
	{
		public float Distance; // distance from brick (brick center?)
		public ModBrickGrid CellGrid; // grid this cell belongs to
		public Vector3 WorldPos; // what could this be
		public Vector3I GridPos; // position of cell in grid

		public ModBrickCell(float distance, ModBrickGrid grid, Vector3 worldPos, Vector3I gridPos)
		{
			Distance = distance;
			CellGrid = grid;
			WorldPos = worldPos;
			GridPos = gridPos;
		}
	}
}