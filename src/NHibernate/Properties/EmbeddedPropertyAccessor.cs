using System;
using System.Collections;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Properties
{
	[Serializable]
	public class EmbeddedPropertyAccessor : IPropertyAccessor
	{
		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new EmbeddedGetter(theClass);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new EmbeddedSetter(theClass);
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return false; }
		}

		#endregion

		[Serializable]
		public sealed class EmbeddedGetter : IGetter
		{
			private readonly System.Type clazz;

			internal EmbeddedGetter(System.Type clazz)
			{
				this.clazz = clazz;
			}

			#region IGetter Members

			public object Get(object target)
			{
				return target;
			}

			public System.Type ReturnType
			{
				get { return clazz; }
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
				return Get(owner);
			}

			#endregion

			public override string ToString()
			{
				return string.Format("EmbeddedGetter({0})", clazz.FullName);
			}
		}

		[Serializable]
		public sealed class EmbeddedSetter : ISetter
		{
			private readonly System.Type clazz;

			internal EmbeddedSetter(System.Type clazz)
			{
				this.clazz = clazz;
			}

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

			public override string ToString()
			{
				return string.Format("EmbeddedSetter({0})", clazz.FullName);
			}
		}

	}
}
