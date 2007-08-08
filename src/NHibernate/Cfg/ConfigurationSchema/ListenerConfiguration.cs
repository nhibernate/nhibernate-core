using System;
using System.Xml;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	public enum ListenerType
	{
		NotValidType,
		Autoflush,
		Merge,
		Create,
		CreateOnFlush,
		Delete,
		DirtyCheck,
		Evict,
		Flush,
		FlushEntity,
		Load,
		LoadCollection,
		Lock,
		Refresh,
		Replicate,
		SaveUpdate,
		Save,
		PreUpdate,
		Update,
		PreLoad,
		PreDelete,
		PreInsert,
		PostLoad,
		PostInsert,
		PostUpdate,
		PostDelete,
		PostCommitUpdate,
		PostCommitInsert,
		PostCommitDelete
	}

	public class ListenerConfiguration
	{
		internal ListenerConfiguration(XPathNavigator listenerElement)
		{
			Parse(listenerElement);
		}

		public ListenerConfiguration(string clazz)
		{
			if (string.IsNullOrEmpty(clazz))
				throw new ArgumentException("clazz is null or empty.", "clazz");
			this.clazz = clazz;
		}

		public ListenerConfiguration(string clazz, ListenerType type)
			: this(clazz)
		{
			this.type = type;
		}

		private void Parse(XPathNavigator listenerElement)
		{
			if (listenerElement.MoveToFirstAttribute())
			{
				do
				{
					switch (listenerElement.Name)
					{
						case "class":
							if (listenerElement.Value.Trim().Length == 0)
								throw new HibernateConfigException("Invalid listener element; the attribute <class> must be assigned with no empty value");
							clazz = listenerElement.Value;
							break;
						case "type":
							type = CfgXmlHelper.ListenerTypeConvertFrom(listenerElement.Value);
							break;
					}
				}
				while (listenerElement.MoveToNextAttribute());
			}
		}

		private string clazz;
		public string Class
		{
			get { return clazz; }
		}

		private ListenerType type= ListenerType.NotValidType;
		public ListenerType Type
		{
			get { return type; }
		}

	}
}
