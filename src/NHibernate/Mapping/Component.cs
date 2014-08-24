using System;
using System.Collections.Generic;
using NHibernate.Tuple.Component;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// The mapping for a component, composite element, composite identifier,
	/// etc.
	/// </summary>
	[Serializable]
	public class Component : SimpleValue, IMetaAttributable
	{
		private readonly List<Property> properties = new List<Property>();
		private System.Type componentClass;
		private bool embedded;
		private Property parentProperty;
		private PersistentClass owner;
		private bool dynamic;
		private bool isKey;
		private string nodeName;
		private string roleName;
		private Dictionary<EntityMode, string> tuplizerImpls;
		private string componentClassName;

		/// <summary></summary>
		public int PropertySpan
		{
			get { return properties.Count; }
		}

		/// <summary></summary>
		public IEnumerable<Property> PropertyIterator
		{
			get { return properties; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		public void AddProperty(Property p)
		{
			properties.Add(p);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		public override void AddColumn(Column column)
		{
			throw new NotSupportedException("Cant add a column to a component");
		}

		/// <summary></summary>
		public override int ColumnSpan
		{
			get
			{
				int n = 0;
				foreach (Property p in PropertyIterator)
					n += p.ColumnSpan;

				return n;
			}
		}

		/// <summary></summary>
		public override IEnumerable<ISelectable> ColumnIterator
		{
			get
			{
				List<IEnumerable<ISelectable>> iters = new List<IEnumerable<ISelectable>>();
				foreach (Property property in PropertyIterator)
				{
					iters.Add(property.ColumnIterator);
				}
				return new JoinedEnumerable<ISelectable>(iters);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public Component(PersistentClass owner)
			: base(owner.Table)
		{
			this.owner = owner;
		}

		public Component(Table table, PersistentClass owner)
			: base(table)
		{
			this.owner = owner;
		}

		public Component(Collection collection)
			: base(collection.CollectionTable)
		{
			owner = collection.Owner;
		}

		public Component(Component component)
			: base(component.Table)
		{
			owner = component.Owner;
		}

		public override void SetTypeUsingReflection(string className, string propertyName, string accesorName)
		{
		}

		/// <summary></summary>
		public bool IsEmbedded
		{
			get { return embedded; }
			set { embedded = value; }
		}

		/// <summary></summary>
		public bool IsDynamic
		{
			get { return dynamic; }
			set { dynamic = value; }
		}

		/// <summary></summary>
		public System.Type ComponentClass
		{
			get
			{
				// NH Different implementation (we use reflection only when needed)
				if (componentClass == null)
				{
					try
					{
						componentClass = ReflectHelper.ClassForName(componentClassName);
					}
					catch (Exception cnfe)
					{
						if (!IsDynamic) // TODO remove this if leave the Exception
							throw new MappingException("component class not found: " + componentClassName, cnfe);
						return null;
					}
				}
				return componentClass;
			}
			set // TODO NH: Remove the setter
			{
				componentClass = value;
				if (componentClass != null)
					componentClassName = componentClass.AssemblyQualifiedName;
			} 
		}

		/// <summary></summary>
		public PersistentClass Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		/// <summary></summary>
		public Property ParentProperty
		{
			get { return parentProperty; }
			set { parentProperty = value; }
		}

		public override bool[] ColumnInsertability
		{
			get
			{
				bool[] result = new bool[ColumnSpan];
				int i = 0;
				foreach (Property prop in PropertyIterator)
				{
					bool[] chunk = prop.Value.ColumnInsertability;
					if (prop.IsInsertable)
					{
						System.Array.Copy(chunk, 0, result, i, chunk.Length);
					}
					i += chunk.Length;
				}
				return result;
			}
		}

		public override bool[] ColumnUpdateability
		{
			get
			{
				bool[] result = new bool[ColumnSpan];
				int i = 0;
				foreach (Property prop in PropertyIterator)
				{
					bool[] chunk = prop.Value.ColumnUpdateability;
					if (prop.IsUpdateable)
					{
						System.Array.Copy(chunk, 0, result, i, chunk.Length);
					}
					i += chunk.Length;
				}
				return result;
			}
		}

		public string ComponentClassName
		{
			get { return componentClassName; }
			set
			{
				if ((componentClassName == null && value != null) || (componentClassName != null && !componentClassName.Equals(value)))
				{
					componentClass = null;
					componentClassName = value;
				}
			}
		}

		public bool IsKey
		{
			get { return isKey; }
			set { isKey = value; }
		}

		public string NodeName
		{
			get { return nodeName; }
			set { nodeName = value; }
		}

		public string RoleName
		{
			get { return roleName; }
			set { roleName = value; }
		}

		public Property GetProperty(string propertyName)
		{
			IEnumerable<Property> iter = PropertyIterator;
			foreach (Property prop in iter)
			{
				if (prop.Name.Equals(propertyName))
				{
					return prop;
				}
			}
			throw new MappingException("component property not found: " + propertyName);
		}

		public virtual void AddTuplizer(EntityMode entityMode, string implClassName)
		{
			if (tuplizerImpls == null)
				tuplizerImpls = new Dictionary<EntityMode, string>();

			tuplizerImpls[entityMode] = implClassName;
		}

		public virtual string GetTuplizerImplClassName(EntityMode mode)
		{
			// todo : remove this once ComponentMetamodel is complete and merged
			if (tuplizerImpls == null)
			{
				return null;
			}
			return tuplizerImpls[mode];
		}

		public virtual IDictionary<EntityMode, string> TuplizerMap
		{
			get
			{
				if (tuplizerImpls == null)
					return null;

				return tuplizerImpls;
			}
		}

		public bool HasPocoRepresentation
		{
			get { return componentClassName != null; }
		}

		public override IType Type
		{
			get
			{
				if (type == null)
					type = BuildType();

				return type;
			}
		}

		private IType type;

		private IType BuildType()
		{
			// TODO : temporary initial step towards HHH-1907
			ComponentMetamodel metamodel = new ComponentMetamodel(this);
			if (IsEmbedded)
			{
				return new EmbeddedComponentType(metamodel);
			}
			else
			{
				return new ComponentType(metamodel);
			}
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + StringHelper.CollectionToString(properties) + ')';
		}

		public IDictionary<string, MetaAttribute> MetaAttributes { get; set; }

		public MetaAttribute GetMetaAttribute(string attributeName)
		{
			if (MetaAttributes == null)
			{
				return null;
			}
			MetaAttribute result;
			MetaAttributes.TryGetValue(attributeName, out result);
			return result;
		}
	}
}
