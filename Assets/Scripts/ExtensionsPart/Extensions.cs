using System;
using UnityEngine;
using System.Linq;

public static class Extensions
{
	public const char WordSeparator = ' ';
	public const char LineSeparator = '*';

	public static Vector3 Flat (this Vector3 origin)
	{
		return new Vector3 (origin.x, 0, origin.z);
	}
	public static string FirstWord (this string text)
	{
		return text.GetWords ().FirstOrDefault ();
	}
	public static string[] GetWords (this string text)
	{
		return text.SmartSplit (WordSeparator);
	}
	public static string[] GetLines (this string text)
	{
		return text.SmartSplit(LineSeparator);
	}
	public static string[] SmartSplit (this string text, char separator)
	{
		return text.Split (separator).Where ((string s) => !string.IsNullOrEmpty (s)).ToArray ();
	}
}

