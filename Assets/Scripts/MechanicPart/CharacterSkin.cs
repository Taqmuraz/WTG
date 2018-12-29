using System;
using UnityEngine;
using RPG_System;
using RPG_Data;

namespace RPG_Mechanic
{
	public class CharacterSkin : AliveComponent<Renderer>
	{
		public CharacterSkin (AliveOverlay basedOn) : base (basedOn)
		{
		}
		public override void Initialize ()
		{
			UpdateSkin ();
		}
		protected virtual void UpdateSkin ()
		{
			CharacterSkinParameter skinParam = (relativeObject.GetData () as CharacterData).parameters.GetOfType<CharacterSkinParameter> ();
			Renderer[] rends = gameObj.GetComponentsInChildren<Renderer> ();
			if (skinParam) {
				if (rends.Length < 2) {
					foreach (var mat in component.materials) {
						mat.color = skinParam.GetColor (mat.name.FirstWord());
					}
				} else {
					foreach (var rend in rends) {
						rend.material.color = skinParam.GetColor (rend.material.name.FirstWord ());
					}
				}
			}
		}
	}
	
}
