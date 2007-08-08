using System;
using System.Xml;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Values for listener type property.
	/// </summary>
	/// <remarks>Unused</remarks>
	public enum ListenerType
	{
		// TODO:Implement listeners and events (remove de remarks from this enum)
		/// <summary>Not allowed in Xml. It represente de default value when an explicit type is assigned.</summary>
		NotValidType,
		/// <summary>Xml value: auto-flush</summary>
		Autoflush,
		/// <summary>Xml value: merge</summary>
		Merge,
		/// <summary>Xml value: create</summary>
		Create,
		/// <summary>Xml value: create-onflush</summary>
		CreateOnFlush,
		/// <summary>Xml value: delete</summary>
		Delete,
		/// <summary>Xml value: dirty-check</summary>
		DirtyCheck,
		/// <summary>Xml value: evict</summary>
		Evict,
		/// <summary>Xml value: flush</summary>
		Flush,
		/// <summary>Xml value: flush-entity</summary>
		FlushEntity,
		/// <summary>Xml value: load</summary>
		Load,
		/// <summary>Xml value: load-collection</summary>
		LoadCollection,
		/// <summary>Xml value: lock</summary>
		Lock,
		/// <summary>Xml value: refresh</summary>
		Refresh,
		/// <summary>Xml value: replicate</summary>
		Replicate,
		/// <summary>Xml value: save-update</summary>
		SaveUpdate,
		/// <summary>Xml value: save</summary>
		Save,
		/// <summary>Xml value: pre-update</summary>
		PreUpdate,
		/// <summary>Xml value: update</summary>
		Update,
		/// <summary>Xml value: pre-load</summary>
		PreLoad,
		/// <summary>Xml value: pre-delete</summary>
		PreDelete,
		/// <summary>Xml value: pre-insert</summary>
		PreInsert,
		/// <summary>Xml value: post-load</summary>
		PostLoad,
		/// <summary>Xml value: post-insert</summary>
		PostInsert,
		/// <summary>Xml value: post-update</summary>
		PostUpdate,
		/// <summary>Xml value: post-delete</summary>
		PostDelete,
		/// <summary>Xml value: post-commit-update</summary>
		PostCommitUpdate,
		/// <summary>Xml value: post-commit-insert</summary>
		PostCommitInsert,
		/// <summary>Xml value: post-commit-delete</summary>
		PostCommitDelete
	}

	/// <summary>
	/// Configuration parsed values for a listener XML node
	/// </summary>
	public class ListenerConfiguration
	{
		internal ListenerConfiguration(XPathNavigator listenerElement)
		{
			Parse(listenerElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListenerConfiguration"/> class.
		/// </summary>
		/// <param name="clazz">The class full name.</param>
		/// <exception cref="ArgumentException">When <paramref name="clazz"/> is null or empty.</exception>
		public ListenerConfiguration(string clazz)
		{
			if (string.IsNullOrEmpty(clazz))
				throw new ArgumentException("clazz is null or empty.", "clazz");
			this.clazz = clazz;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListenerConfiguration"/> class.
		/// </summary>
		/// <param name="clazz">The class full name.</param>
		/// <param name="type">The listener type.</param>
		/// <exception cref="ArgumentException">When <paramref name="clazz"/> is null or empty.</exception>
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
		/// <summary>
		/// The class full name.
		/// </summary>
		public string Class
		{
			get { return clazz; }
		}

		private ListenerType type= ListenerType.NotValidType;
		/// <summary>
		/// The listener type.
		/// </summary>
		/// <remarks>Default value <see cref="ListenerType.NotValidType"/> mean that the value is ignored.</remarks>
		public ListenerType Type
		{
			get { return type; }
		}

	}
}
