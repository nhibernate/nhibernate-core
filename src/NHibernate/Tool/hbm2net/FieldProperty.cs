using System;
using MultiMap = System.Collections.Hashtable;
using Element = System.Xml.XmlElement;

namespace NHibernate.Tool.hbm2net
{
	
	
	public class FieldProperty:MappingElement
	{
		virtual public System.String FieldName
		{
			get
			{
				return this.fieldName;
			}
			
		}
		virtual public System.String AccessorName
		{
			get
			{
				return this.accessorName;
			}
			
		}
		private System.String GetterType
		{
			get
			{
				return (FullyQualifiedTypeName.Equals("boolean"))?"is":"get";
			}
			
		}
		virtual public System.String FullyQualifiedTypeName
		{
			get
			{
				return classType.FullyQualifiedName;
			}
			
		}
		virtual public bool Identifier
		{
			get
			{
				return id;
			}
			
		}
		virtual public bool Nullable
		{
			get
			{
				return nullable;
			}
			
		}
		virtual public bool Generated
		{
			get
			{
				return generated;
			}
			
		}
		/// <summary> Returns the classType. </summary>
		/// <returns> ClassName
		/// </returns>
		virtual public ClassName ClassType
		{
			get
			{
				return classType;
			}
			
		}
		private ClassName Type
		{
			set
			{
				this.classType = value;
			}
			
		}
		/// <summary> Returns the foreignClass.</summary>
		/// <returns> ClassName
		/// </returns>
		/// <summary> Sets the foreignClass.</summary>
		/// <param name="foreignClass">The foreignClass to set
		/// </param>
		virtual public ClassName ForeignClass
		{
			get
			{
				return foreignClass;
			}
			
			set
			{
				this.foreignClass = value;
			}
			
		}
		/// <summary> Returns the foreignKeys.</summary>
		/// <returns> Set
		/// </returns>
		virtual public SupportClass.SetSupport ForeignKeys
		{
			get
			{
				return foreignKeys;
			}
			
		}
		/// <summary> Method getGetterSignature.</summary>
		/// <returns> String
		/// </returns>
		virtual public System.String GetterSignature
		{
			get
			{
				return GetterType + AccessorName + "()";
			}
			
		}
		virtual public bool GeneratedAsProperty
		{
			get
			{
				return getMetaAsBool("gen-property", true);
			}
			
		}
		/// <summary> </summary>
		/// <returns>  Return the implementation specific type for this property. e.g. java.util.ArrayList when the type is java.util.List;
		/// </returns>
		virtual public ClassName ImplementationClassName
		{
			get
			{
				return implementationClassName;
			}
			
		}
		/// <returns>
		/// </returns>
		virtual public ClassMapping ParentClass
		{
			get
			{
				return parentClass;
			}
			
		}
		virtual public System.String FieldScope
		{
			get
			{
				return getScope("scope-field", "private");
			}
			
		}
		virtual public System.String PropertyGetScope
		{
			get
			{
				return getScope("scope-get", "public");
			}
			
		}
		virtual public System.String PropertySetScope
		{
			get
			{
				return getScope("scope-set", "public");
			}
			
		}
		
		/// <summary>the field name </summary>
		private System.String fieldName = null;
		
		/// <summary>the property name </summary>
		private System.String accessorName = null;
		
		/// <summary>true if this is part of an id </summary>
		private bool id = false;
		
		
		private bool generated = false;
		private bool nullable = true;
		private ClassName classType;
		private ClassName foreignClass;
		private SupportClass.SetSupport foreignKeys;
		
		private ClassName implementationClassName;
		
		private ClassMapping parentClass;
		
		
		public FieldProperty(Element element, MappingElement parent, System.String name, ClassName type, bool nullable, MultiMap metaattribs):base(element, parent)
		{
			initWith(name, type, type, nullable, id, false, null, null, metaattribs);
		}
		
		public FieldProperty(Element element, MappingElement parent, System.String name, ClassName type, bool nullable, bool id, bool generated, MultiMap metaattribs):base(element, parent)
		{
			initWith(name, type, type, nullable, id, generated, null, null, metaattribs);
		}
		
		public FieldProperty(Element element, MappingElement parent, System.String name, ClassName type, ClassName implementationClassName, bool nullable, ClassName foreignClass, SupportClass.SetSupport foreignKeys, MultiMap metaattribs):base(element, parent)
		{
			initWith(name, type, implementationClassName, nullable, id, false, foreignClass, foreignKeys, metaattribs);
		}
		
		protected internal virtual void  initWith(System.String name, ClassName type, ClassName implementationClassName, bool nullable, bool id, bool generated, ClassName foreignClass, SupportClass.SetSupport foreignKeys, MultiMap metaattribs)
		{
			this.fieldName = name;
			Type = type;
			this.nullable = nullable;
			this.id = id;
			this.generated = generated;
			this.implementationClassName = implementationClassName;
			this.accessorName = beancapitalize(name);
			this.foreignClass = foreignClass;
			this.foreignKeys = foreignKeys;
			MetaAttribs = metaattribs;
		}
		
		/// <summary> foo -> Foo
		/// FOo -> FOo
		/// 
		/// </summary>
		/// <param name="">name2
		/// </param>
		/// <returns>
		/// </returns>
		private System.String beancapitalize(System.String fieldname)
		{
			if ((System.Object) fieldname == null || fieldname.Length == 0)
			{
				return fieldname;
			}
			
			if (fieldname.Length > 1 && System.Char.IsUpper(fieldname[1]) && System.Char.IsUpper(fieldname[0]))
			{
				return fieldname;
			}
			char[] chars = fieldname.ToCharArray();
			chars[0] = System.Char.ToUpper(chars[0]);
			return new System.String(chars);
		}
		
		public override System.String ToString()
		{
			return FullyQualifiedTypeName + ":" + FieldName;
		}
		
		public virtual System.String getScope(System.String localScopeName, System.String defaultScope)
		{
			if ((System.Object) defaultScope == null)
				defaultScope = "private";
			return (getMeta(localScopeName) == null)?defaultScope:getMetaAsString(localScopeName);
		}
	}
}