using System;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;

namespace NHibernate.Cfg.ConfigurationSchema
{
	public static class CfgXmlHelper
	{
		public const string CfgSectionName = "hibernate-configuration";
		public const string CfgSchemaXMLNS = "urn:nhibernate-configuration-2.2";

		// note that the prefix has absolutely nothing to do with what the user
		// selects as their prefix in the document. It is the prefix we use to 
		// build the XPath and the nsMgr takes care of translating our prefix into
		// the user defined prefix...
		public const string CfgNamespacePrefix = "cfg";
		private const string RootPrefixPath = "//" + CfgNamespacePrefix + ":";
		private const string ChildPrefixPath = CfgNamespacePrefix + ":";

		private static XmlNamespaceManager nsMgr;

		static CfgXmlHelper()
		{
			NameTable nt = new NameTable();
			nsMgr = new XmlNamespaceManager(nt);
			nsMgr.AddNamespace(CfgNamespacePrefix, CfgSchemaXMLNS);

			ByteCodeProviderExpression = XPathExpression.Compile(RootPrefixPath + "bytecode-provider", nsMgr);
			ReflectionOptimizerExpression = XPathExpression.Compile(RootPrefixPath + "reflection-optimizer", nsMgr);
			SessionFactoryExpression = XPathExpression.Compile(RootPrefixPath + "session-factory", nsMgr);
			SessionFactoryPropertiesExpression = XPathExpression.Compile(RootPrefixPath + "session-factory//" + ChildPrefixPath + "property", nsMgr);
			SessionFactoryMappingsExpression = XPathExpression.Compile(RootPrefixPath + "session-factory//" + ChildPrefixPath + "mapping", nsMgr);
			SessionFactoryClassesCacheExpression = XPathExpression.Compile(RootPrefixPath + "session-factory//" + ChildPrefixPath + "class-cache", nsMgr);
			SessionFactoryCollectionsCacheExpression = XPathExpression.Compile(RootPrefixPath + "session-factory//" + ChildPrefixPath + "collection-cache", nsMgr);
			SessionFactoryEventsExpression = XPathExpression.Compile(RootPrefixPath + "session-factory//" + ChildPrefixPath + "event", nsMgr);
			SessionFactoryListenersExpression = XPathExpression.Compile(RootPrefixPath + "session-factory//" + ChildPrefixPath + "listener", nsMgr);
		}

		public static readonly XPathExpression ByteCodeProviderExpression;
		public static readonly XPathExpression ReflectionOptimizerExpression;
		public static readonly XPathExpression SessionFactoryExpression;
		public static readonly XPathExpression SessionFactoryPropertiesExpression;
		public static readonly XPathExpression SessionFactoryMappingsExpression;
		public static readonly XPathExpression SessionFactoryClassesCacheExpression;
		public static readonly XPathExpression SessionFactoryCollectionsCacheExpression;
		public static readonly XPathExpression SessionFactoryEventsExpression;
		public static readonly XPathExpression SessionFactoryListenersExpression;

		public static BytecodeProviderType ByteCodeProviderConvertFrom(string byteCodeProvider)
		{
			switch (byteCodeProvider)
			{
				case "codedom":
					return BytecodeProviderType.Codedom;
				case "lcg":
					return BytecodeProviderType.Lcg;
				default:
					return BytecodeProviderType.Null;
			}
		}

		internal static string ByteCodeProviderToString(BytecodeProviderType byteCodeProvider)
		{
			switch (byteCodeProvider)
			{
				case BytecodeProviderType.Codedom:
					return "codedom";
				case BytecodeProviderType.Lcg:
					return "lcg";
				default:
					return "null";
			}
		}

		public static ClassCacheUsage ClassCacheUsageConvertFrom(string usage)
		{
			switch (usage)
			{
				case "read-only":
					return ClassCacheUsage.Readonly;
				case "read-write":
					return ClassCacheUsage.ReadWrite;
				case "nonstrict-read-write":
					return ClassCacheUsage.NonStrictReadWrite;
				case "transactional":
					return ClassCacheUsage.Transactional;
				default:
					throw new HibernateConfigException(string.Format("Invalid ClassCacheUsage value:{0}", usage));
			}
		}

		internal static string ClassCacheUsageConvertToString(ClassCacheUsage usage)
		{
			switch (usage)
			{
				case ClassCacheUsage.Readonly:
					return "read-only";
				case ClassCacheUsage.ReadWrite:
					return "read-write";
				case ClassCacheUsage.NonStrictReadWrite:
					return "nonstrict-read-write";
				case ClassCacheUsage.Transactional:
					return "transactional";
				default:
					return string.Empty;
			}
		}

