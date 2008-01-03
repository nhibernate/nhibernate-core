using System;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An array has a primary key consisting of the key columns + index column
	/// </summary>
	[Serializable]
	public class Array : List
	{
		private System.Type elementClass;
		private string elementClassName;

		public Array(PersistentClass owner) : base(owner)
		{
		}

		public System.Type ElementClass
		{
			get
			{
				if (elementClass == null)
				{
					if (elementClassName == null)
					{
						IType elementType = Element.Type;
						elementClass = IsPrimitiveArray ? ((PrimitiveType) elementType).PrimitiveClass : elementType.ReturnedClass;
					}
					else
					{
						try
						{
							elementClass = ReflectHelper.ClassForName(elementClassName);
						}
						catch (Exception cnfe)
						{
							throw new MappingException(cnfe);
						}
					}
				}
				return elementClass;
			}
		}

		public override CollectionType DefaultCollectionType
		{
			get { return TypeFactory.Array(Role, ReferencedPropertyName, Embedded, ElementClass); }
		}

		public override bool IsArray
		{
			get { return true; }
		}

		public string ElementClassName
		{
			get { return elementClassName; }
			set
			{
				if ((elementClassName == null && value != null) || (elementClassName != null && !elementClassName.Equals(value)))
				{
					// the property change
					elementClassName = value;
					elementClass = null; // invalidate type
				}
			}
		}
	}
}