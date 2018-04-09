using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ILightmapArray
{
	public ILightmap[] lightmaps;
}

[System.Serializable]
public struct ILightmap
{
	public Texture2D dir;
	public Texture2D light;
}

public class ILightmapSetter : MonoBehaviour {

	public int lightmapIndex = 0;
	private int probe = 0;

	public ILightmapArray[] array;

	private void Start () {
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
