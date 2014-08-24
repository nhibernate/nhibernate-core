using System;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Configuration parsed values for a collection-cache XML node.
	/// </summary>
	public class CollectionCacheConfiguration
	{
		internal CollectionCacheConfiguration(XPathNavigator collectionCacheElement)
		{
			Parse(collectionCacheElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionCacheConfiguration"/> class.
		/// </summary>
		/// <param name="collection">The cache role.</param>
		/// <param name="usage">Cache strategy.</param>
		/// <exception cref="ArgumentException">When <paramref name="collection"/> is null or empty.</exception>
		public CollectionCacheConfiguration(string collection, EntityCacheUsage usage)
		{
			if (String.IsNullOrEmpty(collection))
				throw new ArgumentException("collection is null or empty.", "collection");
			this.collection = collection;
			this.usage = usage;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionCacheConfiguration"/> class.
		/// </summary>
		/// <param name="collection">The cache role.</param>
		/// <param name="usage">Cache strategy.</param>
		/// <param name="region">The cache region.</param>
		/// <exception cref="ArgumentException">When <paramref name="collection"/> is null or empty.</exception>
		public CollectionCacheConfiguration(string collection, EntityCacheUsage usage, string region)
			:this(collection,usage)
		{
			this.region = region;
		}

		private void Parse(XPathNavigator collectionCacheElement)
		{
			if (collectionCacheElement.MoveToFirstAttribute())
			{
				do
				{
					switch (collectionCacheElement.Name)
					{
						case "collection":
							if (collectionCacheElement.Value.Trim().Length == 0)
								throw new HibernateConfigException("Invalid collection-cache element; the attribute <collection> must be assigned with no empty value");
							collection = collectionCacheElement.Value;
							break;
						case "usage":
							usage = EntityCacheUsageParser.Parse(collectionCacheElement.Value);
							break;
						case "region":
							region = collectionCacheElement.Value;
							break;
					}
				}
				while (collectionCacheElement.MoveToNextAttribute());
			}
		}

		private string collection;
		/// <summary>
		/// The role.
		/// </summary>
		public string Collection
		{
			get { return collection; }
		}

		private string region;
		/// <summary>
		/// The cache region.
		/// </summary>
		/// <remarks>If null or empty the <see cref="CollectionCacheConfiguration.Collection"/> is used during configuration.</remarks>
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
	}
}
