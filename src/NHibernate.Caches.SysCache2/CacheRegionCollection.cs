using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace NHibernate.Caches.SysCache2
{
	/// <summary>
	/// 
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface"),
	 ConfigurationCollection(typeof(CacheRegionElement),
	 	AddItemName="cacheRegion", CollectionType=ConfigurationElementCollectionType.BasicMap)]
	public class CacheRegionCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Gets or sets the <see cref="CacheRegionElement"/> at the specified index.
		/// </summary>
		public CacheRegionElement this[int index]
		{
			get { return base.BaseGet(index) as CacheRegionElement; }
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
		/// Gets the <see cref="CacheRegionElement"/> with the specified name.
		/// </summary>
		public new CacheRegionElement this[string name]
		{
			get { return base.BaseGet(name) as CacheRegionElement; }
		}

		/// <summary>
		/// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection"></see>.
		/// </summary>
		/// <value></value>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType"></see> of this collection.</returns>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new CacheRegionElement();
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
			return ((CacheRegionElement) element).Name;
		}

		/// <summary>
		/// Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.
		/// </summary>
		/// <value></value>
		/// <returns>The name of the collection; otherwise, an empty string. The default is an empty string.</returns>
		protected override string ElementName
		{
			get { return "cacheRegion"; }
		}
	}
}