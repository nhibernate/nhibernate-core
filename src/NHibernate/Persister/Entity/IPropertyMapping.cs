using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Abstraction of all mappings that define properties: entities, collection elements.
	/// </summary>
	public interface IPropertyMapping
	{
		// TODO: It would be really, really nice to use this to also model components!

		/// <summary>
		/// Get the type of the thing containing the properties
		/// </summary>
		IType Type { get; }

		/// <summary>
		/// Given a component path expression, get the type of the property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		IType ToType(string propertyName);

		/// <summary>
		/// Given a component path expression, get the type of the property. 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="type"></param>
		/// <returns>true if a type was found, false if not</returns>
		bool TryToType(string propertyName, out IType type);

		/// <summary>
		/// Given a query alias and a property path, return the qualified column name
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		string[] ToColumns(string alias, string propertyName);

		/// <summary> Given a property path, return the corresponding column name(s).</summary>
		string[] ToColumns(string propertyName);
	}
}