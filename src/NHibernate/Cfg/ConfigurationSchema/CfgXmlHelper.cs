using System.Xml;
using System.Xml.XPath;
using NHibernate.Event;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Helper to parse hibernate-configuration XmlNode.
	/// </summary>
	public static class CfgXmlHelper
	{
		/// <summary>
		/// The XML node name for hibernate configuration section in the App.config/Web.config and
		/// for the hibernate.cfg.xml .
		/// </summary>
		public const string CfgSectionName = "hibernate-configuration";

		/// <summary>The XML Namespace for the nhibernate-configuration</summary>
		public const string CfgSchemaXMLNS = "urn:nhibernate-configuration-2.2";

		// note that the prefix has absolutely nothing to do with what the user
		// selects as their prefix in the document. It is the prefix we use to 
		// build the XPath and the nsMgr takes care of translating our prefix into
		// the user defined prefix...
		public const string CfgNamespacePrefix = "cfg";
		private const string RootPrefixPath = "//" + CfgNamespacePrefix + ":";
		private const string ChildPrefixPath = CfgNamespacePrefix + ":";

		private static readonly XmlNamespaceManager nsMgr;

		static CfgXmlHelper()
		{
			NameTable nt = new NameTable();
			nsMgr = new XmlNamespaceManager(nt);
			nsMgr.AddNamespace(CfgNamespacePrefix, CfgSchemaXMLNS);

			ByteCodeProviderExpression = XPathExpression.Compile(RootPrefixPath + "bytecode-provider", nsMgr);
			ReflectionOptimizerExpression = XPathExpression.Compile(RootPrefixPath + "reflection-optimizer", nsMgr);
			SessionFactoryExpression = XPathExpression.Compile(RootPrefixPath + "session-factory", nsMgr);
			SessionFactoryPropertiesExpression = XPathExpression.Compile(RootPrefixPath + "session-factory/" + ChildPrefixPath + "property", nsMgr);
			SessionFactoryMappingsExpression = XPathExpression.Compile(RootPrefixPath + "session-factory/" + ChildPrefixPath + "mapping", nsMgr);
			SessionFactoryClassesCacheExpression = XPathExpression.Compile(RootPrefixPath + "session-factory/" + ChildPrefixPath + "class-cache", nsMgr);
			SessionFactoryCollectionsCacheExpression = XPathExpression.Compile(RootPrefixPath + "session-factory/" + ChildPrefixPath + "collection-cache", nsMgr);
			SessionFactoryEventsExpression = XPathExpression.Compile(RootPrefixPath + "session-factory/" + ChildPrefixPath + "event", nsMgr);
			SessionFactoryListenersExpression = XPathExpression.Compile(RootPrefixPath + "session-factory/" + ChildPrefixPath + "listener", nsMgr);
		}

		/// <summary>XPath expression for bytecode-provider property.</summary>
		public static readonly XPathExpression ByteCodeProviderExpression;
		/// <summary>XPath expression for reflection-optimizer property.</summary>
		public static readonly XPathExpression ReflectionOptimizerExpression;
		/// <summary>XPath expression for session-factory whole node.</summary>
		public static readonly XPathExpression SessionFactoryExpression;
		/// <summary>XPath expression for session-factory.property nodes</summary>
		public static readonly XPathExpression SessionFactoryPropertiesExpression;
		/// <summary>XPath expression for session-factory.mapping nodes</summary>
		public static readonly XPathExpression SessionFactoryMappingsExpression;
		/// <summary>XPath expression for session-factory.class-cache nodes</summary>
		public static readonly XPathExpression SessionFactoryClassesCacheExpression;
		/// <summary>XPath expression for session-factory.collection-cache nodes</summary>
		public static readonly XPathExpression SessionFactoryCollectionsCacheExpression;
		/// <summary>XPath expression for session-factory.event nodes</summary>
		public static readonly XPathExpression SessionFactoryEventsExpression;
		/// <summary>XPath expression for session-factory.listener nodes</summary>
		public static readonly XPathExpression SessionFactoryListenersExpression;

		internal static string ToConfigurationString(this BytecodeProviderType source)
		{
			switch (source)
			{
				case BytecodeProviderType.Codedom:
					return "codedom";
				case BytecodeProviderType.Lcg:
					return "lcg";
				default:
					return "null";
			}
		}

		/// <summary>
		/// Convert a string to <see cref="ClassCacheInclude"/>.
		/// </summary>
		/// <param name="include">The string that represent <see cref="ClassCacheInclude"/>.</param>
		/// <returns>
		/// The <paramref name="include"/> converted to <see cref="ClassCacheInclude"/>.
		/// </returns>
		/// <exception cref="HibernateConfigException">If the values is invalid.</exception>
		/// <remarks>
		/// See <see cref="ClassCacheInclude"/> for allowed values.
		/// </remarks>
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

		/// <summary>
		/// Convert a string to <see cref="ListenerType"/>.
		/// </summary>
		/// <param name="listenerType">The string that represent <see cref="ListenerType"/>.</param>
		/// <returns>
		/// The <paramref name="listenerType"/> converted to <see cref="ListenerType"/>.
		/// </returns>
		/// <exception cref="HibernateConfigException">If the values is invalid.</exception>
		/// <remarks>
		/// See <see cref="ListenerType"/> for allowed values.
		/// </remarks>
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
				case "pre-collection-recreate":
					return ListenerType.PreCollectionRecreate;
				case "pre-collection-remove":
					return ListenerType.PreCollectionRemove;
				case "pre-collection-update":
					return ListenerType.PreCollectionUpdate;
				case "post-collection-recreate":
					return ListenerType.PostCollectionRecreate;
				case "post-collection-remove":
					return ListenerType.PostCollectionRemove;
				case "post-collection-update":
					return ListenerType.PostCollectionUpdate;
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
				case ListenerType.PreCollectionRecreate:
					return "pre-collection-recreate";
				case ListenerType.PreCollectionRemove:
					return "pre-collection-remove";
				case ListenerType.PreCollectionUpdate:
					return "pre-collection-update";
				case ListenerType.PostCollectionRecreate:
					return "post-collection-recreate";
				case ListenerType.PostCollectionRemove:
					return "post-collection-remove";
				case ListenerType.PostCollectionUpdate:
					return "post-collection-update";
				default:
					return string.Empty;
			}
		}
	}

}
