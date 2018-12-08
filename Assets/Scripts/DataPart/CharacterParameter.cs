using System;
using RPG_Mechanic;
using RPG_System;

namespace RPG_Data
{
	[Serializable]
	public class CharacterParameterContainer : Container<CharacterParameter>
	{
		public CharacterParameterContainer (CharacterData root, params CharacterParameter[] prms) : base (prms)
		{
			root.OnInstalled.extendEvent += OnInstall;
		}
		public virtual void OnInstall (AliveOverlay root)
		{
			root.activeBehaviour.OnStart.extendEvent += OnStart;
			root.activeBehaviour.OnEverySecUpdate.extendEvent += EverySecUpdate;
			root.activeBehaviour.OnUpdate.extendEvent += Update;
			root.activeBehaviour.OnDestroyObj.extendEvent += OnDestroy;
		}
		public virtual void OnStart (int arg)
		{
			DoForAll ((CharacterParameter p) => p.OnAdd());
		}
		public virtual void EverySecUpdate (int arg)
		{
			DoForAll ((CharacterParameter p) => p.EverySecUpdate());
		}
		public virtual void Update (int arg)
		{
			DoForAll ((CharacterParameter p) => p.Update ());
		}
		public void OnDestroy (int arg)
		{
		}
		public override void Add (CharacterParameter element)
		{
			element.OnAdd ();
			base.Add (element);
		}
		public override void Remove (CharacterParameter element)
		{
			element.OnRemove ();
			base.Remove (element);
		}
	}

	[Serializable]
	public abstract class CharacterParameter : ICharacterParameter
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

