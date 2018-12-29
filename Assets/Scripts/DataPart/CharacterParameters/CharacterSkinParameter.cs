using System;
using RPG_Mechanic;
using RPG_System;
using UnityEngine;
using System.Linq;

namespace RPG_Data
{
	[Serializable]
	public struct CharacterColor
	{
		public string name;
		public float r;
		public float g;
		public float b;
		public float a;

		public CharacterColor (string _name, Color c)
		{
			r = c.r;
			g = c.g;
			b = c.b;
			a = c.a;
			name = _name;
		}

		public Color ToColor ()
		{
			return new Color (r, g, b, a);
		}
	}

	[System.Serializable]
	public class CharacterSkinParameter : CharacterParameter
	{
		protected CharacterColor[] colors { get; private set; }

		public CharacterSkinParameter (string id, params CharacterColor[] _colors) : base (id)
		{
			colors = _colors;
		}

		public Color GetColor (string name)
		{
			return colors.FirstOrDefault ((CharacterColor c) => c.name.StartsWith (name)).ToColor();
		}
	}
	
}
