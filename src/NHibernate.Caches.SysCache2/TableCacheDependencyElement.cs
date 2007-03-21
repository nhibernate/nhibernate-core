using System;
using System.Configuration;

namespace NHibernate.Caches.SysCache2
{
	/// <summary>
	/// Configures a table monitor cache dependency for an Nhibernate cache region.
	/// </summary>
	public class TableCacheDependencyElement : ConfigurationElement
	{
		/// <summary>Holds the configuration property definitions</summary>
		private static readonly ConfigurationPropertyCollection _properties;

		/// <summary>
		/// Initializes the <see cref="CacheRegionElement"/> class.
		/// </summary>
		static TableCacheDependencyElement()
		{
			//building the properties collection and overriding the properties property apparently
			//increases performace considerably
			_properties = new ConfigurationPropertyCollection();

			ConfigurationProperty nameProperty = new ConfigurationProperty("name", typeof(string), String.Empty,
			                                                               ConfigurationPropertyOptions.IsKey);

			_properties.Add(nameProperty);

			ConfigurationProperty entryProperty = new ConfigurationProperty("databaseEntryName",
			                                                                typeof(string), null,
			                                                                ConfigurationPropertyOptions.IsRequired);

			_properties.Add(entryProperty);

			ConfigurationProperty tableNameProperty = new ConfigurationProperty("tableName",
			                                                                    typeof(string), null,
			                                                                    ConfigurationPropertyOptions.IsRequired);

			_properties.Add(tableNameProperty);
		}

		/// <summary>
		/// The unique name of the dependency 
		/// </summary>
		public string Name
		{
			get { return (string) base["name"]; }
		}

		/// <summary>
		/// The name of the <see cref="System.Web.Configuration.SqlCacheDependencyDatabase"/> that
		/// contains the connection information for the table monitor
		/// </summary>
		public string DatabaseEntryName
		{
			get { return (string) base["databaseEntryName"]; }
		}

		/// <summary>
		/// The table in the database to monitor
		/// </summary>
		public string TableName
		{
			get { return (string) base["tableName"]; }
		}

		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"></see> collection of properties for the element.</returns>
		protected override ConfigurationPropertyCollection Properties
		{
			get { return _properties; }
		}
	}
}