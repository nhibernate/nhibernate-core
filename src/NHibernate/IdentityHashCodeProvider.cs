using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace NHibernate
{
	[Serializable]
	public class IdentityHashCodeProvider : IHashCodeProvider
	{
		public int GetHashCode(object obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}
	}
}