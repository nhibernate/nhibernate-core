using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <include file='IType.cs.xmldoc' 
	///		path='//members[@type="IType"]/member[@name="T:IType"]/*'
	/// />
	public interface IType
	{
		// QUESTION:
		// How do we implement Serializable interface? Standard .NET pattern or other?
		// Can we mark this interface with the Serializable attribute?

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsAssociationType"]/*'
		/// /> 
		bool IsAssociationType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsPersistentCollectionType"]/*'
		/// /> 
		bool IsPersistentCollectionType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsComponentType"]/*'
		/// /> 
		bool IsComponentType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsEntityType"]/*'
		/// /> 
		bool IsEntityType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsObjectType"]/*'
		/// /> 
		bool IsObjectType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.SqlTypes"]/*'
		/// /> 
		SqlType[ ] SqlTypes( IMapping mapping );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.GetColumnSpan"]/*'
		/// /> 
		int GetColumnSpan( IMapping mapping );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.ReturnedClass"]/*'
		/// /> 
		System.Type ReturnedClass { get; }


		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Equals"]/*'
		/// /> 
		bool Equals( object x, object y );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.IsDirty"]/*'
		/// /> 
		bool IsDirty( object old, object current, ISessionImplementor session );


		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(IDataReader, String[], ISessionImplementor, Object)"]/*'
		/// /> 
		object NullSafeGet( IDataReader rs, string[ ] names, ISessionImplementor session, object owner );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(IDataReader, String, ISessionImplementor, Object)"]/*'
		/// /> 
		object NullSafeGet( IDataReader rs, string name, ISessionImplementor session, Object owner );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet"]/*'
		/// /> 
		void NullSafeSet( IDbCommand st, object value, int index, ISessionImplementor session );


		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.ToXML"]/*'
		/// /> 
		string ToXML( object value, ISessionFactoryImplementor factory );


		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.Name"]/*'
		/// /> 
		string Name { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.DeepCopy"]/*'
		/// /> 
		object DeepCopy( object val );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsMutable"]/*'
		/// /> 
		bool IsMutable { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name=M:IType.Disassemble"]/*'
		/// /> 
		object Disassemble( object value, ISessionImplementor session );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Assemble"]/*'
		/// /> 
		object Assemble( object cached, ISessionImplementor session, object owner );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.HasNiceEquals"]/*'
		/// /> 
		bool HasNiceEquals { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Hydrate"]/*'
		/// /> 
		object Hydrate( IDataReader rs, string[ ] names, ISessionImplementor session, object owner );

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.ResolveIdentifier"]/*'
		/// /> 
		object ResolveIdentifier( object value, ISessionImplementor session, object owner );

	}
}