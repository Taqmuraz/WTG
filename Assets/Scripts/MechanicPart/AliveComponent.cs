using System;
using UnityEngine;
using RPG_System;

namespace RPG_Mechanic
{
	public abstract class AliveComponent<T> : IAliveComponent, IExistable, IPhysicsExistable where T : Component
	{
		public T component { get; private set; }

		public AliveOverlay relativeObject { get; private set; }

		public Transform trans { get; private set; }
		public GameObject gameObj { get; private set; }
		public Rigidbody body { get; private set; }
		public Collider coll { get; private set; }

		public AliveComponent (AliveOverlay basedOn)
		{
			relativeObject = basedOn;
			trans = basedOn.activeBehaviour.trans;
			gameObj = basedOn.activeBehaviour.gameObj;
			body = basedOn.activeBehaviour.body;
			coll = basedOn.activeBehaviour.coll;

			component = (T)ExtendBehaviour.AddDefaultComponent (basedOn.activeBehaviour.gameObj, typeof(T));

			relativeObject.activeBehaviour.OnUpdate.extendEvent += Update;
			relativeObject.activeBehaviour.OnFixedUpdate.extendEvent += FixedUpdate;
			relativeObject.activeBehaviour.OnCollisionObjEnter.extendEvent += OnCollisionEnter;

			Debug.Log ("Component initialized : " + component.ToString());

			Initialize ();
		}
		public abstract void Initialize ();

		protected virtual void Update (int arg)
		{
		}
		protected virtual void FixedUpdate (int arg) 
		{
		}
		protected virtual void OnCollisionEnter (Collision cl) 
		{
		}

		public static Type GetComponentType ()
		{
			return typeof(T);
		}
	}
}

