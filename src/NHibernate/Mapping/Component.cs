using System;
using System.Collections;

namespace NHibernate.Mapping
{
	/// <summary>
	/// The mapping for a component, composite element, composite identifier,
	/// etc.
	/// </summary>
	public class Component : SimpleValue
	{
		private ArrayList properties = new ArrayList();
		private System.Type componentClass;
		private bool embedded;
		private string parentProperty;
		private PersistentClass owner;
		private bool dynamic;

		/// <summary></summary>
		public int PropertySpan
		{
			get { return properties.Count; }
		}

		/// <summary></summary>
		public ICollection PropertyCollection
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
				foreach (Property p in PropertyCollection)
				{
					n += p.ColumnSpan;
				}
				return n;
			}
		}

		/// <summary></summary>
		public override ICollection ColumnCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				foreach (Property prop in PropertyCollection)
				{
					retVal.AddRange(prop.ColumnCollection);
				}
				return retVal;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public Component(Table table)
			: base(table)
		{
			this.owner = null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyAccess"></param>
		public override void SetTypeByReflection(System.Type propertyClass, string propertyName, string propertyAccess)
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
			get { return componentClass; }
			set { componentClass = value; }
		}

		/// <summary></summary>
		public PersistentClass Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		/// <summary></summary>
		public string ParentProperty
		{
			get { return parentProperty; }
			set { parentProperty = value; }
		}

		/// <summary></summary>
		public ArrayList Properties
		{
			get { return properties; }
		}

		public override bool[] ColumnInsertability
		{
			get
			{
				bool[] result = new bool[ColumnSpan];
				int i = 0;
				foreach (Property prop in PropertyCollection)
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
				foreach (Property prop in PropertyCollection)
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

		public Property GetProperty(string propertyName)
		{
			ICollection iter = PropertyCollection;
			foreach (Property prop in iter)
			{
				if (prop.Name.Equals(propertyName))
				{
					return prop;
				}
			}
			throw new MappingException("component property not found: " + propertyName);
		}
	}
}