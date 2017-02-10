using System;
using System.Collections;
using System.Data.Common;
using System.Xml;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// The base implementation of the <see cref="IType"/> interface.
	/// Mapping of the built in Type hierarchy.
	/// </summary>
	[Serializable]
	public abstract class AbstractType : IType
	{
		/// <summary>
		/// Gets a value indicating if the <see cref="AbstractType"/> is an <see cref="IAssociationType"/>.
		/// </summary>
		/// <value>false - by default an <see cref="AbstractType"/> is not an <see cref="IAssociationType"/>.</value>
		public virtual bool IsAssociationType
		{
			get { return false; }
		}

		public virtual bool IsXMLElement
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="AbstractType"/> is a <see cref="CollectionType"/>.
		/// </summary>
		/// <value>false - by default an <see cref="AbstractType"/> is not a <see cref="CollectionType"/>.</value>
		public virtual bool IsCollectionType
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="AbstractType"/> is an <see cref="IAbstractComponentType"/>.
		/// </summary>
		/// <value>false - by default an <see cref="AbstractType"/> is not an <see cref="IAbstractComponentType"/>.</value>
		public virtual bool IsComponentType
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="AbstractType"/> is a <see cref="EntityType"/>.
		/// </summary>
		/// <value>false - by default an <see cref="AbstractType"/> is not a <see cref="EntityType"/>.</value>
		public virtual bool IsEntityType
		{
			get { return false; }
		}

		/// <summary>
		/// Disassembles the object into a cacheable representation.
		/// </summary>
		/// <param name="value">The value to disassemble.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> is not used by this method.</param>
		/// <param name="owner">optional parent entity object (needed for collections) </param>
		/// <returns>The disassembled, deep cloned state of the object</returns>
		/// <remarks>
		/// This method calls DeepCopy if the value is not null.
		/// </remarks>
		public virtual object Disassemble(object value, ISessionImplementor session, object owner)
		{
			if (value == null) 
				return null;

			return DeepCopy(value, session.EntityMode, session.Factory);
		}

		/// <summary>
		/// Reconstructs the object from its cached "disassembled" state.
		/// </summary>
		/// <param name="cached">The disassembled state from the cache</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> is not used by this method.</param>
		/// <param name="owner">The parent Entity object is not used by this method</param>
		/// <returns>The assembled object.</returns>
		/// <remarks>
		/// This method calls DeepCopy if the value is not null.
		/// </remarks>
		public virtual object Assemble(object cached, ISessionImplementor session, object owner)
		{
			if (cached == null)
				return null;

			return DeepCopy(cached, session.EntityMode, session.Factory);
		}

		public virtual void BeforeAssemble(object cached, ISessionImplementor session)
		{
		}

		/// <summary>
		/// Should the parent be considered dirty, given both the old and current 
		/// field or element value?
		/// </summary>
		/// <param name="old">The old value</param>
		/// <param name="current">The current value</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> is not used by this method.</param>
		/// <returns>true if the field is dirty</returns>
		/// <remarks>This method uses <c>IType.Equals(object, object)</c> to determine the value of IsDirty.</remarks>
		public virtual bool IsDirty(object old, object current, ISessionImplementor session)
		{
			return !IsSame(old, current, session.EntityMode);
		}

		/// <summary>
		/// Retrieves an instance of the mapped class, or the identifier of an entity 
		/// or collection from a <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the values.</param>
		/// <param name="names">
		/// The names of the columns in the <see cref="DbDataReader"/> that contain the 
		/// value to populate the IType with.
		/// </param>
		/// <param name="session">the session</param>
		/// <param name="owner">The parent Entity</param>
		/// <returns>An identifier or actual object mapped by this IType.</returns>
		/// <remarks>
		/// This method uses the <c>IType.NullSafeGet(DbDataReader, string[], ISessionImplementor, object)</c> method
		/// to Hydrate this <see cref="AbstractType"/>.
		/// </remarks>
		public virtual object Hydrate(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, names, session, owner);
		}

		/// <summary>
		/// Maps identifiers to Entities or Collections. 
		/// </summary>
		/// <param name="value">An identifier or value returned by <c>Hydrate()</c></param>
		/// <param name="session">The <see cref="ISessionImplementor"/> is not used by this method.</param>
		/// <param name="owner">The parent Entity is not used by this method.</param>
		/// <returns>The value.</returns>
		/// <remarks>
		/// There is nothing done in this method other than return the value parameter passed in.
		/// </remarks>
		public virtual object ResolveIdentifier(object value, ISessionImplementor session, object owner)
		{
			return value;
		}

		public virtual object SemiResolve(object value, ISessionImplementor session, object owner)
		{
			return value;
		}

		/// <summary>
		///Gets a value indicating if the implementation is an "object" type
		/// </summary>
		/// <value>false - by default an <see cref="AbstractType"/> is not a "object" type.</value>
		public virtual bool IsAnyType
		{
			get { return false; }
		}

		/// <summary>
		/// Says whether the value has been modified
		/// </summary>
		public virtual bool IsModified(
			object old,
			object current,
			bool[] checkable,
			ISessionImplementor session)
		{
			return IsDirty(old, current, session);
		}

		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj.GetType() == GetType());
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode();
		}

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.DeepCopy"]/*'
		/// /> 
		public abstract object DeepCopy(object val, EntityMode entityMode, ISessionFactoryImplementor factory);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.SqlTypes"]/*'
		/// /> 
		public abstract SqlType[] SqlTypes(IMapping mapping);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.GetColumnSpan"]/*'
		/// /> 
		public abstract int GetColumnSpan(IMapping mapping);

		public virtual object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache,
							  ForeignKeyDirection foreignKeyDirection)
		{
			bool include;
			if (IsAssociationType)
			{
				IAssociationType atype = (IAssociationType)this;
				include = atype.ForeignKeyDirection == foreignKeyDirection;
			}
			else
			{
				include = ForeignKeyDirection.ForeignKeyFromParent.Equals(foreignKeyDirection);
			}
			return include ? Replace(original, target, session, owner, copyCache) : target;
		}

		public virtual bool IsSame(object x, object y, EntityMode entityMode)
		{
			return IsEqual(x, y, entityMode);
		}

		public virtual bool IsEqual(object x, object y, EntityMode entityMode)
		{
			return EqualsHelper.Equals(x, y);
		}

		public virtual bool IsEqual(object x, object y, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			return IsEqual(x, y, entityMode);
		}

		public virtual int GetHashCode(object x, EntityMode entityMode)
		{
			return x.GetHashCode();
		}

		public virtual int GetHashCode(object x, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			return GetHashCode(x, entityMode);
		}

		public virtual int Compare(object x, object y, EntityMode? entityMode)
		{
			IComparable xComp = x as IComparable;
			IComparable yComp = y as IComparable;
			if (xComp != null)
				return xComp.CompareTo(y);
			if (yComp != null)
				return yComp.CompareTo(x);

			throw new HibernateException(
				string.Format("Can't compare {0} with {1}; you must implement System.IComparable", x.GetType(), y.GetType()));
		}

		public virtual IType GetSemiResolvedType(ISessionFactoryImplementor factory)
		{
			return this;
		}

		protected internal static void ReplaceNode(XmlNode container, XmlNode value)
		{
			if (container != value)
			{
				//not really necessary, I guess...
				XmlNode parent = container.ParentNode;
				parent.ReplaceChild(value, container);
			}
		}

		public abstract object Replace(object original, object current, ISessionImplementor session, object owner,
									   IDictionary copiedAlready);

		public abstract void SetToXMLNode(XmlNode node, object value, ISessionFactoryImplementor factory);
		public abstract object FromXMLNode(XmlNode xml, IMapping factory);
		public abstract bool[] ToColumnNullness(object value, IMapping mapping);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsMutable"]/*'
		/// /> 
		public abstract bool IsMutable { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.Name"]/*'
		/// /> 
		public abstract string Name { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(DbDataReader, string[], ISessionImplementor, object)"]/*'
		/// /> 
		public abstract object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(DbDataReader, string, ISessionImplementor, object)"]/*'
		/// /> 
		public abstract object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, Object owner);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet(settable)"]/*'
		/// /> 
		public abstract void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet"]/*'
		/// /> 
		public abstract void NullSafeSet(DbCommand st, object value, int index, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.ReturnedClass"]/*'
		/// /> 
		public abstract System.Type ReturnedClass { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.ToString"]/*'
		/// /> 
		public abstract string ToLoggableString(object value, ISessionFactoryImplementor factory);

		public abstract bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session);
	}
}