using System;
using CompositeUserType = NHibernate.ICompositeUserType;
using UserType = NHibernate.IUserType;
using PrimitiveType = NHibernate.Type.PrimitiveType;
using Type = NHibernate.Type.TypeType;
using TypeFactory = NHibernate.Type.TypeFactory;
using ReflectHelper = NHibernate.Util.ReflectHelper;
using StringHelper = NHibernate.Util.StringHelper;
using Element = System.Xml.XmlElement;
using MultiMap = System.Collections.Hashtable;
using System.Xml;
using IType = NHibernate.Type.IType;

namespace NHibernate.tool.hbm2net
{
	
	
	public class ClassMapping:MappingElement
	{
		private void  InitBlock()
		{
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			fields = new SupportClass.ListCollectionSupport();
			imports = new SupportClass.TreeSetSupport();
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			subclasses = new SupportClass.ListCollectionSupport();
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport Fields
		{
			get
			{
				return fields;
			}
			
		}
		virtual public SupportClass.SetSupport Imports
		{
			get
			{
				return imports;
			}
			
		}
		/// <summary>shorthand method for getClassName().getFullyQualifiedName() </summary>
		virtual public System.String FullyQualifiedName
		{
			get
			{
				return ClassName.FullyQualifiedName;
			}
			
		}
		/// <summary>shorthand method for getClassName().getName() </summary>
		virtual public System.String Name
		{
			get
			{
				return ClassName.Name;
			}
			
		}
		/// <summary>shorthand method for getClassName().getPackageName() </summary>
		virtual public System.String PackageName
		{
			get
			{
				return ClassName.PackageName;
			}
			
		}
		virtual public ClassName ClassName
		{
			get
			{
				return name;
			}
			
		}
		virtual public System.String GeneratedName
		{
			get
			{
				return generatedName.Name;
			}
			
		}
		virtual public System.String GeneratedPackageName
		{
			get
			{
				return generatedName.PackageName;
			}
			
		}
		virtual public System.String Proxy
		{
			get
			{
				return proxyClass;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport Subclasses
		{
			get
			{
				return subclasses;
			}
			
		}
		virtual public System.String SuperClass
		{
			get
			{
				return superClass;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport LocalFieldsForFullConstructor
		{
			get
			{
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport result = new SupportClass.ListCollectionSupport();
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator myFields = Fields.GetEnumerator(); myFields.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					FieldProperty field = (FieldProperty) myFields.Current;
					if (!field.Identifier || (field.Identifier && !field.Generated))
					{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
						result.Add(field);
					}
				}
				
				return result;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport FieldsForSupersFullConstructor
		{
			get
			{
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport result = new SupportClass.ListCollectionSupport();
				if (SuperClassMapping != null)
				{
					// The correct sequence is vital here, as the subclass should be
					// able to invoke the fullconstructor based on the sequence returned
					// by this method!
					result.AddAll(SuperClassMapping.FieldsForSupersFullConstructor);
					result.AddAll(SuperClassMapping.LocalFieldsForFullConstructor);
				}
				
				return result;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport LocalFieldsForMinimalConstructor
		{
			get
			{
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport result = new SupportClass.ListCollectionSupport();
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator myFields = Fields.GetEnumerator(); myFields.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					FieldProperty field = (FieldProperty) myFields.Current;
					if ((!field.Identifier && !field.Nullable) || (field.Identifier && !field.Generated))
					{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
						result.Add(field);
					}
				}
				return result;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport AllFields
		{
			get
			{
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport result = new SupportClass.ListCollectionSupport();
				
				if (SuperClassMapping != null)
				{
					result.AddAll(SuperClassMapping.AllFields);
				}
				else
				{
					result.AddAll(Fields);
				}
				return result;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport AllFieldsForFullConstructor
		{
			get
			{
				
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport result = FieldsForSupersFullConstructor;
				result.AddAll(LocalFieldsForFullConstructor);
				return result;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport FieldsForSupersMinimalConstructor
		{
			get
			{
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport result = new SupportClass.ListCollectionSupport();
				if (SuperClassMapping != null)
				{
					// The correct sequence is vital here, as the subclass should be
					// able to invoke the fullconstructor based on the sequence returned
					// by this method!
					result.AddAll(SuperClassMapping.FieldsForSupersMinimalConstructor);
					result.AddAll(SuperClassMapping.LocalFieldsForMinimalConstructor);
				}
				
				return result;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport AllFieldsForMinimalConstructor
		{
			get
			{
				
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport result = FieldsForSupersMinimalConstructor;
				result.AddAll(LocalFieldsForMinimalConstructor);
				return result;
			}
			
		}
		public static System.Collections.IEnumerator Components
		{
			get
			{
				return components.Values.GetEnumerator();
			}
			
		}
		/// <summary> Returns the superClassMapping.</summary>
		/// <returns> ClassMapping
		/// </returns>
		virtual public ClassMapping SuperClassMapping
		{
			get
			{
				return superClassMapping;
			}
			
		}
		virtual public bool Interface
		{
			get
			{
				return getMetaAsBool("interface");
			}
			
		}
		/// <returns>
		/// </returns>
		virtual public System.String Scope
		{
			get
			{
				System.String classScope = "public";
				if (getMeta("scope-class") != null)
				{
					classScope = getMetaAsString("scope-class").Trim();
				}
				return classScope;
			}
			
		}
		/// <returns>
		/// </returns>
		virtual public System.String DeclarationType
		{
			get
			{
				if (Interface)
				{
					return "interface";
				}
				else
				{
					return "class";
				}
			}
			
		}
		/// <summary> Return the modifers for this class.
		/// Adds "abstract" if class should be abstract (but not if scope contains abstract)
		/// TODO: deprecate/remove scope-class and introduce class-modifier instead
		/// </summary>
		/// <returns>
		/// </returns>
		virtual public System.String Modifiers
		{
			get
			{
				if (shouldBeAbstract() && (Scope.IndexOf("abstract") == - 1))
				{
					return "abstract";
				}
				else
				{
					return "";
				}
			}
			
		}
		/// <returns>
		/// </returns>
		virtual public bool SuperInterface
		{
			get
			{
				return SuperClassMapping == null?false:SuperClassMapping.Interface;
			}
			
		}
		
		private ClassName name = null;
		private ClassName generatedName = null;
		private System.String superClass = null;
		private ClassMapping superClassMapping = null;
		private System.String proxyClass = null;
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'fields' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private SupportClass.ListCollectionSupport fields;
		//UPGRADE_NOTE: The initialization of  'imports' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private SupportClass.TreeSetSupport imports;
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'subclasses' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private SupportClass.ListCollectionSupport subclasses;
		//UPGRADE_NOTE: Final was removed from the declaration of 'components '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
		//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilHashMap"'
		private static readonly System.Collections.IDictionary components = new System.Collections.Hashtable();
		private bool mustImplementEquals_Renamed_Field = false;
		
		private bool shouldBeAbstract_Renamed_Field = false;
		
		public ClassMapping(System.String classPackage, MappingElement parentElement, ClassName superClass, ClassMapping superClassMapping, Element classElement, MultiMap inheritedMeta):this(classPackage, parentElement, superClass, classElement, inheritedMeta)
		{
			
			this.superClassMapping = superClassMapping;
			
			if (this.superClassMapping != null)
			{
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport l = this.superClassMapping.AllFieldsForFullConstructor;
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator iter = l.GetEnumerator(); iter.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					FieldProperty element = (FieldProperty) iter.Current;
					ClassName ct = element.ClassType;
					if (ct != null)
					{
						// add imports for superclasses possible fields.
						addImport(ct);
					}
					else
					{
						addImport(element.FullyQualifiedTypeName);
					}
				}
			}
		}
		
		public ClassMapping(System.String classPackage, MappingElement parentElement, ClassName superClass, Element classElement, MultiMap inheritedMeta):base(classElement, parentElement)
		{
			InitBlock();
			initWith(classPackage, superClass, classElement, false, inheritedMeta);
		}
		
		public ClassMapping(System.String classPackage, Element classElement, MappingElement parentElement, MultiMap inheritedMeta):base(classElement, parentElement)
		{
			InitBlock();
			initWith(classPackage, null, classElement, false, inheritedMeta);
		}
		
		public ClassMapping(System.String classPackage, Element classElement, MappingElement parentElement, bool component, MultiMap inheritedMeta):base(classElement, parentElement)
		{
			InitBlock();
			initWith(classPackage, null, classElement, component, inheritedMeta);
		}
		
		protected internal virtual void  initWith(System.String classPackage, ClassName mySuperClass, Element classElement, bool component, MultiMap inheritedMeta)
		{
			
			System.String fullyQualifiedName = classElement.Attributes[component?"class":"name"].Value;
			
			if (fullyQualifiedName.IndexOf((System.Char) '.') < 0 && (System.Object) classPackage != null && classPackage.Trim().Length > 0)
			{
				fullyQualifiedName = classPackage + "." + fullyQualifiedName;
			}
			//log.debug("Processing mapping for class: " + fullyQualifiedName);
			
			MetaAttribs = MetaAttributeHelper.loadAndMergeMetaMap(classElement, inheritedMeta);
			
			//    class & package names
			name = new ClassName(fullyQualifiedName);
			
			if (getMeta("generated-class") != null)
			{
				generatedName = new ClassName(getMetaAsString("generated-class").Trim());
				shouldBeAbstract_Renamed_Field = true;
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				//log.warn("Generating " + generatedName + " instead of " + name);
			}
			else
			{
				generatedName = name;
			}
			
			if (mySuperClass != null)
			{
				this.superClass = mySuperClass.Name;
				addImport(mySuperClass); // can only be done AFTER this class gets its own name.
			}
			
			
			// get the properties defined for this class
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport propertyList = new SupportClass.ListCollectionSupport();
			propertyList.AddAll(classElement.GetElementsByTagName("property"));
			propertyList.AddAll(classElement.GetElementsByTagName("version"));
			propertyList.AddAll(classElement.GetElementsByTagName("timestamp"));
			propertyList.AddAll(classElement.GetElementsByTagName("key-property"));
			propertyList.AddAll(classElement.GetElementsByTagName("any"));
			
			// get all many-to-one associations defined for the class
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport manyToOneList = new SupportClass.ListCollectionSupport();
			manyToOneList.AddAll(classElement.GetElementsByTagName("many-to-one"));
			manyToOneList.AddAll(classElement.GetElementsByTagName("key-many-to-one"));
			
			XmlAttribute att = classElement.Attributes["proxy"];
			if (att != null)
				proxyClass = att.Value;
			
			Element id = classElement["id"];
			
			if (id != null)
			{
				propertyList.Insert(0, id);
				// implementEquals();
			}
			
			// composite id
			Element cmpid = classElement["composite-id"];
			if (cmpid != null)
			{
				implementEquals();
				System.String cmpname = cmpid.Attributes["name"].Value;
				System.String cmpclass = cmpid.Attributes["class"].Value;
				if ((System.Object) cmpclass == null || cmpclass.Equals(string.Empty))
				{
					//Embedded composite id
					//implementEquals();
					propertyList.AddAll(0, cmpid.GetElementsByTagName("key-property"));
					manyToOneList.AddAll(0, cmpid.GetElementsByTagName("key-many-to-one"));
				}
				else
				{
					//Composite id class
					ClassMapping mapping = new ClassMapping(classPackage, cmpid, this, true, MetaAttribs);
					MultiMap metaForCompositeid = MetaAttributeHelper.loadAndMergeMetaMap(cmpid, MetaAttribs);
					mapping.implementEquals();
					ClassName classType = new ClassName(cmpclass);
					// add an import and field for this property
					addImport(classType);
					FieldProperty cmpidfield = new FieldProperty(cmpid, this, cmpname, classType, false, true, false, metaForCompositeid);
					addFieldProperty(cmpidfield);
					object tempObject;
					tempObject = mapping;
					components[mapping.FullyQualifiedName] = tempObject;
					System.Object generatedAux = tempObject;
				}
			}
			
			// checked after the default sets of implement equals.
			if (getMetaAsBool("implement-equals"))
			{
				implementEquals();
			}
			
			// derive the class imports and fields from the properties
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator properties = propertyList.GetEnumerator(); properties.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element property = (Element) properties.Current;
				
				MultiMap metaForProperty = MetaAttributeHelper.loadAndMergeMetaMap(property, MetaAttribs);
				System.String propertyName = (property.Attributes["name"] == null?null:property.Attributes["name"].Value);
				if ((System.Object) propertyName == null || propertyName.Trim().Equals(string.Empty))
				{
					continue; //since an id doesn't necessarily need a name
				}
				
				// ensure that the type is specified
				System.String type = (property.Attributes["type"] == null?null:property.Attributes["type"].Value);
				if ((System.Object) type == null && cmpid != null)
				{
					// for composite-keys
					type = (property.Attributes["class"] == null?null:property.Attributes["class"].Value);
				}
				if ("timestamp".Equals(property.LocalName))
				{
					type = "System.DateTime";
				}
				
				if ("any".Equals(property.LocalName))
				{
					type = "System.Object";
				}
				
				if ((System.Object) type == null || type.Trim().Equals(string.Empty))
				{
					//log.warn("property \"" + propertyName + "\" in class " + Name + " is missing a type attribute");
					continue;
				}
				
				
				// handle in a different way id and properties...
				// ids may be generated and may need to be of object type in order to support
				// the unsaved-value "null" value.
				// Properties may be nullable (ids may not)
				if (property == id)
				{
					Element generator = property["generator"];
					System.String unsavedValue = (property.Attributes["unsaved-value"] == null?null:property.Attributes["unsaved-value"].Value);
					bool mustBeNullable = ((System.Object) unsavedValue != null && unsavedValue.Equals("null"));
					bool generated = !(generator.Attributes["class",""] == null?string.Empty:generator.Attributes["class",""].Value).Equals("assigned");
					ClassName rtype = getFieldType(type, mustBeNullable, false);
					addImport(rtype);
					FieldProperty idField = new FieldProperty(property, this, propertyName, rtype, false, true, generated, metaForProperty);
					addFieldProperty(idField);
				}
				else
				{
					System.String notnull = (property.Attributes["not-null"] == null?null:property.Attributes["not-null"].Value);
					// if not-null property is missing lets see if it has been
					// defined at column level
					if ((System.Object) notnull == null)
					{
						Element column = property["column"];
						if (column != null)
							notnull = (column.Attributes["not-null"] == null?null:column.Attributes["not-null"].Value);
					}
					bool nullable = ((System.Object) notnull == null || notnull.Equals("false"));
					bool key = property.LocalName.StartsWith("key-"); //a composite id property
					ClassName t = getFieldType(type);
					addImport(t);
					FieldProperty stdField = new FieldProperty(property, this, propertyName, t, nullable && !key, key, false, metaForProperty);
					addFieldProperty(stdField);
				}
			}
			
			// one to ones
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator onetoones = classElement.GetElementsByTagName("one-to-one").GetEnumerator(); onetoones.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element onetoone = (Element) onetoones.Current;
				
				MultiMap metaForOneToOne = MetaAttributeHelper.loadAndMergeMetaMap(onetoone, MetaAttribs);
				System.String propertyName = onetoone.Attributes["name"].Value;
				
				// ensure that the class is specified
				System.String clazz = onetoone.Attributes["class"].Value;
				if (clazz.Length == 0)
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					//log.warn("one-to-one \"" + name + "\" in class " + Name + " is missing a class attribute");
					continue;
				}
				ClassName cn = getFieldType(clazz);
				addImport(cn);
				FieldProperty fm = new FieldProperty(onetoone, this, propertyName, cn, true, metaForOneToOne);
				addFieldProperty(fm);
			}
			
			// many to ones - TODO: consolidate with code above
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator manytoOnes = manyToOneList.GetEnumerator(); manytoOnes.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element manyToOne = (Element) manytoOnes.Current;
				
				MultiMap metaForManyToOne = MetaAttributeHelper.loadAndMergeMetaMap(manyToOne, MetaAttribs);
				System.String propertyName = manyToOne.Attributes["name"].Value;
				
				// ensure that the type is specified
				System.String type = manyToOne.Attributes["class"].Value;
				if (type.Length == 0)
				{
					//log.warn("many-to-one \"" + propertyName + "\" in class " + Name + " is missing a class attribute");
					continue;
				}
				ClassName classType = new ClassName(type);
				
				// is it nullable?
				System.String notnull = manyToOne.Attributes["not-null"].Value;
				bool nullable = ((System.Object) notnull == null || notnull.Equals("false"));
				bool key = manyToOne.LocalName.StartsWith("key-"); //a composite id property
				
				// add an import and field for this property
				addImport(classType);
				FieldProperty f = new FieldProperty(manyToOne, this, propertyName, classType, nullable && !key, key, false, metaForManyToOne);
				addFieldProperty(f);
			}
			
			// collections
			doCollections(classPackage, classElement, "list", "System.Collections.ICollection", "System.Collections.ArrayList", MetaAttribs);
			doCollections(classPackage, classElement, "map", "System.Collections.IDictionary", "System.Collections.Hashtable", MetaAttribs);
			doCollections(classPackage, classElement, "set", "System.Collections.IDictionary", "System.Collections.Hashtable", MetaAttribs);
			//UPGRADE_ISSUE: Method 'java.lang.System.getProperty' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1000_javalangSystemgetProperty_javalangString_javalangString"'
			doCollections(classPackage, classElement, "bag", "java.util.List", "java.util.ArrayList", MetaAttribs);
			//UPGRADE_ISSUE: Method 'java.lang.System.getProperty' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1000_javalangSystemgetProperty_javalangString_javalangString"'
			doCollections(classPackage, classElement, "idbag", "java.util.List", "java.util.ArrayList", MetaAttribs);
			doArrays(classElement, "array", MetaAttribs);
			doArrays(classElement, "primitive-array", MetaAttribs);
			
			
			
			
			//components
			
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator iter = classElement.GetElementsByTagName("component").GetEnumerator(); iter.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element cmpe = (Element) iter.Current;
				MultiMap metaForComponent = MetaAttributeHelper.loadAndMergeMetaMap(cmpe, MetaAttribs);
				System.String cmpname = cmpe.Attributes["name"].Value;
				System.String cmpclass = cmpe.Attributes["class"].Value;
				if ((System.Object) cmpclass == null || cmpclass.Equals(string.Empty))
				{
					//log.warn("component \"" + cmpname + "\" in class " + Name + " does not specify a class");
					continue;
				}
				ClassMapping mapping = new ClassMapping(classPackage, cmpe, this, true, MetaAttribs);
				
				ClassName classType = new ClassName(cmpclass);
				// add an import and field for this property
				addImport(classType);
				FieldProperty ff = new FieldProperty(cmpe, this, cmpname, classType, false, metaForComponent);
				addFieldProperty(ff);
				object tempObject2;
				tempObject2 = mapping;
				components[mapping.FullyQualifiedName] = tempObject2;
				System.Object generatedAux2 = tempObject2;
			}
			
			
			//    subclasses (done last so they can access this superclass for info)
			
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator iter = classElement.GetElementsByTagName("subclass").GetEnumerator(); iter.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element subclass = (Element) iter.Current;
				ClassMapping subclassMapping = new ClassMapping(classPackage, this, name, this, subclass, MetaAttribs);
				addSubClass(subclassMapping);
			}
			
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator iter = classElement.GetElementsByTagName("joined-subclass").GetEnumerator(); iter.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element subclass = (Element) iter.Current;
				ClassMapping subclassMapping = new ClassMapping(classPackage, this, name, this, subclass, MetaAttribs);
				addSubClass(subclassMapping);
			}
			
			validateMetaAttribs();
		}
		
		
		private void  addFieldProperty(FieldProperty fieldProperty)
		{
			if (fieldProperty.ParentClass == null)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				fields.Add(fieldProperty);
			}
			else
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				throw new System.SystemException("Field " + fieldProperty + " is already associated with a class: " + fieldProperty.ParentClass);
			}
		}
		
		
		public virtual void  implementEquals()
		{
			mustImplementEquals_Renamed_Field = true;
		}
		
		public virtual bool mustImplementEquals()
		{
			return (!Interface) && mustImplementEquals_Renamed_Field;
		}
		
		
		// We need a minimal constructor only if it's different from
		// the full constructor or the no-arg constructor.
		// A minimal construtor is one that lets
		// you specify only the required fields.
		public virtual bool needsMinimalConstructor()
		{
			return (AllFieldsForFullConstructor.Count != AllFieldsForMinimalConstructor.Count) && AllFieldsForMinimalConstructor.Count > 0;
		}
		
		public virtual void  addImport(ClassName className)
		{
			// if the package is java.lang or our own package don't add
			if (!className.inJavaLang() && !className.inSamePackage(generatedName) && !className.Primitive)
			{
				if (className.Array)
				{
					imports.Add(className.FullyQualifiedName.Substring(0, (className.FullyQualifiedName.Length - 2) - (0))); // remove []
				}
				else
				{
					imports.Add(className.FullyQualifiedName);
				}
			}
		}
		
		public virtual void  addImport(System.String className)
		{
			ClassName cn = new ClassName(className);
			addImport(cn);
		}
		
		private void  doCollections(System.String classPackage, Element classElement, System.String xmlName, System.String interfaceClass, System.String implementingClass, MultiMap inheritedMeta)
		{
			
			System.String originalInterface = interfaceClass;
			System.String originalImplementation = implementingClass;
			
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator collections = classElement.GetElementsByTagName(xmlName).GetEnumerator(); collections.MoveNext(); )
			{
				
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element collection = (Element) collections.Current;
				MultiMap metaForCollection = MetaAttributeHelper.loadAndMergeMetaMap(collection, inheritedMeta);
				System.String propertyName = collection.Attributes["name"].Value;
				
				//		Small hack to switch over to sortedSet/sortedMap if sort is specified. (that is sort != unsorted)
				System.String sortValue = (collection.Attributes["sort"] == null?null:collection.Attributes["sort"].Value);
				if ((System.Object) sortValue != null && !"unsorted".Equals(sortValue) && !"".Equals(sortValue.Trim()))
				{
					if ("map".Equals(xmlName))
					{
						//UPGRADE_TODO: Interface 'java.util.SortedMap' was converted to 'System.Collections.SortedList' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
						interfaceClass = typeof(System.Collections.SortedList).FullName;
						//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.TreeMap' and 'System.Collections.SortedList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
						implementingClass = typeof(System.Collections.SortedList).FullName;
					}
					else if ("set".Equals(xmlName))
					{
						interfaceClass = typeof(SupportClass.SortedSetSupport).FullName;
						implementingClass = typeof(SupportClass.TreeSetSupport).FullName;
					}
				}
				else
				{
					interfaceClass = originalInterface;
					implementingClass = originalImplementation;
				}
				
				ClassName interfaceClassName = new ClassName(interfaceClass);
				ClassName implementationClassName = new ClassName(implementingClass);
				
				// add an import and field for this collection
				addImport(interfaceClassName);
				// import implementingClassName should only be 
				// added if the initialisaiton code of the field 
				// is actually used - and currently it isn't!
				//addImport(implementingClassName);
				
				
				ClassName foreignClass = null;
				SupportClass.SetSupport foreignKeys = null;
				// Collect bidirectional data
				if (collection.GetElementsByTagName("one-to-many").Count != 0)
				{
					foreignClass = new ClassName(collection["one-to-many"].Attributes["class"].Value);
				}
				else if (collection.GetElementsByTagName("many-to-many").Count != 0)
				{
					foreignClass = new ClassName(collection["many-to-many"].Attributes["class"].Value);
				}
				
				// Do the foreign keys and import
				if (foreignClass != null)
				{
					// Collect the keys
					foreignKeys = new SupportClass.HashSetSupport();
					foreignKeys.Add(collection["key"].Attributes["column"].Value);
					
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
					for (System.Collections.IEnumerator iter = collection["key"].GetElementsByTagName("column").GetEnumerator(); iter.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
						foreignKeys.Add(((Element) iter.Current).Attributes["name"].Value);
					}
					
					//addImport(foreignClass);
				}
				FieldProperty cf = new FieldProperty(collection, this, propertyName, interfaceClassName, implementationClassName, false, foreignClass, foreignKeys, metaForCollection);
				addFieldProperty(cf);
				if (collection.GetElementsByTagName("composite-element").Count != 0)
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
					for (System.Collections.IEnumerator compositeElements = collection.GetElementsByTagName("composite-element").GetEnumerator(); compositeElements.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
						Element compositeElement = (Element) compositeElements.Current;
						System.String compClass = compositeElement.Attributes["class"].Value;
						
						try
						{
							ClassMapping mapping = new ClassMapping(classPackage, compositeElement, this, true, MetaAttribs);
							ClassName classType = new ClassName(compClass);
							// add an import and field for this property
							addImport(classType);
							object tempObject;
							tempObject = mapping;
							components[mapping.FullyQualifiedName] = tempObject;
							System.Object generatedAux = tempObject;
						}
						catch (System.Exception e)
						{
							//log.error("Error building composite-element " + compClass, e);
						}
					}
				}
			}
		}
		
		private void  doArrays(Element classElement, System.String type, MultiMap inheritedMeta)
		{
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator arrays = classElement.GetElementsByTagName(type).GetEnumerator(); arrays.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element array = (Element) arrays.Current;
				MultiMap metaForArray = MetaAttributeHelper.loadAndMergeMetaMap(array, inheritedMeta);
				System.String role = array.Attributes["name"].Value;
				System.String elementClass = (array.Attributes["element-class"] == null?null:array.Attributes["element-class"].Value);
				if ((System.Object) elementClass == null)
				{
					Element elt = array["element"];
					if (elt == null)
						elt = array["one-to-many"];
					if (elt == null)
						elt = array["many-to-many"];
					if (elt == null)
						elt = array["composite-element"];
					if (elt == null)
					{
						//log.warn("skipping collection with subcollections");
						continue;
					}
					elementClass = elt.Attributes["type"].Value;
					if ((System.Object) elementClass == null)
						elementClass = elt.Attributes["class"].Value;
				}
				ClassName cn = getFieldType(elementClass, false, true);
				
				addImport(cn);
				FieldProperty af = new FieldProperty(array, this, role, cn, false, metaForArray);
				addFieldProperty(af);
			}
		}
		
		private ClassName getFieldType(System.String hibernateType)
		{
			return getFieldType(hibernateType, false, false);
		}
		
		/// <summary> Return a ClassName for a hibernatetype.
		/// 
		/// </summary>
		/// <param name="hibernateType">Name of the hibernatetype (e.g. "binary")
		/// </param>
		/// <param name="needObject">
		/// </param>
		/// <param name="isArray">if the type should be postfixed with array brackes ("[]")
		/// </param>
		/// <returns>
		/// </returns>
		private ClassName getFieldType(System.String hibernateType, bool mustBeNullable, bool isArray)
		{
			System.String postfix = isArray?"[]":"";
			// deal with hibernate binary type
			ClassName cn = null;
			if (hibernateType.Equals("binary"))
			{
				cn = new ClassName("byte[]" + postfix);
				return cn;
			}
			else
			{
				IType basicType = TypeFactory.Basic(hibernateType);
				if (basicType != null)
				{
					
					if ((basicType is PrimitiveType) && !hibernateType.Trim().Equals(basicType.ReturnedClass.Name) && !mustBeNullable)
					{
						cn = new ClassName(((PrimitiveType) basicType).PrimitiveClass.Name + postfix);
						return cn;
					}
					else
					{
						cn = new ClassName(basicType.ReturnedClass.Name + postfix);
						return cn;
					}
				}
				else
				{
					// check and resolve correct type if it is an usertype
					hibernateType = getTypeForUserType(hibernateType);
					cn = new ClassName(hibernateType + postfix);
					// add an import and field for this property
					addImport(cn);
					return cn;
				}
			}
		}
		
		/// <summary>Returns name of returnedclass if type is an UserType *</summary>
		private System.String getTypeForUserType(System.String type)
		{
			System.Type clazz = null;
			try
			{
				if (type.IndexOf("(")>0)
					type = type.Substring(0, type.IndexOf("("));
				clazz = ReflectHelper.ClassForName(type);
				
				if (typeof(UserType).IsAssignableFrom(clazz))
				{
					//UPGRADE_TODO: Method 'java.lang.Class.newInstance' was converted to 'SupportClass.CreateNewInstance' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
					//UPGRADE_WARNING: Method 'java.lang.Class.newInstance' was converted to 'SupportClass.CreateNewInstance' which may throw an exception. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1101"'
					UserType ut = (UserType) SupportClass.CreateNewInstance(clazz);
					//log.debug("Resolved usertype: " + type + " to " + ut.ReturnedType.Name);
					System.String t = clazzToName(ut.ReturnedType);
					return t;
				}
				
				if (typeof(CompositeUserType).IsAssignableFrom(clazz))
				{
					//UPGRADE_TODO: Method 'java.lang.Class.newInstance' was converted to 'SupportClass.CreateNewInstance' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
					//UPGRADE_WARNING: Method 'java.lang.Class.newInstance' was converted to 'SupportClass.CreateNewInstance' which may throw an exception. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1101"'
					CompositeUserType ut = (CompositeUserType) SupportClass.CreateNewInstance(clazz);
					//log.debug("Resolved composite usertype: " + type + " to " + ut.ReturnedClass.Name);
					System.String t = clazzToName(ut.ReturnedClass);
					return t;
				}
			}
			//UPGRADE_NOTE: Exception 'java.lang.ClassNotFoundException' was converted to 'System.Exception' which has different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1100"'
			catch (System.IO.FileNotFoundException e)
			{
				if (type.IndexOf(",")>0)
					type = type.Substring(0, type.IndexOf(","));
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				//log.warn("Could not find UserType: " + type + ". Using the type '" + type + "' directly instead. (" + e.ToString() + ")");
			}
			catch (System.UnauthorizedAccessException iae)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				//log.warn("Error while trying to resolve UserType. Using the type '" + type + "' directly instead. (" + iae.ToString() + ")");
			}
			//UPGRADE_NOTE: Exception 'java.lang.InstantiationException' was converted to 'System.Exception' which has different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1100"'
			catch (System.Exception e)
			{
				Console.WriteLine(e);
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				//log.warn("Error while trying to resolve UserType. Using the type '" + type + "' directly instead. (" + e.ToString() + ")");
			}
			
			return type;
		}
		
		private System.String clazzToName(System.Type cl)
		{
			System.String s = null;
			
			if (cl.IsArray)
			{
				s = clazzToName(cl.GetElementType()) + "[]";
			}
			else
			{
				s = cl.FullName;
			}
			
			return s;
		}
		
		
		/// <summary> Method shouldBeAbstract.</summary>
		/// <returns> boolean
		/// </returns>
		public virtual bool shouldBeAbstract()
		{
			return shouldBeAbstract_Renamed_Field;
		}
		
		// Based on some raw heuristics the following method validates the provided metaattribs.
		internal virtual void  validateMetaAttribs()
		{
			// Inform that "extends" is not used if this one is a genuine subclass
			if ((System.Object) SuperClass != null && getMeta("extends") != null)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				//log.warn("Warning: meta attribute extends='" + getMetaAsString("extends") + "' will be ignored for subclass " + name);
			}
		}
		
		/// <seealso cref="java.lang.Object#toString()">
		/// </seealso>
		public override System.String ToString()
		{
			return "ClassMapping: " + name.FullyQualifiedName;
		}
		
		/// <param name="">subclassMapping
		/// </param>
		public virtual void  addSubClass(ClassMapping subclassMapping)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			subclasses.Add(subclassMapping);
		}
		
		public virtual void  addImport(System.Type clazz)
		{
			addImport(clazz.FullName);
		}
		static ClassMapping()
		{
			//log = LogFactory.getLog(typeof(ClassMapping));
		}
	}
}