using System.Collections;
using NHibernate.Engine;
using NHibernate.Property;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class Property
	{
		private string name;
		private Value propertyValue;
		private string cascade;
		private bool updateable;
		private bool insertable;
		private string propertyAccessorName;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyValue"></param>
		public Property( Value propertyValue )
		{
			this.propertyValue = propertyValue;
		}

		/// <summary></summary>
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

		/// <summary></summary>
		public bool IsUpdateable
		{
			get { return updateable && !IsFormula; }
			set { updateable = value; }
		}

		/// <summary></summary>
		public bool IsComposite
		{
			get { return propertyValue is Component; }
		}

		/// <summary></summary>
		public Value Value
		{
			get { return propertyValue; }
			set { this.propertyValue = value; }
		}

		/// <summary></summary>
		public Cascades.CascadeStyle CascadeStyle
		{
			get
			{
				IType type = propertyValue.Type;
				if( type.IsComponentType && !type.IsObjectType )
				{
					IAbstractComponentType actype = ( IAbstractComponentType ) propertyValue.Type;
					int length = actype.Subtypes.Length;
					for( int i = 0; i < length; i++ )
					{
						if( actype.Cascade( i ) != Cascades.CascadeStyle.StyleNone )
						{
							return Cascades.CascadeStyle.StyleAll;
						}
					}

					return Cascades.CascadeStyle.StyleNone;
				}
				else
				{
					if( cascade.Equals( "all" ) )
					{
						return Cascades.CascadeStyle.StyleAll;
					}
					else if( cascade.Equals( "all-delete-orphan" ) )
					{
						return Cascades.CascadeStyle.StyleAllGC;
					}
					else if( cascade.Equals( "none" ) )
					{
						return Cascades.CascadeStyle.StyleNone;
					}
					else if( cascade.Equals( "save-update" ) )
					{
						return Cascades.CascadeStyle.StyleSaveUpdate;
					}
					else if( cascade.Equals( "delete" ) )
					{
						return Cascades.CascadeStyle.StyleOnlyDelete;
					}
					else
					{
						throw new MappingException( "Unspported cascade style: " + cascade );
					}
				}
			}
		}

		/// <summary></summary>
		public string Cascade
		{
			get { return cascade; }
			set { cascade = value; }
		}

		/// <summary></summary>
		public bool IsInsertable
		{
			get { return insertable && !IsFormula; }
			set { insertable = value; }
		}

		/// <summary></summary>
		public Formula Formula
		{
			get { return propertyValue.Formula; }
		}

		/// <summary></summary>
		public bool IsFormula
		{
			get { return Formula != null; }
		}

		/// <summary></summary>
		public string PropertyAccessorName
		{
			get { return propertyAccessorName; }
			set { propertyAccessorName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <returns></returns>
		public IGetter GetGetter( System.Type clazz )
		{
			return PropertyAccessor.GetGetter( clazz, name );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <returns></returns>
		public ISetter GetSetter( System.Type clazz )
		{
			return PropertyAccessor.GetSetter( clazz, name );
		}

		/// <summary></summary>
		protected IPropertyAccessor PropertyAccessor
		{
			get { return PropertyAccessorFactory.GetPropertyAccessor( PropertyAccessorName ); }
		}

		/// <summary></summary>
		public bool IsBasicPropertyAccessor
		{
			get { return propertyAccessorName == null || propertyAccessorName.Equals( "property" ); }
		}
	}
}