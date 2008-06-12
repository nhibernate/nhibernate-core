using System;
using System.Collections;
using System.Data;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="Array"/> collection
	/// to the database.
	/// </summary>
	[Serializable]
	public class ArrayType : CollectionType
	{
		private readonly System.Type elementClass;
		private readonly System.Type arrayClass;

		/// <summary>
		/// Initializes a new instance of a <see cref="ArrayType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="elementClass">The <see cref="System.Type"/> of the element contained in the array.</param>
		/// <param name="isEmbeddedInXML"></param>
		/// <remarks>
		/// This creates a bag that is non-generic.
		/// </remarks>
		public ArrayType(string role, string propertyRef, System.Type elementClass, bool isEmbeddedInXML)
			: base(role, propertyRef, isEmbeddedInXML)
		{
			this.elementClass = elementClass;
			arrayClass = Array.CreateInstance(elementClass, 0).GetType();
		}

		/// <summary>
		/// The <see cref="System.Array"/> for the element.
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return arrayClass; }
		}

		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return new PersistentArrayHolder(session, persister);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		public override void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session)
		{
			base.NullSafeSet(st, session.PersistenceContext.GetCollectionHolder(value), index, session);
		}

		public override IEnumerable GetElementsIterator(object collection)
		{
			return (Array)collection;
		}

		/// <summary>
		/// Wraps a <see cref="System.Array"/> in a <see cref="PersistentArrayHolder"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="array">The unwrapped array.</param>
		/// <returns>
		/// An <see cref="PersistentArrayHolder"/> that wraps the non NHibernate <see cref="System.Array"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object array)
		{
			return new PersistentArrayHolder(session, array);
		}

		/// <summary></summary>
		public override bool IsArrayType
		{
			get { return true; }
		}

		// Not ported - ToString( object value, ISessionFactoryImplementor factory )
		// - PesistentCollectionType implementation is able to handle arrays too in .NET

		public override object InstantiateResult(object original)
		{
			return Array.CreateInstance(elementClass, ((Array) original).Length);
		}

		public override object Instantiate(int anticipatedSize)
		{
			throw new NotSupportedException("ArrayType.Instantiate()");
		}

		public override bool HasHolder(EntityMode entityMode)
		{
			return true;
		}

		public override object IndexOf(object collection, object element)
		{
			Array array = (Array) collection;
			int length = array.Length;
			for (int i = 0; i < length; i++)
			{
				//TODO: proxies!
				if (array.GetValue(i) == element)
					return i;
			}
			return null;
		}

		protected internal override bool InitializeImmediately(EntityMode entityMode)
		{
			return true;
		}

		public override object ReplaceElements(object original, object target, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			Array org = (Array) original;
			Array result = (Array)target;

			int length = org.Length;
			if (length != result.Length)
			{
				//note: this affects the return value!
				result = (Array) InstantiateResult(original);
			}

			IType elemType = GetElementType(session.Factory);
			for (int i = 0; i < length; i++)
			{
				result.SetValue(elemType.Replace(org.GetValue(i), null, session, owner, copyCache), i);
			}

			return result;
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			if (value == null)
			{
				return "null";
			}
			Array array = (Array) value;
			int length = array.Length;
			IList list = new ArrayList(length);
			IType elemType = GetElementType(factory);
			for (int i = 0; i < length; i++)
			{
				list.Add(elemType.ToLoggableString(array.GetValue(i), factory));
			}
			return CollectionPrinter.ToString(list);
		}
	}
}