using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Properties;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Mapping for a property of a .NET class (entity
	/// or component).
	/// </summary>
	[Serializable]
	public class Property : IMetaAttributable
	{
		private string name;
		private IValue propertyValue;
		private string cascade;
		private bool updateable = true;
		private bool insertable = true;
		private bool selectable = true;
		private string propertyAccessorName;
		private bool optional;
		private string nodeName;
		private IDictionary<string, MetaAttribute> metaAttributes;
		private PersistentClass persistentClass;
		private bool isOptimisticLocked;
		private PropertyGeneration generation = PropertyGeneration.Never;
		private bool isLazy;
		private bool isNaturalIdentifier;

		public Property()
		{
		}

		public Property(IValue propertyValue)
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
		public IEnumerable<ISelectable> ColumnIterator
		{
			get { return propertyValue.ColumnIterator; }
		}

		/// <summary>
		/// Gets or Sets the name of the Property in the class.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool IsComposite
		{
			get { return propertyValue is Component; }
		}

		public IValue Value
		{
			get { return propertyValue; }
			set { propertyValue = value; }
		}

		public CascadeStyle CascadeStyle
		{
			get
			{
				IType type = propertyValue.Type;
				if (type.IsComponentType && !type.IsAnyType)
				{
					IAbstractComponentType actype = (IAbstractComponentType)propertyValue.Type;
					int length = actype.Subtypes.Length;
					for (int i = 0; i < length; i++)
					{
						if (actype.GetCascadeStyle(i) != CascadeStyle.None)
						{
							return CascadeStyle.All;
						}
					}

					return CascadeStyle.None;
				}
				else if (string.IsNullOrEmpty(cascade) || cascade.Equals("none"))
				{
					return CascadeStyle.None;
				}
				else
				{
					string[] tokens = cascade.Split(new char[] { ',' });
					if (tokens.Length == 0) return CascadeStyle.None;
					CascadeStyle[] styles = new CascadeStyle[tokens.Length];
					int i = 0;
					foreach (string token in tokens)
					{
						styles[i++] = CascadeStyle.GetCascadeStyle(token.ToLowerInvariant().Trim());
					}
					if (tokens.Length == 1) return styles[0];
					else return new CascadeStyle.MultipleCascadeStyle(styles);
				}
			}
		}

		public string Cascade
		{
			get { return cascade; }
			set { cascade = value; }
		}

		public bool IsUpdateable
		{
			get
			{
				bool[] columnUpdateability = propertyValue.ColumnUpdateability;
				return updateable &&
					   (
					// columnUpdateability.Length == 0 ||
					   !ArrayHelper.IsAllFalse(columnUpdateability)
					   );
			}
			set { updateable = value; }
		}

		public bool IsInsertable
		{
			get
			{
				bool[] columnInsertability = propertyValue.ColumnInsertability;
				return insertable &&
					   (
						columnInsertability.Length == 0 ||
						!ArrayHelper.IsAllFalse(columnInsertability)
					   );
			}
			set { insertable = value; }
		}

		/// <summary></summary>
		public bool IsNullable
		{
			get { return propertyValue == null || propertyValue.IsNullable; }
		}

		public bool IsOptional
		{
			get { return optional || IsNullable; }
			set { optional = value; }
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

		protected virtual IPropertyAccessor PropertyAccessor
		{
			get { return PropertyAccessorFactory.GetPropertyAccessor(PropertyAccessorName); }
		}

		public virtual string GetAccessorPropertyName(EntityMode mode)
		{
			return mode == EntityMode.Xml ? nodeName : Name;
		}

		public virtual bool IsBasicPropertyAccessor
		{
			// NH Different behavior : see IPropertyAccessor.CanAccessThroughReflectionOptimizer (ref. NH-1304)
			get { return PropertyAccessor.CanAccessThroughReflectionOptimizer; }
		}

		public IDictionary<string, MetaAttribute> MetaAttributes
		{
			get { return metaAttributes; }
			set { metaAttributes = value; }
		}

		public MetaAttribute GetMetaAttribute(string attributeName)
		{
			if (metaAttributes == null)
			{
				return null;
			}
			MetaAttribute result;
			metaAttributes.TryGetValue(attributeName, out result);
			return result;
		}

		public bool IsValid(IMapping mapping)
		{
			return Value.IsValid(mapping);
		}

		public string NullValue
		{
			get
			{
				if (propertyValue is SimpleValue)
				{
					return ((SimpleValue)propertyValue).NullValue;
				}
				else
					return null;
			}
		}

		public PersistentClass PersistentClass
		{
			get { return persistentClass; }
			set { persistentClass = value; }
		}

		public bool IsSelectable
		{
			get { return selectable; }
			set { selectable = value; }
		}

		public bool IsOptimisticLocked
		{
			get { return isOptimisticLocked; }
			set { isOptimisticLocked = value; }
		}

		public PropertyGeneration Generation
		{
			get { return generation; }
			set { generation = value; }
		}

		public bool IsLazy
		{
			get
			{
				// NH Specific: Hibernate doesn't make a distinction between
				// lazy and no-proxy, but NHibernate does. While Hibernate tracks
				// just whatever a property is lazy, we need to track lazy/no-proxy seperatedly.
				
				return isLazy;
			}
			set { isLazy = value; }
		}

		public virtual bool BackRef
		{
			get { return false; }
		}

		public bool IsNaturalIdentifier
		{
			get { return isNaturalIdentifier; }
			set { isNaturalIdentifier = value; }
		}

		public string NodeName
		{
			get { return nodeName; }
			set { nodeName = value; }
		}

		// both many-to-one and one-to-one are represented as a
		// Property.  EntityPersister is relying on this value to
		// determine "lazy fetch groups" in terms of field-level
		// interception.  So we need to make sure that we return
		// true here for the case of many-to-one and one-to-one
		// with lazy="no-proxy"
		public bool UnwrapProxy { get; set; }

		public bool IsEntityRelation
		{
			get { return (Value as ToOne) != null; }
		}
	}
}
