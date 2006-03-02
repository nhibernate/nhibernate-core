using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

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

		/// <summary>
		/// Gets a value indicating if the <see cref="AbstractType"/> is a <see cref="PersistentCollectionType"/>.
		/// </summary>
		/// <value>false - by default an <see cref="AbstractType"/> is not a <see cref="PersistentCollectionType"/>.</value>
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
		/// <returns>The disassembled, deep cloned state of the object</returns>
		/// <remarks>
		/// This method calls DeepCopy if the value is not null.
		/// </remarks>
		public virtual object Disassemble( object value, ISessionImplementor session )
		{
			if( value == null )
			{
				return null;
			}
			else
			{
				return DeepCopy( value );
			}
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
		public virtual object Assemble( object cached, ISessionImplementor session, object owner )
		{
			if( cached == null )
			{
				return null;
			}
			else
			{
				return DeepCopy( cached );
			}
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
		public virtual bool IsDirty( object old, object current, ISessionImplementor session )
		{
			return !Equals( old, current );
		}

		/// <summary>
		/// Retrives an instance of the mapped class, or the identifier of an entity 
		/// or collection from a <see cref="IDataReader"/>.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> that contains the values.</param>
		/// <param name="names">
		/// The names of the columns in the <see cref="IDataReader"/> that contain the 
		/// value to populate the IType with.
		/// </param>
		/// <param name="session">the session</param>
		/// <param name="owner">The parent Entity</param>
		/// <returns>An identifier or actual object mapped by this IType.</returns>
		/// <remarks>
		/// This method uses the <c>IType.NullSafeGet(IDataReader, string[], ISessionImplementor, object)</c> method
		/// to Hydrate this <see cref="AbstractType"/>.
		/// </remarks>
		public virtual object Hydrate( IDataReader rs, string[ ] names, ISessionImplementor session, object owner )
		{
			return NullSafeGet( rs, names, session, owner );
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
		public virtual object ResolveIdentifier( object value, ISessionImplementor session, object owner )
		{
			return value;
		}

		/// <summary>
		///Gets a value indicating if the implementation is an "object" type
		/// </summary>
		/// <value>false - by default an <see cref="AbstractType"/> is not a "object" type.</value>
		public virtual bool IsObjectType
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
			ISessionImplementor session )
		{
			return IsDirty( old, current, session );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="original"></param>
		/// <param name="current"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <param name="copiedAlready"></param>
		/// <returns></returns>
		public virtual object Copy( object original, object current, ISessionImplementor session, object owner, IDictionary copiedAlready )
		{
			if ( original == null )
			{
				return null;
			}
			return Assemble( Disassemble( original, session ), session, owner );
		}

		public override bool Equals( object obj )
		{
			return obj == this || ( obj != null && obj.GetType() == GetType() );
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode();
		}

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.DeepCopy"]/*'
		/// /> 
		public abstract object DeepCopy( object val );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.SqlTypes"]/*'
		/// /> 
		public abstract SqlType[ ] SqlTypes( IMapping mapping );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.GetColumnSpan"]/*'
		/// /> 
		public abstract int GetColumnSpan( IMapping mapping );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Equals"]/*'
		/// /> 
		new public abstract bool Equals( object x, object y ); //We need "new" because object.Equal is not marked as virtual. Is it correct? Or because this is *abstract* so we're not really overriding it?

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsMutable"]/*'
		/// /> 
		public abstract bool IsMutable { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.Name"]/*'
		/// /> 
		public abstract string Name { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.HasNiceEquals"]/*'
		/// /> 
		public abstract bool HasNiceEquals { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(IDataReader, string[], ISessionImplementor, object)"]/*'
		/// /> 
		public abstract object NullSafeGet( IDataReader rs, string[ ] names, ISessionImplementor session, object owner );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(IDataReader, string, ISessionImplementor, object)"]/*'
		/// /> 
		public abstract object NullSafeGet( IDataReader rs, string name, ISessionImplementor session, Object owner );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet"]/*'
		/// /> 
		public abstract void NullSafeSet( IDbCommand st, object value, int index, ISessionImplementor session );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.ReturnedClass"]/*'
		/// /> 
		public abstract System.Type ReturnedClass { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.ToString"]/*'
		/// /> 
		public abstract string ToLoggableString( object value, ISessionFactoryImplementor factory );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.FromString"]/*'
		/// /> 
		public abstract object FromString( string xml );

		public abstract bool IsDirty( object old, object current, bool[] checkable, ISessionImplementor session );
	}
}