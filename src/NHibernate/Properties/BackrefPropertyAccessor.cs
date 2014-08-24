using System;
using System.Collections;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Properties
{
	[Serializable]
	public struct UnknownBackrefProperty
	{
	}

	/// <summary> Represents a "back-reference" to the id of a collection owner. </summary>
	[Serializable]
	public class BackrefPropertyAccessor : IPropertyAccessor
	{
		public static readonly object Unknown = new UnknownBackrefProperty();
		private readonly string propertyName;
		private readonly string entityName;

		public BackrefPropertyAccessor(string collectionRole, string entityName)
		{
			propertyName = collectionRole.Substring(entityName.Length + 1);
			this.entityName = entityName;
		}

		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new BackrefGetter(this);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new BackrefSetter();
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return false; }
		}

		#endregion

		/// <summary> The Setter implementation for id backrefs.</summary>
		[Serializable]
		private class BackrefSetter : ISetter
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


		/// <summary> The Getter implementation for id backrefs.</summary>
		[Serializable]
		private class BackrefGetter : IGetter
		{
			private readonly BackrefPropertyAccessor encloser;

			public BackrefGetter(BackrefPropertyAccessor encloser)
			{
				this.encloser = encloser;
			}

			#region IGetter Members

			public object Get(object target)
			{
				return Unknown;
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
				if (session == null)
				{
					return Unknown;
				}
				else
				{
					return session.PersistenceContext.GetOwnerId(encloser.entityName, encloser.propertyName, owner, mergeMap);
				}
			}

			#endregion
		}
	}
}
