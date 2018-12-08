using System;

[Serializable]
public abstract class NullBool
{
	public static implicit operator bool (NullBool nb)
	{
		return nb != null;
	}
}

