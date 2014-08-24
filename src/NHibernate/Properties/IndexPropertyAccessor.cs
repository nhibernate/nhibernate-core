using System;
using System.Collections;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Properties
{
	/// <summary> Represents a "back-reference" to the index of a collection. </summary>
	[Serializable]
	public class IndexPropertyAccessor : IPropertyAccessor
	{
		private readonly string propertyName;
		private readonly string entityName;

		/// <summary> Constructs a new instance of IndexPropertyAccessor. </summary>
		/// <param name="collectionRole">The collection role which this back ref references. </param>
		/// <param name="entityName">The owner entity name.</param>
		public IndexPropertyAccessor(string collectionRole, string entityName)
		{
			propertyName = collectionRole.Substring(entityName.Length + 1);
			this.entityName = entityName;
		}

		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new IndexGetter(this);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new IndexSetter();
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return false; }
		}

		#endregion

		/// <summary> The Setter implementation for index backrefs.</summary>
		[Serializable]
		public sealed class IndexSetter : ISetter
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

		/// <summary> The Getter implementation for index backrefs.</summary>
		[Serializable]
		public class IndexGetter : IGetter
		{
			private readonly IndexPropertyAccessor encloser;

			public IndexGetter(IndexPropertyAccessor encloser)
			{
				this.encloser = encloser;
			}

			#region IGetter Members

			public object Get(object target)
			{
				return BackrefPropertyAccessor.Unknown;
			}

			public System.Type ReturnType
			{
				get { return typeof (object); }
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
					return BackrefPropertyAccessor.Unknown;
				}
				else
				{
					return session.PersistenceContext.GetIndexInOwner(encloser.entityName, encloser.propertyName, owner, mergeMap);
				}
			}

			#endregion
		}
	}
}
