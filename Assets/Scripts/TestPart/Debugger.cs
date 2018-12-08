using System;
using UnityEngine;

public class Debugger
{
	public static void Log (object obj)
	{
		Debug.Log (obj.ToString ());
	}
}

