using System;
using System.Collections;
using System.Text;
using System.Xml;

using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg 
{
	internal class Binder 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Binder));

		private static XmlNamespaceManager nsmgr;
		private const string nsPrefix = "hbm";
		internal static Dialect.Dialect dialect;

		/// <summary>
		/// Converts a partial class name into a fully qualified one
		/// </summary>
		/// <param name="className"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public static string FullClassName( string className, Mappings mapping )
		{
			if ( className == null )
			{
				return null;
			}

			// TODO: Make this stronger, assumes stuff about presence of culture, public key etc
			// Split off the assembly reference if it's present
			string[] parts = className.Split( ',' );
			string name = parts[ 0 ];
			if ( name.IndexOf( '.' ) == -1 && mapping.DefaultNamespace != null )
			{
				// No namespace - use the default
				name = mapping.DefaultNamespace + "." + name;
			}

			string assm;
			if ( parts.Length == 1 )
			{
				if ( mapping.DefaultAssembly == null )
				{
					// No default, let it try and find it automatically
					assm = "";
				}
				else
				{
					// No assembly - use the default
					assm = ", " + mapping.DefaultAssembly;
				}
			}
			else
			{
				// Use the assembly reference provided
				assm = ", " + parts[ 1 ];
			}

			// Rebuild the name
			return name + assm;
		}

		public static void BindClass(XmlNode node, PersistentClass model, Mappings mapping) 
		{
			string className = node.Attributes["name"] == null ? null : FullClassName( node.Attributes["name"].Value, mapping );
			
			//CLASS
			try 
			{
				model.MappedClass = ReflectHelper.ClassForName(className);
			} 
			catch ( Exception cnfe ) 
			{
				throw new MappingException( "persistent class " + className + " not found", cnfe );
			}

			//PROXY INTERFACE
			XmlAttribute proxyNode = node.Attributes["proxy"];
			XmlAttribute lazyNode = node.Attributes["lazy"];
			bool lazyTrue = lazyNode != null && "true".Equals( lazyNode.Value );
			if ( proxyNode != null && ( lazyNode == null || lazyTrue ) )
			{
				try 
				{
					model.ProxyInterface = ReflectHelper.ClassForName( proxyNode.Value );
				} 
				catch (Exception cnfe) 
				{
					throw new MappingException( "proxy class not found", cnfe );
				}
			}
			if ( proxyNode == null && lazyTrue )
			{
				model.ProxyInterface = model.MappedClass;
			}
			
			//DISCRIMINATOR
			XmlAttribute discriminatorNode = node.Attributes["discriminator-value"];
			model.DiscriminatorValue = (discriminatorNode==null)
				? model.Name
				: discriminatorNode.Value;
			
			//DYNAMIC UPDATE
			XmlAttribute dynamicNode = node.Attributes["dynamic-update"];
			model.DynamicUpdate = (dynamicNode==null)
				? false :
				"true".Equals( dynamicNode.Value );
			
			//DYNAMIC INSERT
			XmlAttribute insertNode = node.Attributes["dynamic-insert"];
			model.DynamicInsert = (insertNode==null) ? 
				false : 
				"true".Equals( insertNode.Value	);

			//IMPORT
			
			// we automatically want to add an import of the Assembly Qualified Name (includes version, 
			// culture, public-key) to the className supplied in the hbm.xml file.  The most common use-case
			// will have it contain the "FullClassname, AssemblyName", it might contain version, culture, 
			// public key, etc...) but should not assume it does.
			mapping.AddImport( model.MappedClass.AssemblyQualifiedName, StringHelper.GetFullClassname(className) );

			// if we are supposed to auto-import the Class then add an import to get from the Classname
			// to the Assembly Qualified Class Name
			if (mapping.IsAutoImport) 
			{
				mapping.AddImport( model.MappedClass.AssemblyQualifiedName, StringHelper.GetClassname(className) );
			}

			//BATCH SIZE
			XmlAttribute batchNode = node.Attributes["batch-size"];
			if ( batchNode != null )
			{
				model.BatchSize = int.Parse( batchNode.Value ) ;
			}

			//SELECT BEFORE UPDATE
			/*
			XmlAttribute sbuNode = node.Attributes["select-before-update"];
			if ( sbuNode != null )
			{
				model.SelectBeforeUpdate = "true".Equals( sbuNode.Value ) ;
			}

			//OPTIMISTIC LOCK MODE
			XmlAttribute olNode = node.Attributes["optimistic-lock"];
			model.OptimisticLockMode = GetOptimisticLockMode( olNode );

			//META ATTRIBUTES
			model.MetaAttributes = GetMetas( node );
			*/

			//PERSISTER
			XmlAttribute persisterNode = node.Attributes["persister"];
			if ( persisterNode == null )
			{
				//persister = CollectionPersisterImpl.class;
			}
			else
			{
				try
				{
					model.ClassPersisterClass = ReflectHelper.ClassForName( persisterNode.Value );
				}
				catch (Exception)
				{
					throw new MappingException("could not instantiate persister class: " + persisterNode.Value );
				}
			}
		}

		public static void BindSubclass(XmlNode node, Subclass model, Mappings mappings) 
		{
			BindClass(node, model, mappings);

			if ( model.ClassPersisterClass == null ) 
			{
				model.RootClazz.ClassPersisterClass = typeof(EntityPersister);
			}

			model.Table = model.Superclass.Table;

			log.Info("Mapping subclass: " + model.Name + " -> " + model.Table.Name);

			// properties
			PropertiesFromXML(node, model, mappings);
		}

		private static string GetClassTableName( PersistentClass model, XmlNode node, Mappings mappings )
		{
			XmlAttribute tableNameNode = node.Attributes["table"];
			if (tableNameNode==null)
			{
				return mappings.NamingStrategy.ClassToTableName( model.Name );
			}
			else
			{
				return mappings.NamingStrategy.TableName( tableNameNode.Value );
			}
		}

		public static void BindJoinedSubclass(XmlNode node, Subclass model, Mappings mappings) 
		{
			BindClass(node, model, mappings);

			// joined subclass
			if ( model.ClassPersisterClass == null ) 
			{
				model.RootClazz.ClassPersisterClass = typeof(NormalizedEntityPersister);
			}

			//table + schema names
			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			Table mytable = mappings.AddTable(schema, GetClassTableName( model, node, mappings ) );
			model.Table = mytable;

			log.Info("Mapping joined-subclass: " + model.Name + " -> " + model.Table.Name );

			XmlNode keyNode = node.SelectSingleNode(nsPrefix + ":key", nsmgr);
			SimpleValue key = new SimpleValue(mytable);
			model.Key = key;
			BindSimpleValue( keyNode, key, false, model.Name, mappings );

			model.Key.Type = model.Identifier.Type;
			model.CreatePrimaryKey( dialect );
			model.CreateForeignKey( );

			// CHECK
			XmlAttribute chNode = node.Attributes["check"];
			if ( chNode != null )
			{
				mytable.AddCheckConstraint( chNode.Value );
			}

			// properties
			PropertiesFromXML(node, model, mappings);
		}

		public static void BindRootClass(XmlNode node, RootClass model, Mappings mappings) 
		{
			BindClass(node, model, mappings);

			//TABLENAME
			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode==null ? mappings.SchemaName : schemaNode.Value;
			Table table = mappings.AddTable(schema, GetClassTableName( model, node, mappings ) );
			model.Table = table;

			log.Info("Mapping class: " + model.Name + " -> " + model.Table.Name );

			//MUTABLE
			XmlAttribute mutableNode = node.Attributes["mutable"];
			model.IsMutable = (mutableNode==null) || mutableNode.Value.Equals("true");

			//WHERE
			XmlAttribute whereNode = node.Attributes["where"];
			if (whereNode!=null) model.Where = whereNode.Value;

			//CHECK
			XmlAttribute checkNode = node.Attributes["check"];
			if (checkNode!=null) table.AddCheckConstraint( checkNode.Value );

			//POLYMORPHISM
			XmlAttribute polyNode = node.Attributes["polymorphism"];
			model.IsExplicitPolymorphism = (polyNode!=null) && polyNode.Value.Equals("explicit");

			foreach(XmlNode subnode in node.ChildNodes) 
			{
				string name = subnode.LocalName; //Name;
				string propertyName = GetPropertyName(subnode);

				//I am only concerned with elements that are from the nhibernate namespace
				if(subnode.NamespaceURI!=Configuration.MappingSchemaXMLNS) continue;

				switch( name ) 
				{
					case "id":
						SimpleValue id = new SimpleValue(table);
						model.Identifier = id;
						
						if ( propertyName == null) 
						{
							BindSimpleValue(subnode, id, false, RootClass.DefaultIdentifierColumnName, mappings);
							if ( id.Type==null ) throw new MappingException("must specify an identifier type: " + model.MappedClass.Name );
							model.IdentifierProperty = null;
						} 
						else 
						{
							BindSimpleValue( subnode, id, false, propertyName, mappings );
							id.SetTypeByReflection( model.MappedClass, propertyName, PropertyAccess( subnode, mappings ) );
							Mapping.Property prop = new Mapping.Property( id );
							BindProperty( subnode, prop, mappings );
							model.IdentifierProperty = prop;
						}

						if ( id.Type.ReturnedClass.IsArray ) 
							throw new MappingException("illegal use of an array as an identifier (arrays don't reimplement equals)"); //is this true in .net?

						MakeIdentifier(subnode, id, mappings);
						break;

					case "composite-id":
						Component compId = new Component(model);
						model.Identifier = compId;
						if (propertyName==null) 
						{
							BindComponent(subnode, compId, null, model.Name, "id", false, mappings);
							model.HasEmbeddedIdentifier = compId.IsEmbedded;
							model.IdentifierProperty = null;
						} 
						else 
						{
							System.Type reflectedClass = ReflectHelper.GetGetter( model.MappedClass, propertyName ).ReturnType;
							BindComponent(subnode, compId, reflectedClass, model.Name, propertyName, false, mappings);
							Mapping.Property prop = new Mapping.Property(compId);
							BindProperty(subnode, prop, mappings);
							model.IdentifierProperty = prop;
						}
						MakeIdentifier(subnode, compId, mappings);
						break;

					case "version":
					case "timestamp":
						//version
						SimpleValue val = new SimpleValue(table);
						BindSimpleValue(subnode, val, false, propertyName, mappings);
						if ( val.Type==null ) val.Type = ( ("version".Equals(name)) ? NHibernateUtil.Int32 : NHibernateUtil.Timestamp );
						Mapping.Property timestampProp = new Mapping.Property(val);
						BindProperty(subnode, timestampProp, mappings);
						model.Version = timestampProp;
						model.AddProperty(timestampProp);
						break;

					case "discriminator":
						SimpleValue discrim = new SimpleValue(table);
						model.Discriminator = discrim;
						BindSimpleValue(subnode, discrim, false, RootClass.DefaultDiscriminatorColumnName, mappings);
						if ( discrim.Type==null ) 
						{
							discrim.Type = NHibernateUtil.String;
							foreach(Column col in discrim.ColumnCollection) 
							{
								col.Type = NHibernateUtil.String;
							}
						}
						model.IsPolymorphic = true;
						if ( subnode.Attributes["force"] != null && "true".Equals( subnode.Attributes["force"].Value ) ) 
						{
							model.IsForceDiscriminator = true;
						}
						if ( subnode.Attributes["insert"] != null && "true".Equals( subnode.Attributes["false"].Value ) ) 
						{
							model.IsDiscriminatorInsertable = false;
						}
						break;

					case "jcs-cache":
						string className = model.MappedClass.FullName;
						ICacheConcurrencyStrategy cache = CacheFactory.CreateCache( subnode, className, model.IsMutable );
						mappings.AddCache( className, cache );
						model.Cache = cache;

						break;
				}
			}

			model.CreatePrimaryKey(dialect);

			PropertiesFromXML(node, model, mappings);
		}

		public static void BindColumns(XmlNode node, SimpleValue model, bool isNullable, bool autoColumn, string propertyPath, Mappings mappings) 
		{
			//COLUMN(S)
			XmlAttribute columnNode = node.Attributes["column"];
			if ( columnNode==null ) 
			{
				int count=0;
				foreach(XmlNode subnode in node.SelectNodes(nsPrefix + ":column", nsmgr)) 
				{
					Table table = model.Table;
					Column col = new Column( model.Type, count++ );
					BindColumn(subnode, col, isNullable);
					// TODO: Check, isn't the name attribute mandatory in the schema?
					string name = (subnode.Attributes["name"]==null) ? String.Empty : subnode.Attributes["name"].Value;
					col.Name = mappings.NamingStrategy.ColumnName( name );
					if (table!=null) table.AddColumn(col); //table=null -> an association, fill it in later
					model.AddColumn(col);
					//column index
					XmlAttribute indexNode = subnode.Attributes["index"];
					if ( indexNode!=null && table!=null ) 
					{
						table.GetIndex( indexNode.Value ).AddColumn(col);
					}
					XmlAttribute uniqueNode = subnode.Attributes["unique-key"];
					if ( uniqueNode!=null && table!=null ) 
					{
						table.GetUniqueKey( uniqueNode.Value ).AddColumn(col);
					}
				}
			} 
			else 
			{
				Column col = new Column( model.Type, 0 );
				BindColumn(node, col, isNullable);
				col.Name = mappings.NamingStrategy.ColumnName( columnNode.Value );
				Table table = model.Table;
				if (table!=null) table.AddColumn(col); //table=null -> an association - fill it in later
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
				XmlAttribute indexAttr = node.Attributes["unique-key"];
				if ( indexAttr != null && table != null )
				{
					table.GetIndex( indexAttr.Value ).AddColumn( col );
				}
			}

			if ( autoColumn && model.ColumnSpan==0 ) 
			{
				Column col = new Column( model.Type, 0 );
				BindColumn(node, col, isNullable);
				col.Name = mappings.NamingStrategy.PropertyToColumnName( propertyPath );
				model.Table.AddColumn(col);
				model.AddColumn(col);
			}
		}

		/*
		/// <remarks>
		/// Does _not_ automatically make a column if none is specifed by XML
		/// </remarks>
		public static void BindValue(XmlNode node, Value model, bool isNullable) 
		{
			//TYPE
			model.Type = GetTypeFromXML(node);
			BindColumns(node, model, isNullable, false, null);
		}
		*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="model"></param>
		/// <param name="isNullable"></param>
		/// <param name="path"></param>
		/// <param name="mappings"></param>
		public static void BindSimpleValue( XmlNode node, SimpleValue model, bool isNullable, string path, Mappings mappings )
		{
			model.Type = GetTypeFromXML(node);

			XmlAttribute formulaNode = node.Attributes["formula"];
			if (formulaNode != null)
			{
				Formula f = new Formula();
				f.FormulaString = formulaNode.InnerText;
				model.Formula = f;
			}
			else
			{
				BindColumns(node, model, isNullable, true, path, mappings);
			}
		}

		/*
		/// <remarks>
		/// automatically makes a column with the default name if none is specifed by XML
		/// </remarks>
		public static void BindValue(XmlNode node, Value model, bool isNullable, string defaultColumnName) 
		{
			model.Type = GetTypeFromXML(node);
			XmlAttribute formulaNode = node.Attributes["formula"];
			if (formulaNode != null)
			{
				Formula f = new Formula();
				f.FormulaString = formulaNode.InnerText;
				model.Formula = f;
			}
			else
			{
				BindColumns(node, model, isNullable, true, defaultColumnName);
			}
		}
		*/

		private static string PropertyAccess( XmlNode node, Mappings mappings )
		{
			XmlAttribute accessNode = node.Attributes["access"];
			return accessNode != null ? accessNode.Value : mappings.DefaultAccess;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="model"></param>
		/// <param name="mappings"></param>
		public static void BindProperty(XmlNode node, Mapping.Property model, Mappings mappings) 
		{
			model.Name = GetPropertyName(node);
			IType type = model.Value.Type;
			if (type==null) throw new MappingException("could not determine a property type for: " + model.Name );
			
			model.PropertyAccessorName = PropertyAccess( node, mappings );

			XmlAttribute cascadeNode = node.Attributes["cascade"];
			model.Cascade = (cascadeNode==null) ? mappings.DefaultCascade : cascadeNode.Value;
			
			XmlAttribute updateNode = node.Attributes["update"];
			model.IsUpdateable = (updateNode==null) ? true : "true".Equals( updateNode.Value );

			XmlAttribute insertNode = node.Attributes["insert"];
			model.IsInsertable = (insertNode==null) ? true : "true".Equals( insertNode.Value );

			if ( log.IsDebugEnabled ) 
			{
				string msg = "Mapped property: " + model.Name;
				string columns = Columns( model.Value );
				if ( columns.Length > 0  ) msg += " -> " + columns;
				if ( model.Type !=null ) msg += ", type: " + model.Type.Name;
				log.Debug(msg);
			}
		}
		
		private static string Columns(IValue val) 
		{
			StringBuilder columns = new StringBuilder();
			bool first = true;
			foreach(Column col in val.ColumnCollection)
			{
				if (first)
					first = false;
				else
					columns.Append(", ");
				columns.Append(col.Name);
			}
			return columns.ToString();
		}

		/// <remarks>
		/// Called for all collections
		/// </remarks>
		public static void BindCollection(XmlNode node, Mapping.Collection model, string className, string path, Mappings mappings) 
		{
			//ROLENAME
			model.Role = StringHelper.Qualify( className, path ) ;

			XmlAttribute inverseNode = node.Attributes["inverse"];
			if ( inverseNode!=null ) model.IsInverse = StringHelper.BooleanValue( inverseNode.Value );

			XmlAttribute orderNode = node.Attributes["order-by"];
			if ( orderNode!=null ) 
			{
				model.OrderBy = orderNode.Value;
			}
			XmlAttribute whereNode = node.Attributes["where"];
			if ( whereNode!=null ) 
			{
				model.Where = whereNode.Value;
			}
			XmlAttribute batchNode = node.Attributes["batch-size"];
			if ( batchNode!=null ) 
			{
				model.BatchSize = Int32.Parse( batchNode.Value );
			}

			//PERSISTER
			XmlAttribute persisterNode = node.Attributes["persister"];
			if ( persisterNode == null )
			{
				//persister = CollectionPersisterImpl.class;
			}
			else
			{
				try
				{
					string persisterName = FullClassName( persisterNode.Value, mappings );
					model.CollectionPersisterClass = ReflectHelper.ClassForName( persisterName );
				}
				catch (Exception)
				{
					throw new MappingException("could not instantiate collection persister class: " + className);
				}
			}

			InitOuterJoinFetchSetting( node, model );

			XmlNode oneToManyNode = node.SelectSingleNode(nsPrefix + ":one-to-many", nsmgr);
			if ( oneToManyNode!=null ) 
			{
				OneToMany oneToMany = new OneToMany( model.Owner );
				model.Element = oneToMany;
				BindOneToMany( oneToManyNode, oneToMany, mappings );
				//we have to set up the table later!! yuck
			} 
			else 
			{
				//TABLE
				XmlAttribute tableNode = node.Attributes["table"];
				string tableName;
				if ( tableNode != null ) 
				{
					tableName = mappings.NamingStrategy.TableName( tableNode.Value );
				} 
				else 
				{
					tableName = mappings.NamingStrategy.PropertyToTableName( className, path );
				}
				XmlAttribute schemaNode = node.Attributes["schema"];
				string schema = schemaNode==null ? mappings.SchemaName : schemaNode.Value;
				model.CollectionTable = mappings.AddTable(schema, tableName);

				log.Info("Mapping collection: " + model.Role + " -> " + model.CollectionTable.Name );
			}

			//LAZINESS
			XmlAttribute lazyNode = node.Attributes["lazy"];
			if (lazyNode!=null) 
			{
				model.IsLazy = StringHelper.BooleanValue( lazyNode.Value );
			}

			//SORT
			XmlAttribute sortedAtt = node.Attributes["sort"];
			// unsorted, natural, comparator.class.name
			if (sortedAtt==null || sortedAtt.Value.Equals("unsorted") ) 
			{
				model.IsSorted = false;
			} 
			else 
			{
				model.IsSorted = true;
				string comparatorClassName = FullClassName( sortedAtt.Value, mappings );
				if ( !comparatorClassName.Equals("natural") ) 
				{
					try 
					{
						model.Comparer = (IComparer) Activator.CreateInstance( ReflectHelper.ClassForName( comparatorClassName ) );
					} 
					catch (Exception) 
					{
						throw new MappingException( "could not instantiate comparer class: " + comparatorClassName );
					}
				}
			}

			//ORPHAN DELETE (used for programmer error detection)
			XmlAttribute cascadeAtt = node.Attributes["cascade"];
			if ( cascadeAtt!=null && cascadeAtt.Value.Equals("all-delete-orphan") ) model.OrphanDelete = true;

			//set up second pass
			if (model is List) 
			{
				mappings.AddSecondPass( new ListSecondPass(node, mappings, (List) model) );
			} 
			else if (model is Map) 
			{
				mappings.AddSecondPass( new MapSecondPass(node, mappings, (Map) model) );
			} 
			else if (model is Set) 
			{
				mappings.AddSecondPass( new SetSecondPass(node, mappings, (Set) model) );
			}
			else if (model is IdentifierCollection) 
			{
				mappings.AddSecondPass( new IdentifierCollectionSecondPass(node, mappings, (IdentifierCollection) model) );
			}
			else 
			{
				mappings.AddSecondPass( new CollectionSecondPass(node, mappings, model) );
			}
		}

		public static void BindIntegerValue(XmlNode node, IntegerValue model, string defaultColumnName, bool isNullable, Mappings mappings) 
		{
			BindSimpleValue(node, model, isNullable, defaultColumnName, mappings);

			if (model.ColumnCollection.Count > 1)
				log.Error("This shouldn't happen, check BindIntegerValue");
			foreach(Column col in model.ColumnCollection) 
			{
				col.Type = NHibernateUtil.Int32;
				col.TypeIndex = 0;
				break;
			}
		}

		public static void BindManyToOne(XmlNode node, ManyToOne model, string defaultColumnName, bool isNullable, Mappings mappings)
		{
			BindColumns(node, model, isNullable, true, defaultColumnName, mappings);
			InitOuterJoinFetchSetting(node, model);

			XmlAttribute ukName = node.Attributes["property-ref"];
			if ( ukName != null )
			{
				model.ReferencedPropertyName = ukName.Value;
			}

			XmlAttribute typeNode = node.Attributes["class"];
			
			if ( typeNode!=null ) 
			{
				try 
				{
					string name = FullClassName( typeNode.Value, mappings );
					model.Type = TypeFactory.ManyToOne( ReflectHelper.ClassForName( name ), model.ReferencedPropertyName ); 
				} 
				catch 
				{
					throw new MappingException("could not find class: " + typeNode.Value);
				}
			}

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if ( fkNode != null )
			{
				model.ForeignKeyName = fkNode.Value;
			}
		}

		public static void BindAny(XmlNode node, Any model, bool isNullable, Mappings mappings) 
		{
			model.IdentifierType = GetTypeFromXML(node);

			XmlAttribute metaAttribute = node.Attributes["meta-type"];
			if (metaAttribute!=null) 
			{
				IType metaType = TypeFactory.HueristicType( metaAttribute.Value );
				if ( metaType==null ) throw new MappingException("could not interpret meta-type");
				model.MetaType = metaType;

				// TODO: 2.1 Handle meta-value collection
			}

			BindColumns(node, model, isNullable, false, null, mappings);
		}

		public static void BindOneToOne(XmlNode node, OneToOne model, bool isNullable, Mappings mappings) 
		{
			BindColumns(node, model, isNullable, false, null, mappings);
			InitOuterJoinFetchSetting(node, model);

			XmlAttribute constrNode = node.Attributes["constrained"];
			bool constrained = constrNode!=null && constrNode.Value.Equals("true");
			model.IsConstrained = constrained;

			model.ForeignKeyType = (constrained ? ForeignKeyType.ForeignKeyFromParent : ForeignKeyType.ForeignKeyToParent);

			XmlAttribute fkNode = node.Attributes[ "foreign-key" ];
			if ( fkNode != null )
			{
				model.ForeignKeyName = fkNode.Value;
			}

			XmlAttribute ukName = node.Attributes[ "property-ref" ];
			if ( ukName != null )
			{
				model.ReferencedPropertyName = ukName.Value;
			}

			XmlAttribute classNode = node.Attributes[ "class" ];
			if ( classNode != null ) 
			{
				try 
				{
					string name = FullClassName( classNode.Value, mappings );
					model.Type = TypeFactory.OneToOne( ReflectHelper.ClassForName( name ), model.ForeignKeyType, model.ReferencedPropertyName ) ;
				} 
				catch (Exception) 
				{
					throw new MappingException("could not find class: " + classNode.Value);
				}
			}
		}

		public static void BindOneToMany(XmlNode node, OneToMany model, Mappings mappings) 
		{
			try 
			{
				string name = FullClassName( node.Attributes["class"].Value, mappings );
				model.Type = (EntityType) NHibernateUtil.Entity( ReflectHelper.ClassForName( name ) );
			} 
			catch (Exception e) 
			{
				throw new MappingException("associated class not found", e);
			}
		}

		public static void BindColumn(XmlNode node, Column model, bool isNullable) 
		{
			XmlAttribute lengthNode = node.Attributes["length"];
			if ( lengthNode!=null ) model.Length = int.Parse( lengthNode.Value );
			
			XmlAttribute nullNode = node.Attributes["not-null"];
			model.IsNullable = (nullNode!=null) ? !StringHelper.BooleanValue( nullNode.Value) : isNullable;

			XmlAttribute unqNode = node.Attributes["unique"];
			model.IsUnique = unqNode!=null && StringHelper.BooleanValue( unqNode.Value );
			
			XmlAttribute chkNode = node.Attributes["check"];
			model.CheckConstraint = chkNode != null ? chkNode.Value : string.Empty;

			XmlAttribute typeNode = node.Attributes["sql-type"];
			model.SqlType = (typeNode==null) ? null : typeNode.Value;
		}

		/// <remarks>
		/// Called for arrays and primitive arrays
		/// </remarks>
		public static void BindArray(XmlNode node, Mapping.Array model, string prefix, string path, Mappings mappings) 
		{
			BindCollection(node, model, prefix, path, mappings);

			XmlAttribute att = node.Attributes["element-class"];

			if ( att!=null ) 
			{
				try 
				{
					model.ElementClass = ReflectHelper.ClassForName( att.Value );
				} 
				catch (Exception e) 
				{
					throw new MappingException(e);
				}
			} 
			else 
			{
				foreach(XmlNode subnode in node.ChildNodes) 
				{
					string name = subnode.LocalName; //.Name;

					//I am only concerned with elements that are from the nhibernate namespace
					if(subnode.NamespaceURI!=Configuration.MappingSchemaXMLNS) continue;

					switch(name) 
					{
						case "element":
							IType type = GetTypeFromXML(subnode);

							model.ElementClass = type.ReturnedClass;

							break;

						case "one-to-many":
						case "many-to-many":
						case "composite-element":
							try 
							{
								string className = FullClassName( subnode.Attributes["class"].Value, mappings );								
								model.ElementClass = ReflectHelper.ClassForName( className );
							} 
							catch (Exception e) 
							{
								throw new MappingException(e);
							}
							break;
					}
				}
			}
		}

		public static void BindComponent(XmlNode node, Component model, System.Type reflectedClass, string className, string path, bool isNullable, Mappings mappings) 
		{
			XmlAttribute classNode = node.Attributes["class"];
			
			if ( "dynamic-component".Equals( node.Name ) ) 
			{
				model.IsEmbedded = false;
				model.IsDynamic = true;
			} 
			else if ( classNode != null ) 
			{
				try 
				{
					model.ComponentClass = ReflectHelper.ClassForName( FullClassName( classNode.Value, mappings ) ) ;
				} 
				catch (Exception e) 
				{
					throw new MappingException("component class not found", e);
				}
				model.IsEmbedded = false;
			} 
			else if ( reflectedClass != null )
			{
				model.ComponentClass = reflectedClass;
				model.IsEmbedded = false;
			} 
			else 
			{
				// an "embedded" component (ids only)
				model.ComponentClass = model.Owner.MappedClass;
				model.IsEmbedded = true;
			}

			foreach(XmlNode subnode in node.ChildNodes) 
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if ( subnode.NamespaceURI != Configuration.MappingSchemaXMLNS ) continue;

				string name = subnode.LocalName; //.Name;
				string propertyName = GetPropertyName(subnode);
				string subpath = propertyName == null ? null : StringHelper.Qualify( path, propertyName ) ;

				CollectionType collectType = CollectionType.CollectionTypeFromString(name);
				IValue value = null;
				if ( collectType!=null ) 
				{
					Mapping.Collection collection = collectType.Create( subnode, className, subpath, model.Owner, mappings );
					mappings.AddCollection(collection);
					value = collection;
				} 
				else if ( "many-to-one".Equals(name) || "key-many-to-one".Equals(name) ) 
				{
					value = new ManyToOne( model.Table);
					BindManyToOne(subnode, (ManyToOne) value, subpath, isNullable, mappings);
				} 
				else if ( "one-to-one".Equals(name) ) 
				{
					value = new OneToOne( model.Table, model.Owner.Identifier );
					BindOneToOne(subnode, (OneToOne) value, isNullable, mappings);
				} 
				else if ( "any".Equals(name) ) 
				{
					value = new Any( model.Table );
					BindAny(subnode, (Any) value, isNullable, mappings);
				} 
				else if ( "property".Equals(name) || "key-property".Equals(name) ) 
				{
					value = new SimpleValue( model.Table );
					BindSimpleValue(subnode, (SimpleValue) value, isNullable, subpath, mappings);
				} 
				else if ( "component".Equals(name) || "dynamic-component".Equals(name) || "nested-composite-element".Equals(name) ) 
				{
					System.Type subreflectedClass = (model.ComponentClass == null) ?
						null :
						ReflectHelper.GetGetter( model.ComponentClass, propertyName ).ReturnType;
					value = ( model.Owner!=null ) ?
						new Component( model.Owner ) : // a class component
						new Component( model.Table );  // a composite element
					BindComponent(subnode, (Component) value, subreflectedClass, className, subpath, isNullable, mappings);
				} 
				else if ( "parent".Equals(name) ) 
				{
					model.ParentProperty = propertyName;
				}

				if ( value!=null ) 
				{
					model.AddProperty( CreateProperty( value, propertyName, model.ComponentClass, subnode, mappings ) );
				}
			}

			int span = model.PropertySpan;
			string[] names = new string[span];
			IType[] types = new IType[span];
			Cascades.CascadeStyle[] cascade = new Cascades.CascadeStyle[span];
			OuterJoinFetchStrategy[] joinedFetch = new OuterJoinFetchStrategy[span];
			
			int i=0;
			foreach( Mapping.Property prop in model.PropertyCollection ) 
			{
				names[i] = prop.Name;
				types[i] = prop.Type;
				cascade[i] = prop.CascadeStyle;
				joinedFetch[i] = prop.Value.OuterJoinFetchSetting;
				i++;
			}

			IType componentType;
			if ( model.IsDynamic )
			{
				// TODO: Implement DynamicComponentType
				//componentType = new DynamicComponentType( names, types, joinedFetch, cascade );
				componentType = null;
			}
			else
			{
				Property.IGetter[] getters = new Property.IGetter[span];
				Property.ISetter[] setters = new Property.ISetter[span];
				bool foundCustomAccessor = false;
				i = 0;
				foreach( Mapping.Property prop in model.PropertyCollection ) 
				{
					// the setters is new code added for field access
					setters[i] = prop.GetSetter( model.ComponentClass );
					getters[i] = prop.GetGetter( model.ComponentClass );
					if( !prop.IsBasicPropertyAccessor ) foundCustomAccessor = true;
					i++;
				}

				componentType = new ComponentType( model.ComponentClass, names, getters, setters, foundCustomAccessor, types, joinedFetch, cascade, model.ParentProperty, model.IsEmbedded );
			}
			model.Type = componentType;	
		}

		private static Mapping.Property CreateProperty( IValue value, string propertyName, System.Type parentClass, XmlNode subnode, Mappings mappings )
		{
			if ( parentClass != null && value.IsSimpleValue )
			{
				( (SimpleValue) value).SetTypeByReflection( parentClass, propertyName, PropertyAccess( subnode, mappings ) );
			}

			// This is done here 'cos we might only know the type here (ugly!)
			if ( value is ToOne )
			{
				string propertyRef = ( (ToOne) value).ReferencedPropertyName;
				if ( propertyRef != null )
				{
					mappings.AddUniquePropertyReference( ( (EntityType) value.Type).AssociatedClass, propertyRef );
				}
			}

			value.CreateForeignKey( );
			Mapping.Property prop = new Mapping.Property();
			prop.Value = value;
			BindProperty( subnode, prop, mappings );

			return prop;
		}

		private static IType GetTypeFromXML(XmlNode node) 
		{
			IType type;
			XmlAttribute typeNode = node.Attributes["type"];

			if (typeNode==null) typeNode = node.Attributes["id-type"]; //for an any
			if (typeNode==null) 
			{
				return null; //we will have to use reflection
			} 
			else 
			{
				type = TypeFactory.HueristicType( typeNode.Value );
				if (type==null) throw new MappingException("could not interpret type: " + typeNode.Value );
			}
			return type;
		}

		private static void InitOuterJoinFetchSetting(XmlNode node, IFetchable model) 
		{
			XmlAttribute jfNode = node.Attributes["outer-join"];
			if ( jfNode==null ) 
			{
				model.OuterJoinFetchSetting = OuterJoinFetchStrategy.Auto;
			} 
			else 
			{
				string eoj = jfNode.Value;
				if ( "auto".Equals(eoj) ) 
				{
					model.OuterJoinFetchSetting = OuterJoinFetchStrategy.Auto;
				} 
				else 
				{
					model.OuterJoinFetchSetting = ("true".Equals(eoj)) ?
						OuterJoinFetchStrategy.Eager : OuterJoinFetchStrategy.Lazy;
				}
			}
		}

		private static void MakeIdentifier(XmlNode node, SimpleValue model, Mappings mappings) 
		{
			//GENERATOR

			XmlNode subnode = node.SelectSingleNode(nsPrefix + ":generator", nsmgr);
			if ( subnode!=null ) 
			{
				if (subnode.Attributes["class"] == null)
					throw new MappingException("no class given for generator");

				model.IdentifierGeneratorStrategy = subnode.Attributes["class"].Value;

				IDictionary parms = new Hashtable();

				if ( mappings.SchemaName!=null ) 
				{
					parms.Add( "schema", mappings.SchemaName );
				}

				parms.Add( "target_table", model.Table.Name);

				foreach(Column col in model.ColumnCollection)
				{
					parms.Add( "target_column", col);
					break;
				}

				//foreach(XmlNode childNode in subnode.SelectNodes("param")) {
				foreach(XmlNode childNode in subnode.SelectNodes(nsPrefix + ":param", nsmgr)) 
				{
					parms.Add(
						childNode.Attributes["name"].Value,
						childNode.FirstChild.Value
						);
				}

				model.IdentifierGeneratorProperties = parms;
			}

			model.Table.SetIdentifierValue(model);

			//unsaved-value
			XmlAttribute nullValueNode = node.Attributes["unsaved-value"];
			if (nullValueNode!=null) 
				model.NullValue = nullValueNode.Value;
			else
				model.NullValue = "null";
		}

		protected static void PropertiesFromXML(XmlNode node, PersistentClass model, Mappings mappings) 
		{
			Table table = model.Table;

			foreach(XmlNode subnode in node.ChildNodes) 
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if(subnode.NamespaceURI!=Configuration.MappingSchemaXMLNS) continue;

				string name = subnode.LocalName; //.Name;
				string propertyName = GetPropertyName(subnode);

				CollectionType collectType = CollectionType.CollectionTypeFromString(name);
				IValue value = null;
				if (collectType!=null) 
				{
					Mapping.Collection collection = collectType.Create(subnode, model.Name, propertyName, model, mappings);
					mappings.AddCollection(collection);
					value = collection;
				} 
				else if ( "many-to-one".Equals(name) ) 
				{
					value = new ManyToOne(table);
					BindManyToOne(subnode, (ManyToOne) value, propertyName, true, mappings);
				} 
				else if ( "any".Equals(name) ) 
				{
					value = new Any(table);
					BindAny(subnode, (Any) value, true, mappings);
				}
				else if ( "one-to-one".Equals(name) ) 
				{
					value = new OneToOne(table, model.Identifier );
					BindOneToOne(subnode, (OneToOne) value, true, mappings);
				} 
				else if ( "property".Equals(name) ) 
				{
					value = new SimpleValue(table);
					BindSimpleValue(subnode, (SimpleValue) value, true, propertyName, mappings);
				} 
				else if ( "component".Equals(name) || "dynamic-component".Equals(name) ) 
				{
					System.Type reflectedClass = ReflectHelper.GetGetter( model.MappedClass, propertyName ).ReturnType;
					value = new Component(model);
					BindComponent(subnode, (Component) value, reflectedClass, model.Name, propertyName, true, mappings);
				} 
				else if ( "subclass".Equals(name) ) 
				{
					HandleSubclass( model, mappings, subnode );
				} 
				else if ( "joined-subclass".Equals(name) ) 
				{
					HandleJoinedSubclass( model, mappings, subnode );
				}
				if ( value!=null) 
				{
					model.AddNewProperty( CreateProperty( value, propertyName, model.MappedClass, subnode, mappings ) );
				}
			}
		}

		public static void BindSetSecondPass(XmlNode node, Mapping.Set model, IDictionary persistentClasses, Mappings mappings) 
		{
			BindCollectionSecondPass(node, model, persistentClasses, mappings);
		
			if ( !model.IsOneToMany ) model.CreatePrimaryKey();
		}

		/// <remarks>
		/// Called for Lists, arrays, primitive arrays
		/// </remarks>>
		public static void BindListSecondPass(XmlNode node, Mapping.List model, IDictionary classes, Mappings mappings) 
		{
			BindCollectionSecondPass(node, model, classes, mappings);

			XmlNode subnode = node.SelectSingleNode(nsPrefix + ":index", nsmgr);
			IntegerValue iv = new IntegerValue( model.CollectionTable );
			BindIntegerValue( subnode, iv, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany, mappings );
			model.Index = iv;
		}

		public static void BindIdentifierCollectionSecondPass(XmlNode node, IdentifierCollection model, IDictionary persitentClasses, Mappings mappings) 
		{
			BindCollectionSecondPass(node, model, persitentClasses, mappings);

			XmlNode subnode = node.SelectSingleNode( nsPrefix + ":collection-id", nsmgr );
			SimpleValue id = new SimpleValue( model.CollectionTable );
			BindSimpleValue( subnode, id, false, IdentifierCollection.DefaultIdentifierColumnName, mappings);
			model.Identifier = id;
			MakeIdentifier( subnode, id, mappings );
		}

		//map binding

		public static void BindMapSecondPass(XmlNode node, Mapping.Map model, IDictionary classes, Mappings mappings) 
		{
			BindCollectionSecondPass( node, model, classes, mappings );

			foreach( XmlNode subnode in node.ChildNodes ) 
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if( subnode.NamespaceURI != Configuration.MappingSchemaXMLNS ) continue;

				string name = subnode.LocalName; //.Name;

				if ( "index".Equals(name) ) 
				{
					SimpleValue value = new SimpleValue( model.CollectionTable );
					BindSimpleValue( subnode, value, model.IsOneToMany, IndexedCollection.DefaultIndexColumnName, mappings );
					model.Index = value;
					if ( model.Index.Type==null ) throw new MappingException("map index element must specify a type");
				} 
				else if ( "index-many-to-many".Equals(name) ) 
				{
					ManyToOne mto = new ManyToOne( model.CollectionTable );
					BindManyToOne(subnode, mto, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany, mappings);
					model.Index = mto;
				} 
				else if ( "composite-index".Equals(name) ) 
				{
					Component component = new Component( model.CollectionTable );
					BindComponent(subnode, component, null, model.Role, "index", model.IsOneToMany, mappings);
					model.Index = component;
				}
				else if ( "index-many-to-any".Equals(name) ) 
				{
					Any any = new Any( model.CollectionTable );
					BindAny( subnode, any, model.IsOneToMany, mappings );
					model.Index = any;
				} 
			}
		}

		/*
		private static void AddIndexForIndexedCollection(IndexedCollection model)
		{
			//TODO: Find out why this is commented out
//			 if ( model.IsOneToMany ) {
//				Index uniqueKey = model.Table.Index("x");
//				Iterator cols = model.getKey().getColumnIterator();
//				while ( cols.hasNext() ) uniqueKey.addColumn( (Column) cols.next() );
//				cols = ( (IndexedCollection) model ).getIndex().getColumnIterator();
//				while ( cols.hasNext() ) uniqueKey.addColumn( (Column) cols.next() );
//			}
		}
		*/

		/// <remarks>
		/// Called for all collections
		/// </remarks>
		public static void BindCollectionSecondPass(XmlNode node, Mapping.Collection model, IDictionary persistentClasses, Mappings mappings) 
		{
			if ( model.IsOneToMany ) 
			{
				OneToMany oneToMany = (OneToMany) model.Element;
				System.Type assocClass = oneToMany.EntityType.AssociatedClass;
				PersistentClass persistentClass = (PersistentClass) persistentClasses[assocClass];
				if ( persistentClass==null) 
				{
					throw new MappingException(	"Association references unmapped class: " + assocClass.Name);
				}
				oneToMany.AssociatedClass = persistentClass;
				model.CollectionTable = persistentClass.Table;

				log.Info("mapping collection: " + model.Role + " -> " + model.CollectionTable.Name);
			}

			// CHECK
			XmlAttribute chNode = node.Attributes["check"];
			if ( chNode != null )
			{
				model.CollectionTable.AddCheckConstraint( chNode.Value );
			}

			foreach(XmlNode subnode in node.ChildNodes) 
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if(subnode.NamespaceURI!=Configuration.MappingSchemaXMLNS) continue;

				string name = subnode.LocalName; //.Name;

				if ( "key".Equals(name) || "generated-key".Equals(name) ) 
				{
					SimpleValue key = new SimpleValue( model.CollectionTable );
					BindSimpleValue(subnode, key, model.IsOneToMany, Mapping.Collection.DefaultKeyColumnName, mappings);
					key.Type = model.Owner.Identifier.Type;
					if ( key.Type.ReturnedClass.IsArray ) 
					{
						throw new MappingException("illegal use of an array as an identifier (arrays don't reimplement equals)");
					}
					model.Key = key;
				} 
				else if ( "element".Equals(name) ) 
				{
					SimpleValue elt = new SimpleValue( model.CollectionTable );
					model.Element = elt;
					BindSimpleValue(subnode, elt, true, Mapping.Collection.DefaultElementColumnName, mappings);
				} 
				else if ( "many-to-many".Equals(name) ) 
				{
					ManyToOne element = new ManyToOne( model.CollectionTable );
					model.Element = element;
					BindManyToOne(subnode, element, Mapping.Collection.DefaultElementColumnName, true, mappings);
				} 
				else if ( "composite-element".Equals(name) ) 
				{
					Component element = new Component( model.CollectionTable );
					model.Element = element;
					BindComponent(subnode, element, null, model.Role, "element", true, mappings);
				}
				else if ( "many-to-any".Equals(name) ) 
				{
					Any element = new Any( model.CollectionTable );				
					model.Element = element;
					BindAny(subnode, element, true, mappings);
				} 
				else if ( "jcs-cache".Equals(name) )
				{
					ICacheConcurrencyStrategy cache = CacheFactory.CreateCache( subnode, model.Role, model.Owner.IsMutable );
					mappings.AddCache( model.Role, cache );
					model.Cache = cache;
//					model.Cache = Configuration.CreateCache( 
//						subnode.Attributes["usage"].Value,
//						model.Role, 
//						model.Owner );
				}
			}

			if ( !model.IsInverse ) 
			{
				if ( !model.IsOneToMany ) // no foreign key for a one-to-many
				{
					model.Element.CreateForeignKey();
				}

				model.Key.CreateForeignKeyOfClass( model.Owner.MappedClass );
			}
		}

		public static void BindRoot( XmlDocument doc, Mappings model ) 
		{
			XmlNode hmNode = doc.DocumentElement;
			XmlAttribute schemaNode = hmNode.Attributes["schema"];
			model.SchemaName = (schemaNode==null) ? null : schemaNode.Value;
			XmlAttribute dcNode = hmNode.Attributes["default-cascade"];
			model.DefaultCascade = (dcNode==null) ? "none" : dcNode.Value ;
			XmlAttribute daNode = hmNode.Attributes["default-access"];
			model.DefaultAccess = (daNode==null) ? "property" : daNode.Value;
			XmlAttribute aiNode = hmNode.Attributes["auto-import"];
			model.IsAutoImport = (aiNode==null) ? true : "true".Equals( aiNode.Value );
			XmlAttribute nsNode = hmNode.Attributes["namespace"];
			model.DefaultNamespace = (nsNode==null) ? null : nsNode.Value ;
			XmlAttribute assemblyNode = hmNode.Attributes["assembly"];
			model.DefaultAssembly = (assemblyNode==null) ? null : assemblyNode.Value ;

			nsmgr = new XmlNamespaceManager(doc.NameTable);
			// note that the prefix has absolutely nothing to do with what the user
			// selects as their prefix in the document.  It is the prefix we use to 
			// build the XPath and the nsmgr takes care of translating our prefix into
			// the user defined prefix...
			nsmgr.AddNamespace( nsPrefix, Configuration.MappingSchemaXMLNS );
			
			foreach(XmlNode n in hmNode.SelectNodes( nsPrefix + ":class", nsmgr ) ) 
			{
				RootClass rootclass = new RootClass();
				Binder.BindRootClass( n, rootclass, model );
				model.AddClass( rootclass );
			}

			foreach(XmlNode n in hmNode.SelectNodes( nsPrefix + ":subclass", nsmgr ) ) 
			{
				PersistentClass superModel = GetSuperclass( model, n );
				HandleSubclass(superModel, model, n);
			}

			foreach(XmlNode n in hmNode.SelectNodes( nsPrefix + ":joined-subclass", nsmgr ) ) 
			{
				PersistentClass superModel = GetSuperclass(model, n);
				HandleJoinedSubclass(superModel, model, n);
			}

			foreach(XmlNode n in hmNode.SelectNodes( nsPrefix + ":query", nsmgr ) ) 
			{
				string qname = n.Attributes["name"].Value;
				string query = n.FirstChild.Value;
				log.Debug("Named query: " + qname + " -> " + query);
				model.AddQuery(qname, query);
			}

			foreach(XmlNode n in hmNode.SelectNodes( nsPrefix + ":sql-query", nsmgr ) ) 
			{
				string qname = n.Attributes["name"].Value;
				NamedSQLQuery namedQuery = new NamedSQLQuery( n.FirstChild.Value );

				foreach(XmlNode returns in n.SelectNodes( nsPrefix + ":return", nsmgr ) )
				{
					string alias = returns.Attributes["alias"].Value;
					System.Type clazz;
					try
					{
						string name = FullClassName( returns.Attributes["class"].Value, model );
						clazz = ReflectHelper.ClassForName( name );
					}
					catch (Exception)
					{
						throw new MappingException( string.Format( "class not found for alias:", alias ) );
					}
					namedQuery.AddAliasedClass( alias, clazz );
				}

				foreach( XmlNode table in n.SelectNodes( nsPrefix + ":synchronise", nsmgr ) )
				{
					namedQuery.AddSynchronizedTable( table.Attributes["table"].Value );
				}

				log.Debug("Named sql query: " + qname + " -> " + namedQuery.QueryString );
				model.AddSQLQuery(qname, namedQuery);
			}

			foreach(XmlNode n in hmNode.SelectNodes(nsPrefix + ":import", nsmgr) ) 
			{
				string className = FullClassName( n.Attributes["class"].Value, model );
				XmlAttribute renameNode = n.Attributes[ "rename" ];
				string rename = (renameNode == null) ? StringHelper.GetClassname( className ) : renameNode.Value;
				log.Debug( "Import: " + rename + " -> " + className );
				model.AddImport(className, rename);
			}
		}

		private static string GetPropertyName(XmlNode node) 
		{
			if (node.Attributes!=null) 
			{
				XmlAttribute propertyNameNode = node.Attributes["name"];
				return (propertyNameNode==null) ? null : propertyNameNode.Value;
			}
			return null;
		}

		private static void HandleJoinedSubclass( PersistentClass model, Mappings mappings, XmlNode subnode )
		{
			Subclass subclass = new Subclass( model );
			BindJoinedSubclass( subnode, subclass, mappings );
			model.AddSubclass( subclass );
			mappings.AddClass( subclass );
		}

		private static void HandleSubclass( PersistentClass model, Mappings mappings, XmlNode subnode )
		{
			Subclass subclass = new Subclass( model );
			BindSubclass( subnode, subclass, mappings );
			model.AddSubclass( subclass );
			mappings.AddClass( subclass );
		}

		private static PersistentClass GetSuperclass( Mappings model, XmlNode subnode )
		{
			XmlAttribute extendsAttr = subnode.Attributes[ "extends" ];
			if( extendsAttr == null ) 
			{
				throw new MappingException( "'extends' attribute is not found." );
			}
			String extendsValue = FullClassName( extendsAttr.Value, model );
			System.Type superclass;
			try
			{
				superclass = ReflectHelper.ClassForName( extendsValue );
			}
			catch( Exception e )
			{
				throw new MappingException( "extended class not found: " + extendsValue, e );
			}
			PersistentClass superModel = model.GetClass( superclass );

			if( superModel == null )
			{
				throw new MappingException( "Cannot extend unmapped class: " + extendsValue );
			}
			return superModel;
		}

		// TODO: rename this to something like SecondPassBase so we can rename the
		// method secondPass(...) to .net conventions.
		public abstract class SecondPass 
		{
			internal XmlNode node;
			internal Mappings mappings;
			internal Mapping.Collection collection;

			public SecondPass(XmlNode node, Mappings mappings, Mapping.Collection collection) 
			{
				this.node = node;
				this.collection = collection;
				this.mappings = mappings;
			}

			public void DoSecondPass(IDictionary persistentClasses)
			{
				if ( log.IsDebugEnabled ) log.Debug("Second pass for collection: " + collection.Role );
				secondPass(persistentClasses);
				//TODO: h2.1 has a call to collection.createAllKeys(); here
				if ( log.IsDebugEnabled ) 
				{
					string msg = "Mapped collection key: " + Columns( collection.Key );
					if ( collection.IsIndexed ) msg+= ", index: " + Columns( ( (IndexedCollection) collection ).Index );
					if ( collection.IsOneToMany ) 
					{
						msg += ", one-to-many: " + collection.Element.Type.Name;
					}
					else 
					{
						msg += ", element: " + Columns( collection.Element );
						msg += ", type: " + collection.Element.Type.Name;
					}
					log.Debug(msg);
				}			

			}

			public abstract void secondPass(IDictionary persistentClasses);
		}

		private class CollectionSecondPass : SecondPass 
		{
			public CollectionSecondPass(XmlNode node, Mappings mappings, Mapping.Collection collection)
				: base(node, mappings, collection) 
			{ 
			}
			public override void secondPass(IDictionary persistentClasses) 
			{
				Binder.BindCollectionSecondPass(node, collection, persistentClasses, mappings);
			}
		}

		private class IdentifierCollectionSecondPass : SecondPass 
		{
			public IdentifierCollectionSecondPass(XmlNode node, Mappings mappings, Mapping.IdentifierCollection collection) 
				: base(node, mappings, collection)
			{
			}

			public override void secondPass(IDictionary persistentClasses)
			{
				Binder.BindIdentifierCollectionSecondPass( node, (Mapping.IdentifierCollection) collection, persistentClasses, mappings);
			}
		}

		private class MapSecondPass : SecondPass 
		{
			public MapSecondPass(XmlNode node, Mappings mappings, Mapping.Map collection)
				: base(node, mappings, collection) 
			{ 
			}
			public override void secondPass(IDictionary persistentClasses) 
			{
				Binder.BindMapSecondPass(node, (Map) collection, persistentClasses, mappings);
			} 
		}

		private class SetSecondPass : SecondPass 
		{
			public SetSecondPass(XmlNode node, Mappings mappings, Mapping.Set collection)
				: base(node, mappings, collection) 
			{ 
			}
			public override void secondPass(IDictionary persistentClasses) 
			{
				Binder.BindSetSecondPass(node, (Set) collection, persistentClasses, mappings);
			} 
		}

		private class ListSecondPass : SecondPass 
		{
			public ListSecondPass(XmlNode node, Mappings mappings, Mapping.List collection)
				: base(node, mappings, collection) 
			{ 
			}
			public override void secondPass(IDictionary persistentClasses) 
			{
				Binder.BindListSecondPass(node, (List) collection, persistentClasses, mappings);
			} 
		}

		private abstract class CollectionType 
		{
			private string xmlTag;
			public abstract Mapping.Collection Create(XmlNode node, string className, string path, PersistentClass owner, Mappings mappings);

			public CollectionType(string xmlTag) 
			{
				this.xmlTag = xmlTag;
			}

			public override string ToString() 
			{
				return xmlTag;
			}

			private static CollectionType MAP = new CollectionTypeMap("map");
			private class CollectionTypeMap : CollectionType 
			{
				public CollectionTypeMap(string xmlTag) : base(xmlTag) { }
				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, Mappings mappings) 
				{
					Map map = new Map(owner);
					Binder.BindCollection(node, map, prefix, path, mappings);
					return map;
				}
			}

			private static CollectionType SET = new CollectionTypeSet("set");
			private class CollectionTypeSet : CollectionType 
			{
				public CollectionTypeSet(string xmlTag) : base(xmlTag) {}
				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, Mappings mappings) 
				{
					Set setCollection = new Set(owner);
					Binder.BindCollection(node, setCollection, prefix, path, mappings);
					return setCollection;
				}
			}

			private static CollectionType LIST = new CollectionTypeList("list");
			private class CollectionTypeList : CollectionType 
			{
				public CollectionTypeList(string xmlTag) : base(xmlTag) { }
				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, Mappings mappings) 
				{
					List list = new List(owner);
					Binder.BindCollection(node, list, prefix, path, mappings);
					return list;
				}
			}		

			private static CollectionType BAG = new CollectionTypeBag("bag");
			private class CollectionTypeBag : CollectionType 
			{
				public CollectionTypeBag(string xmlTag) : base(xmlTag) {}
				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, Mappings mappings)
				{
					Bag bag = new Bag(owner);
					Binder.BindCollection(node, bag, prefix, path, mappings);
					return bag;
				}

			}

			private static CollectionType IDBAG = new CollectionTypeIdBag("idbag");
			private class CollectionTypeIdBag : CollectionType 
			{
				public CollectionTypeIdBag(string xmlTag) : base(xmlTag) {}
				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, Mappings mappings)
				{
					IdentifierBag bag = new IdentifierBag(owner);
					Binder.BindCollection(node, bag, prefix, path, mappings);
					return bag;
				}

			}

			private static CollectionType ARRAY = new CollectionTypeArray("array");
			private class CollectionTypeArray : CollectionType 
			{
				public CollectionTypeArray(string xmlTag) : base(xmlTag) { }
				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, Mappings mappings) 
				{
					Mapping.Array array = new Mapping.Array(owner);
					Binder.BindArray(node, array, prefix, path, mappings);
					return array;
				}
			}		

			private static CollectionType PRIMITIVE_ARRAY = new CollectionTypePrimitiveArray("primitive-array");
			private class CollectionTypePrimitiveArray : CollectionType 
			{
				public CollectionTypePrimitiveArray(string xmlTag) : base(xmlTag) { }
				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, Mappings mappings) 
				{
					PrimitiveArray array = new PrimitiveArray(owner);
					Binder.BindArray(node, array, prefix, path, mappings);
					return array;
				}
			}	
			
			private static Hashtable Instances = new Hashtable();
			static CollectionType() 
			{
				Instances.Add(MAP.ToString(), MAP);
				Instances.Add(BAG.ToString(), BAG);
				Instances.Add(IDBAG.ToString(), IDBAG);
				Instances.Add(SET.ToString(), SET);
				Instances.Add(LIST.ToString(), LIST);
				Instances.Add(ARRAY.ToString(), ARRAY);
				Instances.Add(PRIMITIVE_ARRAY.ToString(), PRIMITIVE_ARRAY);
			}

			public static CollectionType CollectionTypeFromString(string xmlTagName) 
			{
				return (CollectionType) Instances[xmlTagName];
			}
		}
	}
}
