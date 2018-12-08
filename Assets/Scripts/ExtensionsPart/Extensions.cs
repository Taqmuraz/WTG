using System;
using UnityEngine;

public static class Extensions
{
	public static Vector3 Flat (this Vector3 origin)
	{
		return new Vector3 (origin.x, 0, origin.z);
	}
}

