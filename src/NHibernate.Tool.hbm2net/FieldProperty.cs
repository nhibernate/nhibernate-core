using System;
using System.Reflection;
using log4net;
using MultiMap = System.Collections.Hashtable;
using Element = System.Xml.XmlElement;

namespace NHibernate.Tool.hbm2net
{
	
	
	public class FieldProperty:MappingElement
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public string fieldcase
		{
			get
			{
				if (fieldName.Substring(0,1) == fieldName.Substring(0,1).ToLower())
					return "_" + fieldName;
				return fieldName.Substring(0,1).ToLower() + fieldName.Substring(1);
			}
		}
		public string propcase
		{
			get
			{
				return fieldName;
			}
		}
		virtual public String FieldName
		{
			get
			{
				return this.fieldName;
			}
			
		}
		virtual public String AccessorName
		{
			get
			{
				return this.accessorName;
			}
			
		}
		private String GetterType
		{
			get
			{
				return (FullyQualifiedTypeName.Equals("boolean"))?"is":"get";
			}
			
		}
		virtual public String FullyQualifiedTypeName
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
		virtual public String GetterSignature
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
		virtual public String FieldScope
		{
			get
			{
				return getScope("scope-field", "private");
			}
			
		}
		virtual public String PropertyGetScope
		{
			get
			{
				return getScope("scope-get", "public");
			}
			
		}
		virtual public String PropertySetScope
		{
			get
			{
				return getScope("scope-set", "public");
			}
			
		}
		
		/// <summary>the field name </summary>
		private String fieldName = null;
		
		/// <summary>the property name </summary>
		private String accessorName = null;
		
		/// <summary>true if this is part of an id </summary>
		private bool id = false;
		
		
		private bool generated = false;
		private bool nullable = true;
		private ClassName classType;
		private ClassName foreignClass;
		private SupportClass.SetSupport foreignKeys;
		
		private ClassName implementationClassName;
		
		private ClassMapping parentClass;

		public FieldProperty(Element element, MappingElement parent, String name, ClassName type, bool nullable, MultiMap metaattribs):base(element, parent)
		{
			initWith(name, type, type, nullable, id, false, null, null, metaattribs);
		}
		
		public FieldProperty(Element element, MappingElement parent, String name, ClassName type, bool nullable, bool id, bool generated, MultiMap metaattribs):base(element, parent)
		{
			initWith(name, type, type, nullable, id, generated, null, null, metaattribs);
		}
		
		public FieldProperty(Element element, MappingElement parent, String name, ClassName type, ClassName implementationClassName, bool nullable, ClassName foreignClass, SupportClass.SetSupport foreignKeys, MultiMap metaattribs):base(element, parent)
		{
			initWith(name, type, implementationClassName, nullable, id, false, foreignClass, foreignKeys, metaattribs);
		}
		
		protected internal virtual void  initWith(String name, ClassName type, ClassName implementationClassName, bool nullable, bool id, bool generated, ClassName foreignClass, SupportClass.SetSupport foreignKeys, MultiMap metaattribs)
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
			if (fieldName.Substring(0,1) == fieldName.Substring(0,1).ToLower())
				log.Warn("Nonstandard naming convention found on " + fieldName);
		}
		
		/// <summary> foo -> Foo
		/// FOo -> FOo
		/// 
		/// </summary>
		/// <returns>
		/// </returns>
		private String beancapitalize(String fieldname)
		{
			if ((Object) fieldname == null || fieldname.Length == 0)
			{
				return fieldname;
			}
			
			if (fieldname.Length > 1 && Char.IsUpper(fieldname[1]) && Char.IsUpper(fieldname[0]))
			{
				return fieldname;
			}
			char[] chars = fieldname.ToCharArray();
			chars[0] = Char.ToUpper(chars[0]);
			return new String(chars);
		}
		
		public override String ToString()
		{
			return FullyQualifiedTypeName + ":" + FieldName;
		}
		
		public virtual String getScope(String localScopeName, String defaultScope)
		{
			if ((Object) defaultScope == null)
				defaultScope = "private";
			return (getMeta(localScopeName) == null)?defaultScope:getMetaAsString(localScopeName);
		}
	}
}