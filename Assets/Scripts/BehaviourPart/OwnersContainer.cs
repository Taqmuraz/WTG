using System;
using System.Collections.Generic;
using System.Linq;
using RPG_System;

namespace RPG_Mechanic
{
	public class OwnersContainer : Container<IOwner>
	{
		public OwnersContainer (params IOwner[] _elements) : base (_elements)
		{
		}
	}
}

