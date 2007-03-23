using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace NHibernate.Caches.SysCache2
{
	/// <summary>
	/// Provides Configuration system support for the SysCache configuration section
	/// </summary>
	/// <remarks>
	///		<para>Section name must be 'sysCache'</para>
	/// </remarks>
	public class SysCacheSection : ConfigurationSection
	{
		/// <summary>Confiuration section name</summary>
		private const string SectionName = "syscache2";

		/// <summary>Holds the configuration property definitions</summary>
		private static readonly ConfigurationPropertyCollection _properties;

		/// <summary>
		/// Initializes the <see cref="CacheRegionElement"/> class.
		/// </summary>
		static SysCacheSection()
		{
			//building the properties collection and overriding the properties property apparently
			//increases performace considerably
			_properties = new ConfigurationPropertyCollection();

			ConfigurationProperty regionsProperty = new ConfigurationProperty("", typeof(CacheRegionCollection),
			                                                                  null,
			                                                                  ConfigurationPropertyOptions.IsDefaultCollection);

			_properties.Add(regionsProperty);
		}

		/// <summary>
		/// Gets the cache region elements
		/// </summary>
		public CacheRegionCollection CacheRegions
		{
			get { return (CacheRegionCollection) base[string.Empty]; }
		}

		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"></see> collection of properties for the element.</returns>
		protected override ConfigurationPropertyCollection Properties
		{
			get { return _properties; }
		}

		/// <summary>
		/// Gets the <see cref="SysCacheSection"/> from the configuration
		/// </summary>
		/// <returns>The configured <see cref="SysCacheSection"/></returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static SysCacheSection GetSection()
		{
			return ConfigurationManager.GetSection(SectionName) as SysCacheSection;
		}
	}
}