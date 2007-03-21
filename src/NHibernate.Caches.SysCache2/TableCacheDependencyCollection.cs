using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace NHibernateExtensions.Caches.SysCache
{
	/// <summary>
	/// Represents a collection of <see cref="TableCacheDependencyElement"/> objects.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface"),
	 ConfigurationCollection(typeof(TableCacheDependencyElement),
	 	CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class TableCacheDependencyCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Gets or sets the <see cref="TableCacheDependencyElement"/> at the specified index.
		/// </summary>
		public TableCacheDependencyElement this[int index]
		{
			get { return base.BaseGet(index) as TableCacheDependencyElement; }
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				base.BaseAdd(index, value);
			}
		}

		/// <summary>
		/// Gets the <see cref="TableCacheDependencyElement"/> with the specified name.
		/// </summary>
		public new TableCacheDependencyElement this[string name]
		{
			get { return base.BaseGet(name) as TableCacheDependencyElement; }
		}

		/// <summary>
		/// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection"></see>.
		/// </summary>
		/// <value></value>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType"></see> of this collection.</returns>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new TableCacheDependencyElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for.</param>
		/// <returns>
		/// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TableCacheDependencyElement) element).Name;
		}
	}
}