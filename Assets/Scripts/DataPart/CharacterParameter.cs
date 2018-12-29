using System;
using RPG_Mechanic;
using RPG_System;
using UnityEngine;

namespace RPG_Data
{

	[Serializable]
	public abstract class CharacterParameter : NullBool, ICharacterParameter
	{
		protected string characterID { get; private set; }
		CharacterData _root;
		protected CharacterData root
		{
			get 
			{
				return _root = _root ? _root : (CharacterData)GameData.GetActiveByID (characterID);
			}
		}

		public CharacterParameter (string id)
		{
			characterID = id;
		}

		public CharacterData GetRoot ()
		{
			return root;
		}
		public virtual void OnAdd ()
		{
		}
		public virtual void OnRemove ()
		{
		}
		public virtual void EverySecUpdate ()
		{
		}
		public virtual void Update ()
		{
		}
	}

	public interface ICharacterParameter
	{
		CharacterData GetRoot ();
		void OnAdd();
		void OnRemove();
		void EverySecUpdate();
		void Update();
	}
}

