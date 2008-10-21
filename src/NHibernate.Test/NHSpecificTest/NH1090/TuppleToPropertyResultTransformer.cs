using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NHibernate.Transform;

namespace NHibernate.Test.NHSpecificTest.NH1090
{
	public class TupleToPropertyResultTransformer : IResultTransformer
	{
		private System.Type result;
		private PropertyInfo[] properties;

		public TupleToPropertyResultTransformer(System.Type result, params string[] names)
		{
			this.result = result;
			List<PropertyInfo> props = new List<PropertyInfo>();
			foreach (string name in names)
			{
				props.Add(result.GetProperty(name));
			}
			properties = props.ToArray();
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			object instance = Activator.CreateInstance(result);
			for (int i = 0; i < tuple.Length; i++)
			{
				properties[i].SetValue(instance, tuple[i], null);
			}
			return instance;
		}

		public System.Collections.IList TransformList(IList collection)
		{
			return collection;
		}

		public override int GetHashCode()
		{
			return 1234;
		}

		public override bool Equals(object obj)
		{
			return true;
		}
	}

}
