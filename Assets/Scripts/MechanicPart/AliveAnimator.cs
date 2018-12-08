using System;
using UnityEngine;
using RPG_System;

namespace RPG_Mechanic
{
	public class AliveAnimator : AliveComponent<Animator>
	{
		string controller;
		string avatar;
		Container<AnimatorControllerParameter> prms;

		protected static Container<IAnimatorParameter<AliveOverlay>> aliveAnimatorParameters =
			new Container<IAnimatorParameter<AliveOverlay>> (
				new AliveAnimatorParameter<float> ("MoveK", (AliveOverlay ao) => ao.GetComponent<AliveRigidbody> ().localVelocity.Flat ().magnitude)
			);

		public AliveAnimator (AliveOverlay basedOn, string _controller,
			string _avatar) : base (basedOn)
		{
			avatar = _avatar;
			controller = _controller;
			Initialize ();
		}
		public override void Initialize ()
		{
			if (string.IsNullOrEmpty(avatar) || string.IsNullOrEmpty(controller)) {
				return;
			}
			prms = new Container<AnimatorControllerParameter> (component.parameters);
			component.avatar = ResourcesManager.LoadSource<Avatar>(avatar);
			component.runtimeAnimatorController = ResourcesManager.LoadSource<RuntimeAnimatorController>(ResourcesManager.AnimatorControllersPath + controller);
			component.applyRootMotion = false;
			component.updateMode = AnimatorUpdateMode.Normal;
		}
		public virtual void SetParameter<T> (string name)
		{
			AliveAnimatorParameter<T> param = aliveAnimatorParameters.GetOfType<AliveAnimatorParameter<T>> ((IAnimatorParameter<AliveOverlay> p) => p.name == name);
			if (param) {
				param.Set (component, relativeObject);
			}
		}
		protected override void FixedUpdate (int arg)
		{
			aliveAnimatorParameters.DoForAll ((IAnimatorParameter<AliveOverlay> p) => {
				if (prms.Has((AnimatorControllerParameter acp) => acp.name == p.name)) {
					p.Set(component, relativeObject);
				}
			});
		}
	}
}
