using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using NHibernate.Type;
using NHibernate.Util;
using CompositeUserType = NHibernate.ICompositeUserType;
using UserType = NHibernate.IUserType;
using Type = NHibernate.Type.TypeType;
using Element = System.Xml.XmlElement;
using MultiMap = System.Collections.Hashtable;

namespace NHibernate.Tool.hbm2net
{
	
	
	public class ClassMapping:MappingElement
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private void  InitBlock()
		{
			fields = new SupportClass.ListCollectionSupport();
			imports = new SupportClass.TreeSetSupport();
			subclasses = new SupportClass.ListCollectionSupport();
		}

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
		virtual public String FullyQualifiedName
		{
			get
			{
				return ClassName.FullyQualifiedName;
			}
			
		}
		/// <summary>shorthand method for getClassName().getName() </summary>
		virtual public String Name
		{
			get
			{
				return ClassName.Name;
			}
			
		}
		/// <summary>shorthand method for getClassName().getPackageName() </summary>
		virtual public String PackageName
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
		virtual public String GeneratedName
		{
			get
			{
				return generatedName.Name;
			}
			
		}
		virtual public String GeneratedPackageName
		{
			get
			{
				return generatedName.PackageName;
			}
			
		}
		virtual public String Proxy
		{
			get
			{
				return proxyClass;
			}
			
		}
		virtual public SupportClass.ListCollectionSupport Subclasses
		{
			get
			{
				return subclasses;
			}
			
		}
		virtual public String SuperClass
		{
			get
			{
				return superClass;
			}
			
		}
		virtual public SupportClass.ListCollectionSupport LocalFieldsForFullConstructor
		{
			get
			{
				SupportClass.ListCollectionSupport result = new SupportClass.ListCollectionSupport();
				for (IEnumerator myFields = Fields.GetEnumerator(); myFields.MoveNext(); )
				{
					FieldProperty field = (FieldProperty) myFields.Current;
					if (!field.Identifier || (field.Identifier && !field.Generated))
					{
						result.Add(field);
					}
				}
				
				return result;
			}
			
		}
		virtual public SupportClass.ListCollectionSupport FieldsForSupersFullConstructor
		{
			get
			{
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
		virtual public SupportClass.ListCollectionSupport LocalFieldsForMinimalConstructor
		{
			get
			{
				SupportClass.ListCollectionSupport result = new SupportClass.ListCollectionSupport();
				for (IEnumerator myFields = Fields.GetEnumerator(); myFields.MoveNext(); )
				{
					FieldProperty field = (FieldProperty) myFields.Current;
					if ((!field.Identifier && !field.Nullable) || (field.Identifier && !field.Generated))
					{
						result.Add(field);
					}
				}
				return result;
			}
			
		}
		virtual public SupportClass.ListCollectionSupport AllFields
		{
			get
			{
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
		virtual public SupportClass.ListCollectionSupport AllFieldsForFullConstructor
		{
			get
			{
				
				SupportClass.ListCollectionSupport result = FieldsForSupersFullConstructor;
				result.AddAll(LocalFieldsForFullConstructor);
				return result;
			}
			
		}
		virtual public SupportClass.ListCollectionSupport FieldsForSupersMinimalConstructor
		{
			get
			{
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
		virtual public SupportClass.ListCollectionSupport AllFieldsForMinimalConstructor
		{
			get
			{
				
				SupportClass.ListCollectionSupport result = FieldsForSupersMinimalConstructor;
				result.AddAll(LocalFieldsForMinimalConstructor);
				return result;
			}
			
		}
		public static IEnumerator Components
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
		virtual public String Scope
		{
			get
			{
				String classScope = "public";
				if (getMeta("scope-class") != null)
				{
					classScope = getMetaAsString("scope-class").Trim();
				}
				return classScope;
			}
			
		}
		/// <returns>
		/// </returns>
		virtual public String DeclarationType
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
		virtual public String Modifiers
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
		private String superClass = null;
		private ClassMapping superClassMapping = null;
		private String proxyClass = null;
		private SupportClass.ListCollectionSupport fields;
		private SupportClass.TreeSetSupport imports;
		private SupportClass.ListCollectionSupport subclasses;
		private static readonly IDictionary components = new Hashtable();
		private bool mustImplementEquals_Renamed_Field = false;
		
		private bool shouldBeAbstract_Renamed_Field = false;
		
		public ClassMapping(String classPackage, MappingElement parentElement, ClassName superClass, ClassMapping superClassMapping, Element classElement, MultiMap inheritedMeta):this(classPackage, parentElement, superClass, classElement, inheritedMeta)
		{
			
			this.superClassMapping = superClassMapping;
			
			if (this.superClassMapping != null)
			{
				SupportClass.ListCollectionSupport l = this.superClassMapping.AllFieldsForFullConstructor;
				for (IEnumerator iter = l.GetEnumerator(); iter.MoveNext(); )
				{
					FieldProperty element = (FieldProperty) iter.Current;
					ClassName ct = element.ClassType;
					if (ct != null)
					{
						// add imports for superclasses possible fields.
						//addImport(ct);
					}
					else
					{
						//addImport(element.FullyQualifiedTypeName);
					}
				}
			}
		}
		
		public ClassMapping(String classPackage, MappingElement parentElement, ClassName superClass, Element classElement, MultiMap inheritedMeta):base(classElement, parentElement)
		{
			InitBlock();
			initWith(classPackage, superClass, classElement, false, inheritedMeta);
		}
		
		public ClassMapping(String classPackage, Element classElement, MappingElement parentElement, MultiMap inheritedMeta):base(classElement, parentElement)
		{
			InitBlock();
			initWith(classPackage, null, classElement, false, inheritedMeta);
		}
		
		public ClassMapping(String classPackage, Element classElement, MappingElement parentElement, bool component, MultiMap inheritedMeta):base(classElement, parentElement)
		{
			InitBlock();
			initWith(classPackage, null, classElement, component, inheritedMeta);
		}
		
		protected internal virtual void  initWith(String classPackage, ClassName mySuperClass, Element classElement, bool component, MultiMap inheritedMeta)
		{
			
			String fullyQualifiedName = (classElement.Attributes[component?"class":"name"] == null?string.Empty:classElement.Attributes[component?"class":"name"].Value);
			
			if (fullyQualifiedName.IndexOf('.') < 0 && (Object) classPackage != null && classPackage.Trim().Length > 0)
			{
				fullyQualifiedName = classPackage + "." + fullyQualifiedName;
			}
			log.Debug("Processing mapping for class: " + fullyQualifiedName);
			
			MetaAttribs = MetaAttributeHelper.loadAndMergeMetaMap(classElement, inheritedMeta);
			
			//    class & package names
			name = new ClassName(fullyQualifiedName);
			
			if (getMeta("generated-class") != null)
			{
				generatedName = new ClassName(getMetaAsString("generated-class").Trim());
				shouldBeAbstract_Renamed_Field = true;
				log.Warn("Generating " + generatedName + " instead of " + name);
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
			SupportClass.ListCollectionSupport propertyList = new SupportClass.ListCollectionSupport();
			propertyList.AddAll(classElement.SelectNodes("urn:property", CodeGenerator.nsmgr));
			propertyList.AddAll(classElement.SelectNodes("urn:version", CodeGenerator.nsmgr));
			propertyList.AddAll(classElement.SelectNodes("urn:timestamp", CodeGenerator.nsmgr));
			propertyList.AddAll(classElement.SelectNodes("urn:key-property", CodeGenerator.nsmgr));
			propertyList.AddAll(classElement.SelectNodes("urn:any", CodeGenerator.nsmgr));
			
			// get all many-to-one associations defined for the class
			SupportClass.ListCollectionSupport manyToOneList = new SupportClass.ListCollectionSupport();
			manyToOneList.AddAll(classElement.SelectNodes("urn:many-to-one", CodeGenerator.nsmgr));
			manyToOneList.AddAll(classElement.SelectNodes("urn:key-many-to-one", CodeGenerator.nsmgr));
			
			XmlAttribute att = classElement.Attributes["proxy"];
			if (att != null)
			{
				proxyClass = att.Value;
				if (proxyClass.IndexOf(",")>0)
					proxyClass = proxyClass.Substring(0, proxyClass.IndexOf(","));
			}
			
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
				String cmpname = (cmpid.Attributes["name"] == null?null:cmpid.Attributes["name"].Value);
				String cmpclass = (cmpid.Attributes["class"] == null?null:cmpid.Attributes["class"].Value);
				if ((Object) cmpclass == null || cmpclass.Equals(string.Empty))
				{
					//Embedded composite id
					//implementEquals();
					propertyList.AddAll(0, cmpid.SelectNodes("urn:key-property", CodeGenerator.nsmgr));
					manyToOneList.AddAll(0, cmpid.SelectNodes("urn:key-many-to-one", CodeGenerator.nsmgr));
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
				}
			}
			
			// checked after the default sets of implement equals.
			if (getMetaAsBool("implement-equals"))
			{
				implementEquals();
			}
			
			// derive the class imports and fields from the properties
			for (IEnumerator properties = propertyList.GetEnumerator(); properties.MoveNext(); )
			{
				Element property = (Element) properties.Current;
				
				MultiMap metaForProperty = MetaAttributeHelper.loadAndMergeMetaMap(property, MetaAttribs);
				String propertyName = (property.Attributes["name"] == null?null:property.Attributes["name"].Value);
				if ((Object) propertyName == null || propertyName.Trim().Equals(string.Empty))
				{
					continue; //since an id doesn't necessarily need a name
				}
				
				// ensure that the type is specified
				String type = (property.Attributes["type"] == null?null:property.Attributes["type"].Value);
				if ((Object) type == null && cmpid != null)
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
				
				if ((Object) type == null || type.Trim().Equals(string.Empty))
				{
					if (property == id)
						type = "Int32";
					else
						type = "String";
					log.Warn("property \"" + propertyName + "\" in class " + Name + " is missing a type attribute, guessing " + type);
				}
				
				
				// handle in a different way id and properties...
				// ids may be generated and may need to be of object type in order to support
				// the unsaved-value "null" value.
				// Properties may be nullable (ids may not)
				if (property == id)
				{
					Element generator = property["generator"];
					String unsavedValue = (property.Attributes["unsaved-value"] == null?null:property.Attributes["unsaved-value"].Value);
					bool mustBeNullable = ((Object) unsavedValue != null && unsavedValue.Equals("null"));
					bool generated = !(generator.Attributes["class"] == null?string.Empty:generator.Attributes["class"].Value).Equals("assigned");
					ClassName rtype = getFieldType(type, mustBeNullable, false);
					addImport(rtype);
					FieldProperty idField = new FieldProperty(property, this, propertyName, rtype, false, true, generated, metaForProperty);
					addFieldProperty(idField);
				}
				else
				{
					String notnull = (property.Attributes["not-null"] == null?null:property.Attributes["not-null"].Value);
					// if not-null property is missing lets see if it has been
					// defined at column level
					if ((Object) notnull == null)
					{
						Element column = property["column"];
						if (column != null)
							notnull = (column.Attributes["not-null"] == null?null:column.Attributes["not-null"].Value);
					}
					bool nullable = ((Object) notnull == null || notnull.Equals("false"));
					bool key = property.LocalName.StartsWith("key-"); //a composite id property
					ClassName t = getFieldType(type);
					addImport(t);
					FieldProperty stdField = new FieldProperty(property, this, propertyName, t, nullable && !key, key, false, metaForProperty);
					addFieldProperty(stdField);
				}
			}
			
			// one to ones
			for (IEnumerator onetoones = classElement.SelectNodes("urn:one-to-one", CodeGenerator.nsmgr).GetEnumerator(); onetoones.MoveNext(); )
			{
				Element onetoone = (Element) onetoones.Current;
				
				MultiMap metaForOneToOne = MetaAttributeHelper.loadAndMergeMetaMap(onetoone, MetaAttribs);
				String propertyName = (onetoone.Attributes["name"] == null?string.Empty:onetoone.Attributes["name"].Value);
				
				// ensure that the class is specified
				String clazz = (onetoone.Attributes["class"] == null?string.Empty:onetoone.Attributes["class"].Value);
				if (clazz.Length == 0)
				{
					log.Warn("one-to-one \"" + name + "\" in class " + Name + " is missing a class attribute");
					continue;
				}
				ClassName cn = getFieldType(clazz);
				addImport(cn);
				FieldProperty fm = new FieldProperty(onetoone, this, propertyName, cn, true, metaForOneToOne);
				addFieldProperty(fm);
			}
			
			// many to ones - TODO: consolidate with code above
			for (IEnumerator manytoOnes = manyToOneList.GetEnumerator(); manytoOnes.MoveNext(); )
			{
				Element manyToOne = (Element) manytoOnes.Current;
				
				MultiMap metaForManyToOne = MetaAttributeHelper.loadAndMergeMetaMap(manyToOne, MetaAttribs);
				String propertyName = (manyToOne.Attributes["name"] == null?string.Empty:manyToOne.Attributes["name"].Value);
				
				// ensure that the type is specified
				String type = (manyToOne.Attributes["class"] == null?string.Empty:manyToOne.Attributes["class"].Value);
				if (type.Length == 0)
				{
					log.Warn("many-to-one \"" + propertyName + "\" in class " + Name + " is missing a class attribute");
					continue;
				}
				ClassName classType = new ClassName(type);
				
				// is it nullable?
				String notnull = (manyToOne.Attributes["not-null"] == null?null:manyToOne.Attributes["not-null"].Value);
				bool nullable = ((Object) notnull == null || notnull.Equals("false"));
				bool key = manyToOne.LocalName.StartsWith("key-"); //a composite id property
				
				// add an import and field for this property
				addImport(classType);
				FieldProperty f = new FieldProperty(manyToOne, this, propertyName, classType, nullable && !key, key, false, metaForManyToOne);
				addFieldProperty(f);
			}
			
			// collections
			doCollections(classPackage, classElement, "list", "System.Collections.IList", "System.Collections.ArrayList", MetaAttribs);
			doCollections(classPackage, classElement, "map", "System.Collections.IDictionary", "System.Collections.Hashtable", MetaAttribs);
			doCollections(classPackage, classElement, "set", "System.Collections.IDictionary", "System.Collections.Hashtable", MetaAttribs);
			doCollections(classPackage, classElement, "bag", "System.Collections.IList", "System.Collections.ArrayList", MetaAttribs);
			doCollections(classPackage, classElement, "idbag", "System.Collections.IList", "System.Collections.ArrayList", MetaAttribs);
			doArrays(classElement, "array", MetaAttribs);
			doArrays(classElement, "primitive-array", MetaAttribs);
			
			
			
			
			//components
			
			for (IEnumerator iter = classElement.SelectNodes("urn:component", CodeGenerator.nsmgr).GetEnumerator(); iter.MoveNext(); )
			{
				Element cmpe = (Element) iter.Current;
				MultiMap metaForComponent = MetaAttributeHelper.loadAndMergeMetaMap(cmpe, MetaAttribs);
				String cmpname = (cmpe.Attributes["name"] == null?null:cmpe.Attributes["name"].Value);
				String cmpclass = (cmpe.Attributes["class"] == null?null:cmpe.Attributes["class"].Value);
				if ((Object) cmpclass == null || cmpclass.Equals(string.Empty))
				{
					log.Warn("component \"" + cmpname + "\" in class " + Name + " does not specify a class");
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
			}
			
			
			//    subclasses (done last so they can access this superclass for info)
			
			for (IEnumerator iter = classElement.SelectNodes("urn:subclass", CodeGenerator.nsmgr).GetEnumerator(); iter.MoveNext(); )
			{
				Element subclass = (Element) iter.Current;
				ClassMapping subclassMapping = new ClassMapping(classPackage, this, name, this, subclass, MetaAttribs);
				addSubClass(subclassMapping);
			}
			
			for (IEnumerator iter = classElement.SelectNodes("urn:joined-subclass", CodeGenerator.nsmgr).GetEnumerator(); iter.MoveNext(); )
			{
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
				fields.Add(fieldProperty);
			}
			else
			{
				throw new SystemException("Field " + fieldProperty + " is already associated with a class: " + fieldProperty.ParentClass);
			}
		}
		
		
		public virtual void  implementEquals()
		{
			log.Info("Flagging class to implement Equals() and GetHashCode().");
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
					//TODO: fix imports
					//imports.Add(className.FullyQualifiedName.Substring(0, (className.FullyQualifiedName.Length - 2) - (0))); // remove []
				}
				else
				{
					//TODO: fix imports
					//imports.Add(className.FullyQualifiedName);
				}
			}
		}
		
		public virtual void  addImport(String className)
		{
			ClassName cn = new ClassName(className);
			addImport(cn);
		}
		
		private void  doCollections(String classPackage, Element classElement, String xmlName, String interfaceClass, String implementingClass, MultiMap inheritedMeta)
		{
			
			String originalInterface = interfaceClass;
			String originalImplementation = implementingClass;
			
			for (IEnumerator collections = classElement.SelectNodes("urn:" + xmlName, CodeGenerator.nsmgr).GetEnumerator(); collections.MoveNext(); )
			{
				
				Element collection = (Element) collections.Current;
				MultiMap metaForCollection = MetaAttributeHelper.loadAndMergeMetaMap(collection, inheritedMeta);
				String propertyName = (collection.Attributes["name"] == null?string.Empty:collection.Attributes["name"].Value);
				
				//TODO: map and set in .net
				//		Small hack to switch over to sortedSet/sortedMap if sort is specified. (that is sort != unsorted)
				String sortValue = (collection.Attributes["sort"] == null?null:collection.Attributes["sort"].Value);
				if ((Object) sortValue != null && !"unsorted".Equals(sortValue) && !"".Equals(sortValue.Trim()))
				{
					if ("map".Equals(xmlName))
					{
						interfaceClass = typeof(IDictionary).FullName;
						implementingClass = typeof(IDictionary).FullName;
					}
					else if ("set".Equals(xmlName))
					{
						interfaceClass = typeof(IDictionary).FullName;
						implementingClass = typeof(IDictionary).FullName;
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
				if (collection.SelectNodes("urn:one-to-many", CodeGenerator.nsmgr).Count != 0)
				{
					foreignClass = new ClassName(collection["one-to-many"].Attributes["class"].Value);
				}
				else if (collection.SelectNodes("urn:many-to-many", CodeGenerator.nsmgr).Count != 0)
				{
					foreignClass = new ClassName(collection["many-to-many"].Attributes["class"].Value);
				}
				
				// Do the foreign keys and import
				if (foreignClass != null)
				{
					// Collect the keys
					foreignKeys = new SupportClass.HashSetSupport();
					if (collection["key"].Attributes["column"] != null)
						foreignKeys.Add(collection["key"].Attributes["column"].Value);
					
					for (IEnumerator iter = collection["key"].SelectNodes("urn:column", CodeGenerator.nsmgr).GetEnumerator(); iter.MoveNext(); )
					{
						if (((Element) iter.Current).Attributes["name"] != null)
							foreignKeys.Add(((Element) iter.Current).Attributes["name"].Value);
					}
					
					addImport(foreignClass);
				}
				FieldProperty cf = new FieldProperty(collection, this, propertyName, interfaceClassName, implementationClassName, false, foreignClass, foreignKeys, metaForCollection);
				addFieldProperty(cf);
				if (collection.SelectNodes("urn:composite-element", CodeGenerator.nsmgr).Count != 0)
				{
					for (IEnumerator compositeElements = collection.SelectNodes("urn:composite-element", CodeGenerator.nsmgr).GetEnumerator(); compositeElements.MoveNext(); )
					{
						Element compositeElement = (Element) compositeElements.Current;
						String compClass = compositeElement.Attributes["class"].Value;
						
						try
						{
							ClassMapping mapping = new ClassMapping(classPackage, compositeElement, this, true, MetaAttribs);
							ClassName classType = new ClassName(compClass);
							// add an import and field for this property
							addImport(classType);
							object tempObject;
							tempObject = mapping;
							components[mapping.FullyQualifiedName] = tempObject;
						}
						catch (Exception e)
						{
							log.Error("Error building composite-element " + compClass, e);
						}
					}
				}
			}
		}
		
		private void  doArrays(Element classElement, String type, MultiMap inheritedMeta)
		{
			for (IEnumerator arrays = classElement.SelectNodes(type).GetEnumerator(); arrays.MoveNext(); )
			{
				Element array = (Element) arrays.Current;
				MultiMap metaForArray = MetaAttributeHelper.loadAndMergeMetaMap(array, inheritedMeta);
				String role = array.Attributes["name"].Value;
				String elementClass = (array.Attributes["element-class"] == null?null:array.Attributes["element-class"].Value);
				if ((Object) elementClass == null)
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
						log.Warn("skipping collection with subcollections");
						continue;
					}
					elementClass = (elt.Attributes["type"] == null?null:elt.Attributes["type"].Value);
					if ((Object) elementClass == null)
						elementClass = (elt.Attributes["class"] == null?string.Empty:elt.Attributes["class"].Value);
				}
				ClassName cn = getFieldType(elementClass, false, true);
				
				addImport(cn);
				FieldProperty af = new FieldProperty(array, this, role, cn, false, metaForArray);
				addFieldProperty(af);
			}
		}
		
		private ClassName getFieldType(String hibernateType)
		{
			return getFieldType(hibernateType, false, false);
		}
		
		/// <summary> Return a ClassName for a hibernatetype.
		/// 
		/// </summary>
		/// <param name="hibernateType">Name of the hibernatetype (e.g. "binary")
		/// </param>
		/// <param name="isArray">if the type should be postfixed with array brackes ("[]")
		/// </param>
		/// <param name="mustBeNullable"></param>
		/// <returns>
		/// </returns>
		private ClassName getFieldType(String hibernateType, bool mustBeNullable, bool isArray)
		{
			String postfix = isArray?"[]":"";
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
					cn = new ClassName(basicType.ReturnedClass.Name + postfix);
					return cn;
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
		private String getTypeForUserType(String type)
		{
			System.Type clazz = null;
			try
			{
				if (type.IndexOf("(")>0)
					type = type.Substring(0, type.IndexOf("("));
				clazz = ReflectHelper.ClassForName(type);
				
				if (typeof(UserType).IsAssignableFrom(clazz))
				{
					UserType ut = (UserType) SupportClass.CreateNewInstance(clazz);
					log.Debug("Resolved usertype: " + type + " to " + ut.ReturnedType.Name);
					String t = clazzToName(ut.ReturnedType);
					return t;
				}
				
				if (typeof(CompositeUserType).IsAssignableFrom(clazz))
				{
					CompositeUserType ut = (CompositeUserType) SupportClass.CreateNewInstance(clazz);
					log.Debug("Resolved composite usertype: " + type + " to " + ut.ReturnedClass.Name);
					String t = clazzToName(ut.ReturnedClass);
					return t;
				}
			}
			catch (FileNotFoundException e)
			{
				if (type.IndexOf(",")>0)
					type = type.Substring(0, type.IndexOf(","));
				log.Warn("Could not find UserType: " + type + ". Using the type '" + type + "' directly instead. (" + e.ToString() + ")");
			}
			catch (UnauthorizedAccessException iae)
			{
				log.Warn("Error while trying to resolve UserType. Using the type '" + type + "' directly instead. (" + iae.ToString() + ")");
			}
			catch (Exception e)
			{
				log.Warn("Error while trying to resolve UserType. Using the type '" + type + "' directly instead. (" + e.ToString() + ")");
			}
			
			return type;
		}
		
		private String clazzToName(System.Type cl)
		{
			String s = null;
			
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
			if ((Object) SuperClass != null && getMeta("extends") != null)
			{
				log.Warn("Warning: meta attribute extends='" + getMetaAsString("extends") + "' will be ignored for subclass " + name);
			}
		}
		
		public override String ToString()
		{
			return "ClassMapping: " + name.FullyQualifiedName;
		}
		
		public virtual void  addSubClass(ClassMapping subclassMapping)
		{
			subclasses.Add(subclassMapping);
		}
		
		public virtual void  addImport(System.Type clazz)
		{
			addImport(clazz.FullName);
		}
		static ClassMapping()
		{
		}
	}
}