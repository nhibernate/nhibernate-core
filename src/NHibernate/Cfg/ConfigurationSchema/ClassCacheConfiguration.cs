using System;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;

namespace NHibernate.Cfg.ConfigurationSchema
{
	public enum ClassCacheUsage
	{
		Readonly,
		ReadWrite,
		NonStrictReadWrite,
		Transactional
	}

	public enum ClassCacheInclude
	{
		All,
		NonLazy
	}

	public class ClassCacheConfiguration
	{
		internal ClassCacheConfiguration(XPathNavigator classCacheElement)
		{
			Parse(classCacheElement);
		}

		public ClassCacheConfiguration(string clazz, ClassCacheUsage usage)
		{
			if (string.IsNullOrEmpty(clazz))
				throw new ArgumentException("clazz is null or empty.", "clazz");
			this.clazz = clazz;
			this.usage = usage;
		}

		public ClassCacheConfiguration(string clazz, ClassCacheUsage usage, ClassCacheInclude include)
			: this(clazz, usage)
		{
			this.include = include;
		}

		public ClassCacheConfiguration(string clazz, ClassCacheUsage usage, string region)
			: this(clazz, usage)
		{
			this.region = region;
		}

		public ClassCacheConfiguration(string clazz, ClassCacheUsage usage, ClassCacheInclude include, string region)
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
							usage = CfgXmlHelper.ClassCacheUsageConvertFrom(classCacheElement.Value);
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
		public string Class
		{
			get { return clazz; }
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

		private ClassCacheInclude include = ClassCacheInclude.All;
		public ClassCacheInclude Include
		{
			get { return include; }
		}

	}
}