		public static ClassCacheInclude ClassCacheIncludeConvertFrom(string include)
		{
			switch (include)
			{
				case "all":
					return ClassCacheInclude.All;
				case "non-lazy":
					return ClassCacheInclude.NonLazy;
				default:
					throw new HibernateConfigException(string.Format("Invalid ClassCacheInclude value:{0}", include));
			}
		}

		internal static string ClassCacheIncludeConvertToString(ClassCacheInclude include)
		{
			switch (include)
			{
				case ClassCacheInclude.All:
					return "all";
				case ClassCacheInclude.NonLazy:
					return "non-lazy";
				default:
					return string.Empty;
			}
		}

		public static ListenerType ListenerTypeConvertFrom(string listenerType)
		{
			switch (listenerType)
			{
				case "auto-flush":
					return ListenerType.Autoflush;
				case "merge":
					return ListenerType.Merge;
				case "create":
					return ListenerType.Create;
				case "create-onflush":
					return ListenerType.CreateOnFlush;
				case "delete":
					return ListenerType.Delete;
				case "dirty-check":
					return ListenerType.DirtyCheck;
				case "evict":
					return ListenerType.Evict;
				case "flush":
					return ListenerType.Flush;
				case "flush-entity":
					return ListenerType.FlushEntity;
				case "load":
					return ListenerType.Load;
				case "load-collection":
					return ListenerType.LoadCollection;
				case "lock":
					return ListenerType.Lock;
				case "refresh":
					return ListenerType.Refresh;
				case "replicate":
					return ListenerType.Replicate;
				case "save-update":
					return ListenerType.SaveUpdate;
				case "save":
					return ListenerType.Save;
				case "pre-update":
					return ListenerType.PreUpdate;
				case "update":
					return ListenerType.Update;
				case "pre-load":
					return ListenerType.PreLoad;
				case "pre-delete":
					return ListenerType.PreDelete;
				case "pre-insert":
					return ListenerType.PreInsert;
				case "post-load":
					return ListenerType.PostLoad;
				case "post-insert":
					return ListenerType.PostInsert;
				case "post-update":
					return ListenerType.PostUpdate;
				case "post-delete":
					return ListenerType.PostDelete;
				case "post-commit-update":
					return ListenerType.PostCommitUpdate;
				case "post-commit-insert":
					return ListenerType.PostCommitInsert;
				case "post-commit-delete":
					return ListenerType.PostCommitDelete;
				default:
					throw new HibernateConfigException(string.Format("Invalid ListenerType value:{0}", listenerType));
			}
		}

		internal static string ListenerTypeConvertToString(ListenerType listenerType)
		{
			switch (listenerType)
			{
				case ListenerType.Autoflush:
					return "auto-flush";
				case ListenerType.Merge:
					return "merge";
				case ListenerType.Create:
					return "create";
				case ListenerType.CreateOnFlush:
					return "create-onflush";
				case ListenerType.Delete:
					return "delete";
				case ListenerType.DirtyCheck:
					return "dirty-check";
				case ListenerType.Evict:
					return "evict";
				case ListenerType.Flush:
					return "flush";
				case ListenerType.FlushEntity:
					return "flush-entity";
				case ListenerType.Load:
					return "load";
				case ListenerType.LoadCollection:
					return "load-collection";
				case ListenerType.Lock:
					return "lock";
				case ListenerType.Refresh:
					return "refresh";
				case ListenerType.Replicate:
					return "replicate";
				case ListenerType.SaveUpdate:
					return "save-update";
				case ListenerType.Save:
					return "save";
				case ListenerType.PreUpdate:
					return "pre-update";
				case ListenerType.Update:
					return "update";
				case ListenerType.PreLoad:
					return "pre-load";
				case ListenerType.PreDelete:
					return "pre-delete";
				case ListenerType.PreInsert:
					return "pre-insert";
				case ListenerType.PostLoad:
					return "post-load";
				case ListenerType.PostInsert:
					return "post-insert";
				case ListenerType.PostUpdate:
					return "post-update";
				case ListenerType.PostDelete:
					return "post-delete";
				case ListenerType.PostCommitUpdate:
					return "post-commit-update";
				case ListenerType.PostCommitInsert:
					return "post-commit-insert";
				case ListenerType.PostCommitDelete:
					return "post-commit-delete";
				default:
					return string.Empty;
			}
		}
	}

}
