using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ModBrick.Utility
{
	[System.Serializable]
    public class Vector3I
    {
		public int x;
		public int y;
		public int z;

		public Vector3I(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", x, y, z);
		}
    }
}