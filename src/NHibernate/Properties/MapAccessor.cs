using System.Collections;
using System.Reflection;
using NHibernate.Engine;
using System;
namespace NHibernate.Properties
{
	[Serializable]
	public class MapAccessor : IPropertyAccessor
	{
		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new MapGetter(propertyName);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new MapSetter(propertyName);
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return false; }
		}

		#endregion
		[Serializable]
		public sealed class MapSetter : ISetter
		{
			private readonly string name;

			internal MapSetter(string name)
			{
				this.name = name;
			}

			public MethodInfo Method
			{
				get { return null; }
			}

			public string PropertyName
			{
				get { return null; }
			}

			public void Set(object target, object value)
			{
				((IDictionary)target)[name] = value;
			}
		}
		[Serializable]
		public sealed class MapGetter : IGetter
		{
			private readonly string name;

			internal MapGetter(string name)
			{
				this.name = name;
			}

			public MethodInfo Method
			{
				get { return null; }
			}

			public object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
			{
				return Get(owner);
			}

			public string PropertyName
			{
				get { return null; }
			}

			public System.Type ReturnType
			{
				get { return typeof(object); }
			}

			public object Get(object target)
			{
				return ((IDictionary)target)[name];
			}
		}
	}
}
