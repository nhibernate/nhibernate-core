using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Property;

namespace NHibernate.Mapping 
{
	public class Property 
	{
		private string name;
		private Value propertyValue;
		private string cascade;
		private bool updateable;
		private bool insertable;
		private string propertyAccessorName;

		public Property(Value propertyValue) 
		{
			this.propertyValue = propertyValue;
		}

		public IType Type 
		{
			get { return propertyValue.Type; }
		}

		/// <summary>
		/// Gets the number of columns this property uses in the db.
		/// </summary>
		public int ColumnSpan 
		{
			get { return propertyValue.ColumnSpan; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Column"/>s.
		/// </summary>
		public ICollection ColumnCollection 
		{
			get { return propertyValue.ColumnCollection; }
		}

		/// <summary>
		/// Gets or Sets the name of the Property in the class.
		/// </summary>
		public string Name 
		{
			get { return name; }
			set { name = value; }
		}

		public bool IsUpdateable 
		{
			get { return updateable && !IsFormula ; }
			set { updateable = value; }
		}

		public bool IsComposite 
		{
			get { return propertyValue is Component; }
		}

		public Value Value 
		{
			get { return propertyValue; }
			set { this.propertyValue = value; }
		}

		public Cascades.CascadeStyle CascadeStyle 
		{
			get {
				IType type = propertyValue.Type;
				if (type.IsComponentType && !type.IsObjectType) 
				{
					IAbstractComponentType actype = (IAbstractComponentType) propertyValue.Type;
					int length = actype.Subtypes.Length;
					for (int i=0; i<length; i++) 
					{
						if ( actype.Cascade(i)!= Cascades.CascadeStyle.StyleNone ) return Cascades.CascadeStyle.StyleAll;
					}

					return Cascades.CascadeStyle.StyleNone;
				} 
				else 
				{
					if (cascade.Equals("all")) 
					{
						return Cascades.CascadeStyle.StyleAll;
					} 
					else if (cascade.Equals("all-delete-orphan") )
					{
						return Cascades.CascadeStyle.StyleAllGC;
					}
					else if (cascade.Equals("none")) 
					{
						return Cascades.CascadeStyle.StyleNone;
					} 
					else if (cascade.Equals("save-update")) 
					{
						return Cascades.CascadeStyle.StyleSaveUpdate;
					} 
					else if (cascade.Equals("delete")) 
					{
						return Cascades.CascadeStyle.StyleOnlyDelete;
					} 
					else 
					{
						throw new MappingException("Unspported cascade style: " + cascade);
					}
				}
			}
		}

		public string Cascade 
		{
			get { return cascade; }
			set { cascade = value; }
		}

		public bool IsInsertable {
			get { return insertable && !IsFormula; }
			set { insertable = value; }
		}

		public Formula Formula 
		{
			get { return propertyValue.Formula; }
		}

		public bool IsFormula 
		{
			get { return Formula!=null; }
		}

		public string PropertyAccessorName 
		{
			get { return propertyAccessorName; }
			set { propertyAccessorName = value; }
		}

		public IGetter GetGetter(System.Type clazz) 
		{
			return PropertyAccessor.GetGetter(clazz, name);
		}

		public ISetter GetSetter(System.Type clazz) 
		{
			return PropertyAccessor.GetSetter(clazz, name);
		}

		protected IPropertyAccessor PropertyAccessor 
		{
			get { return PropertyAccessorFactory.GetPropertyAccessor( PropertyAccessorName ); }
		}

		public bool IsBasicPropertyAccessor 
		{
			get { return propertyAccessorName==null || propertyAccessorName.Equals("property"); }
		}
	}
}