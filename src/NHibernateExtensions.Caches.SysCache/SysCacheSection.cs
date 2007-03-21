using System;
using System.Configuration;

namespace NHibernateExtensions.Caches.SysCache {
	/// <summary>
	/// Provides Configuration system support for the SysCache configuration section
	/// </summary>
	/// <remarks>
	///		<para>Section name must be 'sysCache'</para>
	/// </remarks>
	public class SysCacheSection : ConfigurationSection {
		
		#region Constants
		/// <summary>Confiuration section name</summary>
		private const string SectionName = "sysCache";
		#endregion
		
		#region Private Fields
		/// <summary>Holds the configuration property definitions</summary>
		private static readonly ConfigurationPropertyCollection _properties;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Initializes the <see cref="CacheRegionElement"/> class.
		/// </summary>
		static SysCacheSection(){
			//building the properties collection and overriding the properties property apparently
			//increases performace considerably
			_properties = new ConfigurationPropertyCollection();
			
			ConfigurationProperty regionsProperty = new ConfigurationProperty("", typeof(CacheRegionCollection), 
				null, ConfigurationPropertyOptions.IsDefaultCollection);
				
			_properties.Add(regionsProperty);
							
		}
		#endregion
	
		
		
		#region Public Properties
		/// <summary>
		/// Gets the cache region elements
		/// </summary>
		public CacheRegionCollection CacheRegions{
			get{
				return (CacheRegionCollection)base[string.Empty];
			}
		}
		
		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"></see> collection of properties for the element.</returns>
		protected override ConfigurationPropertyCollection Properties {
			get {
				return _properties;
			}
		}		
		#endregion
		
		#region Public Methods
		/// <summary>
		/// Gets the <see cref="SysCacheSection"/> from the configuration
		/// </summary>
		/// <returns>The configured <see cref="SysCacheSection"/></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static SysCacheSection GetSection() {
			return ConfigurationManager.GetSection(SysCacheSection.SectionName) as SysCacheSection;
		}
		#endregion
	}
}
