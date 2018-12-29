using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG_Data;
using System;

namespace RPG_Editor
{
	[Serializable]
	public class ColorParameter : EditorParameter
	{
		[SerializeField] Color color;

		public ColorParameter (string _name, Color _color) : base (_name)
		{
			color = _color;
		}

		public override object GetValue ()
		{
			return color;
		}
		public CharacterColor GetCharacterColor ()
		{
			return new CharacterColor (name, color);
		}
	}
	[Serializable]
	public class StringParameter : EditorParameter
	{
		[SerializeField] string value;

		public StringParameter (string _name, string _value) : base (_name)
		{
			value = _value;
		}

		public override object GetValue ()
		{
			return value;
		}
	}
	[Serializable]
	public abstract class EditorParameter
	{
		public string name { get; private set; }

		public EditorParameter (string _name)
		{
			name = _name;
		}
		public abstract object GetValue ();
		public T GetValue<T> ()
		{
			return (T)GetValue ();
		}
	}

	public abstract class ObjectDataEditor : MonoBehaviour
	{
		[SerializeField] protected StringParameter id = new StringParameter ("id", string.Empty);
		[SerializeField] protected StringParameter prefab = new StringParameter ("prefab", string.Empty);

		public abstract ObjectData GenerateData ();
	}
}
