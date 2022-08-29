using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Tuple.Component
{
	[Serializable]
	internal class DynamicComponentInstantiator : IInstantiator
	{
		public object Instantiate(object id) => Instantiate();

		public object Instantiate() => new DynamicComponent();

		public bool IsInstance(object obj) => obj is DynamicComponent ||
											  obj is IDictionary<string, object> ||
											  obj is IDictionary;
	}
}
