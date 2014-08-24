using System;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Values for class-cache include.
	/// </summary>
	/// <remarks>Not implemented in Cache.</remarks>
	public enum ClassCacheInclude
	{
		// TODO: Implement ClassCacheInclude (remove de remarks from this enum and the property)
		/// <summary>Xml value: all</summary>
		All,
		/// <summary>Xml value: non-lazy</summary>
		NonLazy
	}

	/// <summary>
	/// Configuration parsed values for a class-cache XML node.
	/// </summary>
	public class ClassCacheConfiguration
	{
		internal ClassCacheConfiguration(XPathNavigator classCacheElement)
		{
			Parse(classCacheElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClassCacheConfiguration"/> class.
		/// </summary>
		/// <param name="clazz">The class full name.</param>
		/// <param name="usage">Cache strategy.</param>
		/// <exception cref="ArgumentException">When <paramref name="clazz"/> is null or empty.</exception>
		public ClassCacheConfiguration(string clazz, EntityCacheUsage usage)
		{
			if (string.IsNullOrEmpty(clazz))
				throw new ArgumentException("clazz is null or empty.", "clazz");
			this.clazz = clazz;
			this.usage = usage;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClassCacheConfiguration"/> class.
		/// </summary>
		/// <param name="clazz">The class full name.</param>
		/// <param name="usage">Cache strategy.</param>
		/// <param name="include">Values for class-cache include.</param>
		/// <exception cref="ArgumentException">When <paramref name="clazz"/> is null or empty.</exception>
		public ClassCacheConfiguration(string clazz, EntityCacheUsage usage, ClassCacheInclude include)
			: this(clazz, usage)
		{
			this.include = include;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClassCacheConfiguration"/> class.
		/// </summary>
		/// <param name="clazz">The class full name.</param>
		/// <param name="usage">Cache strategy.</param>
		/// <param name="region">The cache region.</param>
		/// <exception cref="ArgumentException">When <paramref name="clazz"/> is null or empty.</exception>
		public ClassCacheConfiguration(string clazz, EntityCacheUsage usage, string region)
			: this(clazz, usage)
		{
			this.region = region;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClassCacheConfiguration"/> class.
		/// </summary>
		/// <param name="clazz">The class full name.</param>
		/// <param name="usage">Cache strategy.</param>
		/// <param name="include">Values for class-cache include.</param>
		/// <param name="region">The cache region.</param>
		/// <exception cref="ArgumentException">When <paramref name="clazz"/> is null or empty.</exception>
		public ClassCacheConfiguration(string clazz, EntityCacheUsage usage, ClassCacheInclude include, string region)
			: this(clazz, usage, include)
		{
			this.region = region;
		}

		private void Parse(XPathNavigator classCacheElement)
		{
			if (classCacheElement.MoveToFirstAttribute())
			{
				do
				{
					switch (classCacheElement.Name)
					{
						case "class":
							if (classCacheElement.Value.Trim().Length == 0)
								throw new HibernateConfigException("Invalid class-cache element; the attribute <class> must be assigned with no empty value");
							clazz = classCacheElement.Value;
							break;
						case "usage":
							usage = EntityCacheUsageParser.Parse(classCacheElement.Value);
							break;
						case "region":
							region = classCacheElement.Value;
							break;
						case "include":
							include = CfgXmlHelper.ClassCacheIncludeConvertFrom(classCacheElement.Value);
							break;
					}
				}
				while (classCacheElement.MoveToNextAttribute());
			}			
		}

		private string clazz;
		/// <summary>
		/// The class full name.
		/// </summary>
		public string Class
		{
			get { return clazz; }
		}

		private string region;
		/// <summary>
		/// The cache region.
		/// </summary>
		/// <remarks>If null or empty the <see cref="ClassCacheConfiguration.Class"/> is used during configuration.</remarks>
		public string Region
		{
			get { return region; }
		}


		private EntityCacheUsage usage;
		/// <summary>
		/// Cache strategy.
		/// </summary>
		public EntityCacheUsage Usage
		{
			get { return usage; }
		}

		private ClassCacheInclude include = ClassCacheInclude.All;
		/// <summary>
		/// class-cache include.
		/// </summary>
		/// <remarks>
		/// Not implemented in Cache.
		/// Default value <see cref="ClassCacheInclude.All"/>.
		/// </remarks>
		public ClassCacheInclude Include
		{
			get { return include; }
		}

	}
}
