using System.Collections;
using System.Data.Common;
using System.Xml;
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

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.Name"]/*'
		/// /> 
		string Name { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.ReturnedClass"]/*'
		/// /> 
		System.Type ReturnedClass { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsMutable"]/*'
		/// /> 
		bool IsMutable { get; }

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="P:IType.IsAssociationType"]/*'
		/// /> 
		bool IsAssociationType { get; }

		bool IsXMLElement { get;}

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
		///		path='//members[@type="IType"]/member[@name="M:IType.IsDirty"]/*'
		/// /> 
		bool IsDirty(object old, object current, ISessionImplementor session);

		bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session);

		bool IsModified(object oldHydratedState, object currentState, bool[] checkable, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(DbDataReader, String[], ISessionImplementor, Object)"]/*'
		/// /> 
		object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(DbDataReader, String, ISessionImplementor, Object)"]/*'
		/// /> 
		object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet(settable)"]/*'
		/// /> 
		void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet"]/*'
		/// /> 
		void NullSafeSet(DbCommand st, object value, int index, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.ToString"]/*'
		/// /> 
		string ToLoggableString(object value, ISessionFactoryImplementor factory);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.DeepCopy"]/*'
		/// /> 
		object DeepCopy(object val, EntityMode entityMode, ISessionFactoryImplementor factory);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Hydrate"]/*'
		/// /> 
		object Hydrate(DbDataReader rs, string[] names, ISessionImplementor session, object owner);

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

		/// <summary> 
		/// Compare two instances of the class mapped by this type for persistence
		/// "equality" - equality of persistent state - taking a shortcut for
		/// entity references.
		/// </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <param name="entityMode"> </param>
		/// <returns> boolean </returns>
		bool IsSame(object x, object y, EntityMode entityMode);

		/// <summary> 
		/// Compare two instances of the class mapped by this type for persistence
		/// "equality" - equality of persistent state.
		/// </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <param name="entityMode"> </param>
		/// <returns> boolean </returns>
		bool IsEqual(object x, object y, EntityMode entityMode);

		/// <summary> 
		/// Compare two instances of the class mapped by this type for persistence
		/// "equality" - equality of persistent state.
		/// </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <param name="entityMode"> </param>
		/// <param name="factory"></param>
		/// <returns> boolean </returns>
		bool IsEqual(object x, object y, EntityMode entityMode, ISessionFactoryImplementor factory);

		/// <summary> Get a hashcode, consistent with persistence "equality"</summary>
		/// <param name="x"> </param>
		/// <param name="entityMode"> </param>
		int GetHashCode(object x, EntityMode entityMode);

		/// <summary> Get a hashcode, consistent with persistence "equality"</summary>
		/// <param name="x"> </param>
		/// <param name="entityMode"> </param>
		/// <param name="factory"> </param>
		int GetHashCode(object x, EntityMode entityMode, ISessionFactoryImplementor factory);

		/// <summary> compare two instances of the type</summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <param name="entityMode"> </param>
		int Compare(object x, object y, EntityMode? entityMode);

		/// <summary> Get the type of a semi-resolved value.</summary>
		IType GetSemiResolvedType(ISessionFactoryImplementor factory);

		/// <summary> A representation of the value to be embedded in an XML element. </summary>
		/// <param name="node"></param>
		/// <param name="value"> </param>
		/// <param name="factory"> </param>
		void SetToXMLNode(XmlNode node, object value, ISessionFactoryImplementor factory);

		/// <summary> Parse the XML representation of an instance.</summary>
		/// <param name="xml"> </param>
		/// <param name="factory"> </param>
		/// <returns> an instance of the type </returns>
		object FromXMLNode(XmlNode xml, IMapping factory);

		/// <summary> 
		/// Given an instance of the type, return an array of boolean, indicating
		/// which mapped columns would be null. 
		/// </summary>
		/// <param name="value">an instance of the type </param>
		/// <param name="mapping"></param>
		bool[] ToColumnNullness(object value, IMapping mapping);
	}
}