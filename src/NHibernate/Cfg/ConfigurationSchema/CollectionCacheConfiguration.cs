using System;
using System.Xml;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	public class CollectionCacheConfiguration
	{
		public CollectionCacheConfiguration(XPathNavigator collectionCacheElement)
		{
			Parse(collectionCacheElement);
		}

		public CollectionCacheConfiguration(string collection, ClassCacheUsage usage)
		{
			if (String.IsNullOrEmpty(collection))
				throw new ArgumentException("collection is null or empty.", "collection");
			this.collection = collection;
			this.usage = usage;
		}

		public CollectionCacheConfiguration(string collection, ClassCacheUsage usage, string region)
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
							usage = CfgXmlHelper.ClassCacheUsageConvertFrom(collectionCacheElement.Value);
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
		public string Collection
		{
			get { return collection; }
		}

		private string region;
		public string Region
		{
			get { return region; }
		}


		private ClassCacheUsage usage;
		public ClassCacheUsage Usage
		{
			get { return usage; }
		}
	}
}
