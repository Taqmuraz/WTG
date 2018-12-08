using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RPG_System
{
	[Serializable]
	public class Container<T>
	{
		public delegate void ContainerDo (T t);
		public delegate bool ContainerCheck (T t);
		protected List<T> elements { get; private set; }

		public Container (params T[] _elements)
		{
			elements = _elements.ToList();
		}

		public virtual void DoForAll (ContainerDo toDo)
		{
			foreach (var e in elements) {
				toDo (e);
			}
		}
		public virtual TResoult GetOfType <TResoult> () where TResoult : T
		{
			return GetOfType<TResoult> ((T o) => true);
		}
		public virtual TResoult GetOfType <TResoult> (ContainerCheck check) where TResoult : T
		{
			return (TResoult)elements.FirstOrDefault ((T o) => o is TResoult && check (o));
		}
		public virtual TResoult[] GetAllOfType <TResoult> (ContainerCheck check) where TResoult : T
		{
			return Where((T o) => (o is TResoult && check (o))).Cast<TResoult> ().ToArray ();
		}
		public virtual T[] Where (ContainerCheck check)
		{
			return elements.Where ((T o) => check (o)).ToArray ();
		}
		public virtual void Add (T element)
		{
			elements.Add (element);
		}
		public virtual void Remove (T element)
		{
			elements.Remove (element);
		}
		public virtual bool Has (ContainerCheck check)
		{
			try {
				elements.First((T e) => check(e));
				return true;
			} catch {
				return false;
			}
		}
	}
}

