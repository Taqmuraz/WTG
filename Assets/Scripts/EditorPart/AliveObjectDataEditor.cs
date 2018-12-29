using UnityEngine;
using System.Collections;
using System.Linq;
using RPG_Data;

namespace RPG_Editor
{
	public class AliveObjectDataEditor : ObjectDataEditor
	{
		[SerializeField] protected StringParameter behaviour = new StringParameter ("behaviour", string.Empty);
		[SerializeField] StringParameter[] components = new StringParameter[0];

		public override ObjectData GenerateData ()
		{
			return new AliveWorldData (id.GetValue<string>(), prefab.GetValue<string>(), behaviour.GetValue<string>(), components.Select ((StringParameter ef) => ef.GetValue<string> ()).ToArray ());
		}
	}
}
