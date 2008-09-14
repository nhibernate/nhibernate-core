using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// Defines operations common to "compiled" mappings (ie. <c>SessionFactory</c>) and
	/// "uncompiled" mappings (ie <c>Configuration</c> that are used by implementors of <c>IType</c>
	/// </summary>
	public interface IMapping
	{
		IType GetIdentifierType(string className);

		string GetIdentifierPropertyName(string className);

		IType GetReferencedPropertyType(string className, string propertyName);

		bool HasNonIdentifierPropertyNamedId(string className);
	}
}