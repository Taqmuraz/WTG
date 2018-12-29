using System;
using UnityEngine;

namespace RPG_Mechanic
{
	[Serializable]
	public class CheckerValue<T>
	{
		public delegate T TValue ();
		public delegate bool TBool (T obj);

		TValue value;
		TBool bl;

		public CheckerValue(TValue _value, TBool _bl)
		{
			value = _value;
			bl = _bl;
		}

		public bool Check ()
		{
			return bl (value ());
		}
	}

	public interface IThing
	{
		bool IsThingFor (IOwner owner);
	}
	[Serializable]
	public class EverySecEvent<TD> : EveryCheckEvent<float, TD>
	{
		static CheckerValue<float> GetChecker ()
		{
			int last = 0;
			CheckerValue<float>.TValue v = () => Time.fixedTime;
			CheckerValue<float>.TBool b = (float t) => {
				if ((int)t != last) {
					last = (int)t;
					return true;
				}
				return false;
			};
			return new CheckerValue<float> (v, b);
		}

		public EverySecEvent () : base (GetChecker())
		{
			
		}
	}
	[Serializable]
	public class EveryCheckEvent<T, TD> : ExtendEvent<TD>
	{
		CheckerValue<T> checker;

		public EveryCheckEvent (CheckerValue<T> cv) : base ()
		{
			checker = cv;
		}
		public override void Invoke (TD arg)
		{
			if (checker.Check()) {
				base.Invoke (arg);
			}
		}
	}
	[Serializable]
	public class ExtendEvent<TD>
	{
		public delegate void ExtendDelegate (TD arg);

		public event ExtendDelegate extendEvent;

		public virtual void Invoke (TD arg)
		{
			if (extendEvent != null) {
				extendEvent (arg);
			}
		}
	}

	public class ExtendBehaviour : MonoBehaviour, IExistable
	{

		public IOwner currentOwner { get; protected set;}

		public Transform trans { get; private set; }
		public GameObject gameObj { get; private set; }

		protected virtual ExtendBehaviour Init (IOwner owner)
		{
			trans = transform;
			gameObj = gameObject;
			currentOwner = owner;
			return this;
		}

		public virtual bool IsThingFor (IOwner owner)
		{
			return currentOwner == owner;
		}

		public ExtendEvent<int> OnUpdate = new ExtendEvent<int>();
		public ExtendEvent<int> OnEverySecUpdate = new EverySecEvent<int>();
		public ExtendEvent<int> OnFixedUpdate = new ExtendEvent<int>();
		public ExtendEvent<int> OnStart = new ExtendEvent<int>();
		public ExtendEvent<int> OnAwake = new ExtendEvent<int>();
		public ExtendEvent<int> OnDestroyObj = new ExtendEvent<int>();
		public ExtendEvent<Collision> OnCollisionObjEnter = new ExtendEvent<Collision>();
		public ExtendEvent<Collision> OnCollisionObjStay = new ExtendEvent<Collision>();
		public ExtendEvent<Collision> OnCollisionObjExit = new ExtendEvent<Collision>();
		public ExtendEvent<Collider> OnTriggerObjEnter = new ExtendEvent<Collider>();
		public ExtendEvent<Collider> OnTriggerObjStay = new ExtendEvent<Collider>();
		public ExtendEvent<Collider> OnTriggerObjExit = new ExtendEvent<Collider>();
		public ExtendEvent<int> OnBecameObjVisible = new ExtendEvent<int>();
		public ExtendEvent<int> OnBecameObjInvisible = new ExtendEvent<int>();

		protected virtual void Awake ()
		{
			OnAwake.Invoke (0);
		}
		protected virtual void Start ()
		{
			OnStart.Invoke (0);
		}
		protected virtual void Update ()
		{
			OnUpdate.Invoke (0);
		}
		protected virtual void EverySecUpdate ()
		{
			OnEverySecUpdate.Invoke (0);
		}
		protected virtual void FixedUpdate ()
		{
			OnFixedUpdate.Invoke (0);
			EverySecUpdate ();
		}
		protected virtual void OnDestroy ()
		{
			OnDestroyObj.Invoke (0);
		}
		protected virtual void OnCollisionEnter (Collision other)
		{
			OnCollisionObjEnter.Invoke (other);
		}
		protected virtual void OnCollisionStay (Collision other)
		{
			OnCollisionObjStay.Invoke (other);
		}
		protected virtual void OnCollisionExit (Collision other)
		{
			OnCollisionObjExit.Invoke (other);
		}
		protected virtual void OnTriggerEnter (Collider other)
		{
			OnTriggerObjEnter.Invoke (other);
		}
		protected virtual void OnTriggerStay (Collider other)
		{
			OnTriggerObjStay.Invoke (other);
		}
		protected virtual void OnTriggerExit (Collider other)
		{
			OnTriggerObjExit.Invoke (other);
		}
		protected virtual void OnBecameVisible ()
		{
			OnBecameObjVisible.Invoke (0);
		}
		protected virtual void OnBecameInvisible ()
		{
			OnBecameObjInvisible.Invoke (0);
		}

		public static T GetDefault<T> (OverlayBehaviour<T> _owner) where T : ExtendBehaviour
		{
			ExtendBehaviour toReturn = (ExtendBehaviour)AddDefaultComponent (_owner.GetObject (), typeof(T));
			toReturn.Init (_owner);
			return (T)toReturn;
		}
		public static Component AddDefaultComponent (GameObject basedOn, Type type)
		{
			Component toReturn = basedOn.GetComponent(type);
			if (!toReturn) {
				toReturn = basedOn.GetComponentInChildren (type);
			}
			if (!toReturn) {
				toReturn = basedOn.AddComponent (type);
			}
			return toReturn;
		}
	}
}

