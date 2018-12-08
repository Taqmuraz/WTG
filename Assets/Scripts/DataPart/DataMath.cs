using System;
using RPG_Mechanic;
using UnityEngine;

namespace RPG_Data
{
	[Serializable]
	public struct WorldPoint
	{
		public float x;
		public float y;
		public float z;

		public WorldPoint (float _x, float _y, float _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}

		public static implicit operator WorldPoint (Vector3 v)
		{
			return new WorldPoint (v.x, v.y, v.z);
		}
		public static implicit operator Vector3 (WorldPoint w)
		{
			return new Vector3 (w.x, w.y, w.z);
		}
	}
}

