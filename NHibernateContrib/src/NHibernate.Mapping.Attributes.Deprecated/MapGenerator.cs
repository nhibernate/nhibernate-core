using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for MapGenerator.
	/// </summary>
	public sealed class MapGenerator
	{
		#region Constants
		
		public const string DefaultAccessStrategy = "property";
		
		#endregion
		
		#region Member Variables

		private SortedList m_Types;
		
		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public MapGenerator()
		{
			m_Types = new SortedList();
		}
		
		/// <summary>
		/// Adds the assembly's types to the generator.
		/// </summary>
		/// <param name="assembly"></param>
		public void AddAssembly( string assembly )
		{
			this.AddAssembly( Assembly.Load( assembly ) );
		}
		
		/// <summary>
		/// Adds the assembly's types to the generator.
		/// </summary>
		/// <param name="assembly"></param>
		public void AddAssembly( Assembly assembly )
		{
			System.Type[] types = assembly.GetTypes();
			foreach( System.Type type in types )
			{
				if( type.IsClass )
				{
					AddClass( type );
				}
			}
		}
		
		/// <summary>
		/// Adds the class type to the generator.
		/// </summary>
		/// <param name="type"></param>
		public void AddClass( System.Type type )
		{
			if( type.IsClass )
			{
				if( type.IsDefined( typeof(ClassAttribute), true ) ||
						type.IsDefined( typeof(JoinedSubclassAttribute), true ) ||
						type.IsDefined( typeof(SubclassAttribute), true ) )
				{
					m_Types.Add( new TypeKey( type ), type );
				}
			}
			else
			{
				throw new ArgumentException( "Invalid type. Only class types can be added to the generator.", "type" );
			}
		}
		
		/// <summary>
		/// Generates the mapping.
		/// </summary>
		/// <param name="map"></param>
		public void Generate( Stream map )
		{
			//
			//create the stream we will use to write the xml into
			//
			XmlWriter writer = new XmlTextWriter( map, System.Text.Encoding.UTF8 );
			writer.WriteComment( String.Format( "Generated on {0}.", DateTime.Now ) );
			writer.WriteStartElement("hibernate-mapping", "urn:nhibernate-mapping-2.0" );
			
			//
			//write each type into the stream
			//
			TypeNodeCollection nodes = BuildTypeDependencyTree( m_Types );
			foreach( TypeNode node in nodes )
			{
				WriteTypeNode( writer, node );
			}
			
			writer.WriteEndElement(); //</hibernate-mapping>
			writer.Flush();
		}
		
		/// <summary>
		/// Writes the type's map information into the xml writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="node"></param>
		private void WriteTypeNode( XmlWriter writer, TypeNode node )
		{
			bool looking = true;
			System.Type type = node.Type;
			while( looking )
			{
				if( Attribute.IsDefined( type, typeof(ClassAttribute), false ) )
				{
					writer.WriteComment( GetShortTypeName( node.Type ) );
					WriteClass( writer, node, type );
					looking = false;
				}
				else if( Attribute.IsDefined( type, typeof(JoinedSubclassAttribute), false ) )
				{
					writer.WriteComment( GetShortTypeName( node.Type ) );
					WriteJoinedSubclass( writer, node, type );
					looking = false;
				}						
				else if( Attribute.IsDefined( type, typeof(SubclassAttribute), false ) )
				{
					writer.WriteComment( GetShortTypeName( node.Type ) );
					WriteSubclass( writer, node, type );
					looking = false;
				}
				else
				{
					type = type.BaseType;
					looking = true;
				}
			}
		}
		
		/// <summary>
		/// Writes the collection.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="type"></param>
		private void WriteCollections( XmlWriter writer, System.Type type )
		{
			PropertyInfo[] listProps = FindAttributedProperties( typeof(ListAttribute), type );
			foreach( PropertyInfo listProp in listProps )
			{
				WriteList( writer, listProp );
			}

			PropertyInfo[] setProps = FindAttributedProperties( typeof(SetAttribute), type );
			foreach( PropertyInfo setProp in setProps )
			{
				WriteSet( writer, setProp );
			}

		}
		
		/// <summary>
		/// Writes the list.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="parentNode"></param>
		private void WriteList( XmlWriter writer, PropertyInfo property )
		{
			ListAttribute attribute = (ListAttribute)Attribute.GetCustomAttribute( property, typeof(ListAttribute) );
			writer.WriteStartElement( "list" );
			writer.WriteAttributeString( "name", property.Name );
			writer.WriteAttributeString( "table", attribute.Table );
			writer.WriteAttributeString( "access", attribute.Access );
			if( attribute.IsLazy ) writer.WriteAttributeString( "lazy", "true" );
			if( attribute.IsInverse ) writer.WriteAttributeString( "inverse", "true" );
			WriteCollectionKey( writer, property );
			WriteCollectionIndex( writer, property );
			WriteCollectionOneToMany( writer, property );			
			WriteCollectionCompositeElement( writer, property );
			WriteCollectionManyToMany( writer, property );
			writer.WriteEndElement(); //<list>
		}

		/// <summary>
		/// Writes the list.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="parentNode"></param>
		private void WriteSet( XmlWriter writer, PropertyInfo property )
		{
			SetAttribute attribute = (SetAttribute)Attribute.GetCustomAttribute( property, typeof(SetAttribute) );
			writer.WriteStartElement( "set" );
			writer.WriteAttributeString( "name", property.Name );
			writer.WriteAttributeString( "table", attribute.Table );
			writer.WriteAttributeString( "access", attribute.Access );
			if( attribute.IsLazy ) writer.WriteAttributeString( "lazy", "true" );
			WriteCollectionKey( writer, property );
			WriteCollectionOneToMany( writer, property );			
			WriteCollectionCompositeElement( writer, property );
			writer.WriteEndElement(); //<list>
		}		
		
		/// <summary>
		/// Writes the collection key.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="property"></param>
		private void WriteCollectionKey( XmlWriter writer, PropertyInfo property )
		{
			CollectionKeyAttribute attribute = (CollectionKeyAttribute)Attribute.GetCustomAttribute( property, typeof(CollectionKeyAttribute) );
			if( attribute != null )
			{
				writer.WriteStartElement( "key" );
				writer.WriteAttributeString( "column", attribute.Column );
				writer.WriteEndElement(); //</key>
			}
		}
		
		/// <summary>
		/// Writes the collection index.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="type"></param>
		private void WriteDiscriminator( XmlWriter writer, System.Type type )
		{
			DiscriminatorAttribute attribute = (DiscriminatorAttribute)Attribute.GetCustomAttribute( type, typeof(DiscriminatorAttribute) );
			if( attribute != null )
			{
				writer.WriteStartElement( "discriminator" );
				writer.WriteAttributeString( "column", attribute.Column );
				writer.WriteAttributeString( "type", GetShortTypeName( attribute.Type ) );
				
				if( attribute.Force )
					writer.WriteAttributeString( "force", "true" );
					
				writer.WriteEndElement(); //</discriminator>
			}		
		}
		
		/// <summary>
		/// Writes the collection index.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="property"></param>
		private void WriteCollectionIndex( XmlWriter writer, PropertyInfo property )
		{
			CollectionIndexAttribute attribute = (CollectionIndexAttribute)Attribute.GetCustomAttribute( property, typeof(CollectionIndexAttribute) );
			if( attribute != null )
			{
				writer.WriteStartElement( "index" );
				writer.WriteAttributeString( "column", attribute.Column );
				writer.WriteEndElement(); //</index>
			}		
		}
		
		/// <summary>
		/// Writes the collection composite element.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="property"></param>
		private void WriteCollectionCompositeElement( XmlWriter writer, PropertyInfo property )
		{
			CollectionCompositeElementAttribute attribute = (CollectionCompositeElementAttribute)Attribute.GetCustomAttribute( property, typeof(CollectionCompositeElementAttribute) );
			if( attribute != null )
			{
				writer.WriteStartElement( "composite-element" );
				writer.WriteAttributeString( "class", GetShortTypeName( attribute.Type ) );
				WriteProperties( writer, attribute.Type );
				writer.WriteEndElement(); //</composite-element>
			}		
		}

		/// <summary>
		/// Writes the collection many to many.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="property"></param>
		private void WriteCollectionManyToMany( XmlWriter writer, PropertyInfo property )
		{
			CollectionManyToManyAttribute attribute = (CollectionManyToManyAttribute)Attribute.GetCustomAttribute( property, typeof(CollectionManyToManyAttribute) );
			if( attribute != null )
			{
				writer.WriteStartElement( "many-to-many" );
				writer.WriteAttributeString( "column", attribute.Column );
				writer.WriteAttributeString( "class", GetShortTypeName( attribute.Type ) );
				writer.WriteEndElement(); //</many-to-many>
			}		
		}

		/// <summary>
		/// Writes the collection one to many tag.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="property"></param>
		private void WriteCollectionOneToMany( XmlWriter writer, PropertyInfo property )
		{
			CollectionOneToManyAttribute attribute = (CollectionOneToManyAttribute)Attribute.GetCustomAttribute( property, typeof(CollectionOneToManyAttribute) );
			if( attribute != null )
			{
				writer.WriteStartElement( "one-to-many" );
				writer.WriteAttributeString( "class", GetShortTypeName( attribute.Type ) );
				writer.WriteEndElement(); //<one-to-many>
			}
		}
				
		/// <summary>
		/// Writes the subclass.
		/// </summary>
		private void WriteSubclass( XmlWriter writer, TypeNode node, System.Type type )
		{
			SubclassAttribute attribute = (SubclassAttribute)Attribute.GetCustomAttribute( type, typeof(SubclassAttribute), true );
			writer.WriteStartElement( "subclass" );
			writer.WriteAttributeString( "name", GetShortTypeName( node.Type ) );
			
			if( attribute.DiscriminatorValue != null )
				writer.WriteAttributeString( "discriminator-value", attribute.DiscriminatorValue );			
				
			if( attribute.DynamicInsert )
				writer.WriteAttributeString( "dynamic-insert", "true" );
				
			if( attribute.DynamicUpdate )
				writer.WriteAttributeString( "dynamic-update", "true" );		
			
			WriteProperties( writer, type );
			WriteCollections( writer, type );
			WriteAssociations( writer, type );
			
			foreach( TypeNode childNode in node.Nodes )
			{
				WriteTypeNode( writer, childNode );
			}
			
			writer.WriteEndElement(); //</subclass>
		}
		
		/// <summary>
		/// Writes the class.
		/// </summary>
		private void WriteClass( XmlWriter writer, TypeNode node, System.Type type )
		{
			ClassAttribute attribute = (ClassAttribute)Attribute.GetCustomAttribute( type, typeof(ClassAttribute), true );
			writer.WriteStartElement( "class" );
			writer.WriteAttributeString( "name", GetShortTypeName( node.Type ) );
			writer.WriteAttributeString( "table", attribute.Table );
			
			if( attribute.Polymorphism != ClassPolymorphismType.Implicit )
				writer.WriteAttributeString( "polymorphism", GetXmlEnumValue( typeof(ClassPolymorphismType), attribute.Polymorphism ) );
			
			if( attribute.DiscriminatorValue != null )
				writer.WriteAttributeString( "discriminator-value", attribute.DiscriminatorValue );
			
			if( attribute.SqlWhere != null )
				writer.WriteAttributeString( "where", attribute.SqlWhere );
			
			WriteId( writer, node.Type );
			WriteDiscriminator( writer, type );
			WriteVersion( writer, type );
			WriteProperties( writer, type );
			WriteCollections( writer, type );
			WriteAssociations( writer, type );
			
			foreach( TypeNode childNode in node.Nodes )
			{
				WriteTypeNode( writer, childNode );
			}
			
			writer.WriteEndElement(); //</class>
		}

		/// <summary>
		/// Writes the property.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="property"></param>
		private void WriteProperty( XmlWriter writer, PropertyInfo property )
		{
			PropertyAttribute attribute = (PropertyAttribute)Attribute.GetCustomAttribute( property, typeof(PropertyAttribute) );
			writer.WriteStartElement( "property" );
			writer.WriteAttributeString( "name", property.Name );
			writer.WriteAttributeString( "column", attribute.Column == null ? property.Name : attribute.Column );
			writer.WriteAttributeString( "access", attribute.Access );
			
			string typeName = null;
			if( attribute.Type != null )
				typeName = GetShortTypeName( attribute.Type );
			else
				typeName = GetShortTypeName( property.PropertyType );
			
			if( attribute.Size > 0 )
			{
				writer.WriteAttributeString( "type", String.Format( "{0}({1})", typeName, attribute.Size ) );
			}
			else
			{
				writer.WriteAttributeString( "type", typeName );
			}
			
			if( !attribute.Insert )
				writer.WriteAttributeString( "insert", "false" );
			
			if( !attribute.Update )
				writer.WriteAttributeString( "update", "false" );			
			
			//if( attribute.Size > 0 ) 
			//	writer.WriteAttributeString( "length", attribute.Size.ToString() );
			
			writer.WriteEndElement(); //</property>
		}		

		/// <summary>
		/// Writers the properties.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="parentNode"></param>
		private void WriteProperties( XmlWriter writer, System.Type type )
		{
			PropertyInfo[] properties = FindAttributedProperties( typeof(PropertyAttribute), type );
			foreach( PropertyInfo property in properties )
			{
				WriteProperty( writer, property );
			}
		}
		
		/// <summary>
		/// Writes the assoications.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="type"></param>
		private void WriteAssociations( XmlWriter writer, System.Type type )
		{
			PropertyInfo[] properties = FindAttributedProperties( typeof(ManyToOneAttribute), type );
			foreach( PropertyInfo property in properties )
			{
				WriteManyToOne( writer, property );
			}		
		}
		
		/// <summary>
		/// Writes the many to one association.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="property"></param>
		private void WriteManyToOne( XmlWriter writer, PropertyInfo property )
		{
			ManyToOneAttribute attribute = (ManyToOneAttribute)Attribute.GetCustomAttribute( property, typeof(ManyToOneAttribute) );
			if( attribute.Inheritable || ( !attribute.Inheritable && ( property.DeclaringType == property.ReflectedType ) ) )
			{
				writer.WriteStartElement( "many-to-one" );
				writer.WriteAttributeString( "name", property.Name );
				writer.WriteAttributeString( "column", attribute.Column == null ? property.Name : attribute.Column );
				writer.WriteAttributeString( "access", attribute.Access );
				
				string typeName = null;
				if( attribute.Type != null )
					typeName = GetShortTypeName( attribute.Type );
				else
					typeName = GetShortTypeName( property.PropertyType );
				
				writer.WriteAttributeString( "class", typeName );

				if( attribute.Cascade != CascadeType.None )
					writer.WriteAttributeString( "cascade", GetXmlEnumValue( typeof(CascadeType), attribute.Cascade ) );
				
				if( attribute.OuterJoin != OuterJoinType.Auto )
					writer.WriteAttributeString( "outer-join", GetXmlEnumValue( typeof(OuterJoinType), attribute.OuterJoin ) );
							
				if( !attribute.Insert )
					writer.WriteAttributeString( "insert", "false" );
				
				if( !attribute.Update )
					writer.WriteAttributeString( "update", "false" );			
							
				writer.WriteEndElement(); //</many-to-one>
			}
		}
		
		/// <summary>
		/// Writes the version.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="parentNode"></param>
		private void WriteVersion( XmlWriter writer, System.Type type )
		{
			PropertyInfo property = FindAttributedProperty( typeof(VersionAttribute), type );
			if( property != null )
			{
				VersionAttribute attribute = (VersionAttribute)Attribute.GetCustomAttribute( property, typeof(VersionAttribute), false );
				writer.WriteStartElement( "version" );
				writer.WriteAttributeString( "name", property.Name );
				writer.WriteAttributeString( "column", attribute.Column );
				writer.WriteAttributeString( "type", GetShortTypeName( property.PropertyType ) );
				writer.WriteAttributeString( "access", attribute.Access );
				writer.WriteEndElement(); //</version>
			}		
		}
		
		/// <summary>
		/// Writes the id.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="parentNode"></param>
		private void WriteId( XmlWriter writer, System.Type type )
		{
			PropertyInfo property = FindAttributedProperty( typeof(IdAttribute), type );
			if( property == null ) throw new Exception( "Missing required IdAttribute." );

			IdAttribute attribute = (IdAttribute)Attribute.GetCustomAttribute( property, typeof(IdAttribute), false );
			writer.WriteStartElement( "id" );
			writer.WriteAttributeString( "name", property.Name );
			writer.WriteAttributeString( "type", GetShortTypeName( property.PropertyType ) );
			writer.WriteAttributeString( "column", attribute.Column );
			writer.WriteAttributeString( "access", attribute.Access );
			
			if( attribute.UnsavedValueType == UnsavedValueType.Specified )
				writer.WriteAttributeString( "unsaved-value", attribute.UnsavedValue );
			else
				writer.WriteAttributeString( "unsaved-value", GetXmlEnumValue( typeof(UnsavedValueType), attribute.UnsavedValueType ) );

			writer.WriteStartElement("generator");
			writer.WriteAttributeString( "class", GetShortTypeName( attribute.Generator ) );
			WriteGeneratorParameters( writer, property );
			writer.WriteEndElement(); //</generator>
			writer.WriteEndElement(); //</id>
		}
		
		/// <summary>
		/// Writes the generator parameters.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="property"></param>
		private void WriteGeneratorParameters( XmlWriter writer, PropertyInfo property )
		{
			GeneratorParameterAttribute[] attributes = (GeneratorParameterAttribute[])Attribute.GetCustomAttributes( property, typeof(GeneratorParameterAttribute), true );
			foreach( GeneratorParameterAttribute attribute in attributes )
			{
				writer.WriteStartElement( "param" );
				writer.WriteAttributeString( "name", attribute.Name );
				writer.WriteString( attribute.Value );
				writer.WriteEndElement(); //</param>
			}
		}
		
		/// <summary>
		/// Writes the joined subclass.
		/// </summary>
		private void WriteJoinedSubclass( XmlWriter writer, TypeNode node, System.Type type )
		{
			JoinedSubclassAttribute attribute = (JoinedSubclassAttribute)Attribute.GetCustomAttribute( type, typeof(JoinedSubclassAttribute), true );
			writer.WriteStartElement( "joined-subclass" );
			writer.WriteAttributeString( "name", GetShortTypeName( node.Type ) );
			writer.WriteAttributeString( "table", attribute.Table );			
			WriteJoinedSubclassKey( writer, type );			
			WriteProperties( writer, type );
			WriteCollections( writer, type );
			WriteAssociations( writer, type );
			
			foreach( TypeNode childNode in node.Nodes )
			{
				WriteTypeNode( writer, childNode );
			}
			
			writer.WriteEndElement(); //</joined-subclass>
		}
		
		/// <summary>
		/// Writes the joined subclass key.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="parentNode"></param>
		private void WriteJoinedSubclassKey( XmlWriter writer, System.Type type )
		{
			JoinedSubclassKeyAttribute attribute = (JoinedSubclassKeyAttribute)Attribute.GetCustomAttribute( type, typeof(JoinedSubclassKeyAttribute), false );
			writer.WriteStartElement( "key" );
			writer.WriteAttributeString( "column", attribute.Column );
			writer.WriteEndElement(); //</key>
		}
		
		/// <summary>
		/// Constructs an array of TypeNodes.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private TypeNodeCollection BuildTypeDependencyTree( SortedList list )
		{
			TypeNodeCollection nodes = new TypeNodeCollection();
			Hashtable lookup = new Hashtable();
			
			foreach( DictionaryEntry entry in list )
			{
				TypeKey key = (TypeKey)entry.Key;
				TypeNode node = new TypeNode( key.Type );
				lookup.Add( node.Type, node );
				if( key.ExtendsType != null )
				{
					TypeNode extendNode = (TypeNode)lookup[key.ExtendsType];
					extendNode.Nodes.Add( node );
				}
				else
				{
					nodes.Add( node );
				}
			}

			return nodes;
		}
		
		/// <summary>
		/// Searches the members of the class for any member with the attribute defined.
		/// </summary>
		/// <param name="attribute"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private PropertyInfo FindAttributedProperty( System.Type attrType, System.Type classType )
		{
			System.Type type = classType;
			while( ( type == classType ) || ( type != null ) ) 
			{
				PropertyInfo[] properties = type.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );
				foreach( PropertyInfo property in properties )
				{
					if( property.IsDefined( attrType, false ) )
					{
						return property;
					}
				}
				type = type.BaseType;
			}
			return null;
		}
		
		/// <summary>
		/// Searches the members of the class for any member with the attribute defined.
		/// </summary>
		/// <param name="attrType"></param>
		/// <param name="classType"></param>
		/// <returns></returns>
		private PropertyInfo[] FindAttributedProperties( System.Type attrType, System.Type classType )
		{
			ArrayList list = new ArrayList();
			BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			
			System.Type type = classType;
			while( ( type == classType ) || ( type != null ) )
			{
				PropertyInfo[] properties = type.GetProperties( bindings );
				foreach( PropertyInfo property in properties )
				{
					if( property.IsDefined( attrType, false ) )
					{
						list.Add( property );
					}
				}
				type = type.BaseType;
			}
			
			PropertyInfo[] array = new PropertyInfo[list.Count];
			list.CopyTo( array );
			return array;
		}
		
		/// <summary>
		/// Converts the full name into a short name.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private string GetShortTypeName( System.Type type )
		{
			string typeName = type.FullName;
			string assemblyName = type.Assembly.GetName().Name;
			
			if( assemblyName == "mscorlib" && 
					typeName.StartsWith( "System." ) &&
					typeName.IndexOf(".") == typeName.LastIndexOf(".") )
			{
				return typeName.Substring( typeName.LastIndexOf(".") + 1 );
			}
			else
			{
				return String.Format( "{0}, {1}",typeName, assemblyName );
			}
		}
		
		/// <summary>
		/// Gets the xml enum value from the associated attributed enum.
		/// </summary>
		/// <param name="enumType"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private string GetXmlEnumValue( System.Type enumType, object @value )
		{
			FieldInfo[] fields = enumType.GetFields();
			foreach( FieldInfo field in fields )
			{
				if( field.Name == Enum.GetName( enumType, @value ) )
				{
					XmlEnumAttribute attribute = (XmlEnumAttribute)Attribute.GetCustomAttribute( field, typeof(XmlEnumAttribute), false );
					if( attribute != null )
					{
						return attribute.Name;
					}
					else
					{
						throw new ApplicationException( String.Format( "{0} is missing XmlEnumAttribute on {1} value.", enumType, @value ) );
					}
				}
			}
			throw new MissingFieldException( enumType.ToString(), @value.ToString() );
		}
				
	}
}
