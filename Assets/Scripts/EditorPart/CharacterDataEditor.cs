using UnityEngine;
using System.Collections;
using RPG_Data;

namespace RPG_Editor
{
	public class CharacterDataEditor : AliveObjectDataEditor
	{
		[SerializeField] protected ColorParameter color_head = new ColorParameter("hair", Color.black);
		[SerializeField] protected ColorParameter color_cloth_0 = new ColorParameter("cloth_0", Color.green);
		[SerializeField] protected ColorParameter color_cloth_1 = new ColorParameter("cloth_1", Color.red);
		[SerializeField] protected ColorParameter color_skin = new ColorParameter("skin", Color.gray);

		public override ObjectData GenerateData ()
		{
			Transform trans = transform;

			string _id = id.GetValue<string> ();

			CharacterSkinParameter skin = new CharacterSkinParameter (_id,
				                              color_head.GetCharacterColor (),
				                              color_cloth_0.GetCharacterColor (),
				                              color_cloth_1.GetCharacterColor (),
				                              color_skin.GetCharacterColor ());

			CharacterData dat = new CharacterData (_id, prefab.GetValue<string>(), skin);
			dat.position = trans.position;
			dat.euler = trans.eulerAngles;
			return dat;
		}
	}
}
