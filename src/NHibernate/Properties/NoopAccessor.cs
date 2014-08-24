using System;
using System.Collections;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Properties
{
	/// <summary> Used to declare properties not represented at the pojo level </summary>
	[Serializable]
	public class NoopAccessor : IPropertyAccessor
	{
		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new NoopGetter();
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new NoopSetter();
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return false; }
		}

		#endregion

		/// <summary> A Getter which will always return null. It should not be called anyway.</summary>
		[Serializable]
		private class NoopGetter : IGetter
		{
			#region IGetter Members

			public object Get(object target)
			{
				return null;
			}

			public System.Type ReturnType
			{
				get { return typeof(object); }
			}

			public string PropertyName
			{
				get { return null; }
			}

			public MethodInfo Method
			{
				get { return null; }
			}

			public object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
			{
				return null;
			}

			#endregion
		}

		/// <summary> A Setter which will just do nothing.</summary>
		[Serializable]
		private class NoopSetter : ISetter
		{
			#region ISetter Members

			public void Set(object target, object value)
			{
			}

			public string PropertyName
			{
				get { return null; }
			}

			public MethodInfo Method
			{
				get { return null; }
			}

			#endregion
		}
	}
}
