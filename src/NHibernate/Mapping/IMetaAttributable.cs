using System.Collections.Generic;

namespace NHibernate.Mapping
{
	/// <summary> Common interface for things that can handle meta attributes. </summary>
	public interface IMetaAttributable
	{
		/// <summary>
		/// Meta-Attribute collection.
		/// </summary>
		IDictionary<string, MetaAttribute> MetaAttributes { get;set;}

		/// <summary>
		/// Retrieve the <see cref="MetaAttribute"/>
		/// </summary>
		/// <param name="attributeName">The attribute name</param>
		/// <returns>The <see cref="MetaAttribute"/> if exists; null otherwise</returns>
		MetaAttribute GetMetaAttribute(string attributeName);
	}
}
