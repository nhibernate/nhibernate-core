using System;
using System.Collections.Generic;

namespace NHibernate.Tuple.Component
{
	[Serializable]
	internal class DynamicComponentInstantiator : IInstantiator
	{
		public object Instantiate(object id) => Instantiate();

		public object Instantiate() => new Dictionary<string, object>();

		public bool IsInstance(object obj) => obj is Dictionary<string, object>;
	}
}