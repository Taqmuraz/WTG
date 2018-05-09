using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeClass
{

	public delegate void TimeEvent ();

	private static float timeGetter;
	public static float time
	{
		get {
			return timeGetter;
		}
		set {
			float h = hours;
			float d = days;
			timeGetter = value;
		}
	}
	public static float timeSpeed = 1f;
	public static int pointsInDay
	{
		get {
			return 2;
		}
	}

	public static TimeEvent onDayChanged = delegate() {
		return;
	};
	public static TimeEvent onHourChanged = delegate() {
		return;
	};

	private static int day_probe = -1;
	public static int days
	{
		get {
			int a = (int)time / pointsInDay;
			if (day_probe != a) {
				day_probe = a;
				onDayChanged ();
			}
			return a;
		}
	}
	public static int hour_probe = -1;
	public static int hours
	{
		get {
			int a = ((int)time % pointsInDay) * (24 / pointsInDay);
			if (hour_probe != a) {
				hour_probe = a;
				onHourChanged ();
			}
			return a;
		}
	}
}

[System.Serializable]
public struct SLightmapArray
{
	public SLightmap[] lightmaps;
}

[System.Serializable]
public struct SLightmap
{
	public Texture2D dir;
	public Texture2D light;
}

public class ILightmapSetter : MonoBehaviour {

	public int lightmapIndex = 0;
	private int probe = 0;

	public SLightmapArray[] array;

	private void Start () {
		TimeClass.onHourChanged = delegate {
			SetLightmap (TimeClass.hours * TimeClass.pointsInDay / 24);
		};
	}

	private void Update () {
		if (lightmapIndex != probe) {
			SetLightmap (lightmapIndex);
			probe = lightmapIndex;
		}
	}
	public void SetLightmap (int index) {
		LightmapData[] data = new LightmapData[array[index].lightmaps.Length];
		for (int i = 0; i < data.Length; i++) {
			data [i] = new LightmapData ();
			data [i].lightmapColor = array [index].lightmaps [i].light;
			data [i].lightmapDir = array [index].lightmaps [i].dir;
		}
		LightmapSettings.lightmaps = data;
	}
}
