using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <include file='IType.cs.xmldoc' 
	///		path='//members[@type="IType"]/member[@name="T:IType"]/*'
	/// />
	public interface IType : ICacheAssembler
	{
		// QUESTION:
		// How do we implement Serializable interface? Standard .NET pattern or other?
		// Can we mark this interface with the Serializable attribute?

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsAssociationType"]/*'
		/// /> 
		bool IsAssociationType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsCollectionType"]/*'
		/// /> 
		bool IsCollectionType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsComponentType"]/*'
		/// /> 
		bool IsComponentType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsEntityType"]/*'
		/// /> 
		bool IsEntityType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsAnyType"]/*'
		/// /> 
		bool IsAnyType { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.SqlTypes"]/*'
		/// /> 
		SqlType[] SqlTypes(IMapping mapping);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.GetColumnSpan"]/*'
		/// /> 
		int GetColumnSpan(IMapping mapping);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.ReturnedClass"]/*'
		/// /> 
		System.Type ReturnedClass { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Equals"]/*'
		/// /> 
		bool Equals(object x, object y);

		/// <summary>
		/// Get a hashcode, consistent with persistence "equality"
		/// </summary>
		int GetHashCode(object x, ISessionFactoryImplementor factory);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.IsDirty"]/*'
		/// /> 
		bool IsDirty(object old, object current, ISessionImplementor session);

		bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session);

		bool IsModified(object oldHydratedState, object currentState, bool[] checkable, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(IDataReader, String[], ISessionImplementor, Object)"]/*'
		/// /> 
		object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(IDataReader, String, ISessionImplementor, Object)"]/*'
		/// /> 
		object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet(settable)"]/*'
		/// /> 
		void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet"]/*'
		/// /> 
		void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.ToString"]/*'
		/// /> 
		string ToLoggableString(object value, ISessionFactoryImplementor factory);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.FromString"]/*'
		/// /> 
		object FromString(string xml);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.Name"]/*'
		/// /> 
		string Name { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.DeepCopy"]/*'
		/// /> 
		object DeepCopy(object val);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsMutable"]/*'
		/// /> 
		bool IsMutable { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.HasNiceEquals"]/*'
		/// /> 
		bool HasNiceEquals { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Hydrate"]/*'
		/// /> 
		object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.ResolveIdentifier"]/*'
		/// /> 
		object ResolveIdentifier(object value, ISessionImplementor session, object owner);

		/// <summary>
		/// Given a hydrated, but unresolved value, return a value that may be used to
		/// reconstruct property-ref associations.
		/// </summary>
		object SemiResolve(object value, ISessionImplementor session, object owner);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Copy"]/*'
		/// /> 
		object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready);

		/// <summary>
		/// Determines whether the specified value is represented as <see langword="null" /> in the database.
		/// </summary>
		/// <param name="value">The value, may be <see langword="null" />.</param>
		/// <returns>
		/// <see langword="true" /> if the specified value is represented as <see langword="null" /> in the database;
		/// otherwise, <see langword="false" />.
		/// </returns>
		bool IsDatabaseNull(object value);

		/// <summary> 
		/// During merge, replace the existing (target) value in the entity we are merging to
		/// with a new (original) value from the detached entity we are merging. For immutable
		/// objects, or null values, it is safe to simply return the first parameter. For
		/// mutable objects, it is safe to return a copy of the first parameter. For objects
		/// with component values, it might make sense to recursively replace component values. 
		/// </summary>
		/// <param name="original">the value from the detached entity being merged </param>
		/// <param name="target">the value in the managed entity </param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <param name="copyCache"></param>
		/// <param name="foreignKeyDirection"></param>
		/// <returns> the value to be merged </returns>
		object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection);
	}
}