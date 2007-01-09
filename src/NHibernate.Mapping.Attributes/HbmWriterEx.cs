//
// NHibernate.Mapping.Attributes
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Customized HbmWriter.
	/// Support ComponentPropertyAttribute.
	/// </summary>
	public class HbmWriterEx : HbmWriter
	{
		public virtual System.Collections.ArrayList FindSystemAttributedMembers(System.Type attributeType, System.Type classType)
		{
			// Return all members from the classType (and its base types) decorated with this attributeType
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			System.Reflection.BindingFlags bindings = System.Reflection.BindingFlags.Instance
				| System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly;

			System.Type type = classType;
			while( type != null )
			{
				foreach( System.Reflection.MemberInfo member in type.GetMembers(bindings) )
					if( member.GetCustomAttributes(attributeType, false).Length > 0 )
						list.Add(member);

				type = type.BaseType;
				if( type!=null && ( type.IsDefined(typeof(ComponentAttribute), false) || type.IsDefined(typeof(ClassAttribute), false)
					|| type.IsDefined(typeof(SubclassAttribute), false) || type.IsDefined(typeof(JoinedSubclassAttribute), false) ) )
					break; // don't use members of a mapped base class
			}

			return list;
		}


		public override void WriteUserDefinedContent(System.Xml.XmlWriter writer, System.Type classType, System.Type contentAttributeType, BaseAttribute parentAttribute)
		{
			base.WriteUserDefinedContent(writer, classType, contentAttributeType, parentAttribute);

			// Insert [RawXml] after the specified type of attribute
			System.Collections.ArrayList RawXmlAttributedMembersList = FindSystemAttributedMembers( typeof(RawXmlAttribute), classType );
			foreach( System.Reflection.MemberInfo member in RawXmlAttributedMembersList )
			{
				RawXmlAttribute rawXml = member.GetCustomAttributes(typeof(RawXmlAttribute), false)[0] as RawXmlAttribute;
				if(contentAttributeType != rawXml.After)
					continue;
				if(rawXml.Content==null)
					throw new MappingException("You must specify the content of the RawXmlAttribute on the member: " + member.Name + " of the class " + member.DeclaringType.FullName);

				System.Xml.XmlTextWriter textWriter = writer as System.Xml.XmlTextWriter;
				if(textWriter != null) // TODO: Hack to restore indentation after writing the raw XML
				{
					textWriter.WriteStartElement("!----"); // Write <!---->
					textWriter.Flush();
					textWriter.BaseStream.Flush(); // Note: Seek doesn't work properly here; so the started elt can't be removed
				}

				writer.WriteRaw(rawXml.Content);

				if(textWriter != null) // TODO: Hack to restore indentation after writing the raw XML
				{
					textWriter.WriteEndElement();
					textWriter.Flush();
					textWriter.BaseStream.Flush();
					textWriter.BaseStream.Seek(-8, System.IO.SeekOrigin.Current); // Remove </!---->
				}
			}

			if(contentAttributeType == typeof(ComponentAttribute))
			{
				System.Collections.ArrayList ComponentPropertyList = FindSystemAttributedMembers( typeof(ComponentPropertyAttribute), classType );
				foreach( System.Reflection.MemberInfo member in ComponentPropertyList )
				{
					object[] objects = member.GetCustomAttributes(typeof(ComponentPropertyAttribute), false);
					WriteComponentProperty(writer, member, objects[0] as ComponentPropertyAttribute, parentAttribute);
				}
			}

			if(contentAttributeType == typeof(CacheAttribute)) 
			{
				object[] attributes = classType.GetCustomAttributes(typeof(CacheAttribute), false);
				if(attributes.Length > 0)
					WriteCache(writer, null, (CacheAttribute)attributes[0], parentAttribute, classType);
			}
			if(contentAttributeType == typeof(JcsCacheAttribute)) 
			{
				object[] attributes = classType.GetCustomAttributes(typeof(JcsCacheAttribute), false);
				if(attributes.Length > 0)
					WriteJcsCache(writer, null, (JcsCacheAttribute)attributes[0], parentAttribute, classType);
			}
			if(contentAttributeType == typeof(DiscriminatorAttribute)) 
			{
				object[] attributes = classType.GetCustomAttributes(typeof(DiscriminatorAttribute), false);
				if(attributes.Length > 0)
					WriteDiscriminator(writer, null, (DiscriminatorAttribute)attributes[0], parentAttribute, classType);
			}
			if(contentAttributeType == typeof(KeyAttribute)) 
			{
				object[] attributes = classType.GetCustomAttributes(typeof(KeyAttribute), false);
				if(attributes.Length > 0)
					WriteKey(writer, null, (KeyAttribute)attributes[0], parentAttribute, classType);
			}
		}


		public virtual void WriteComponentProperty(System.Xml.XmlWriter writer, System.Reflection.MemberInfo member, ComponentPropertyAttribute attrib, BaseAttribute parentAttribute)
		{
			System.Type componentType = attrib.ComponentType;
			if(componentType == null)
				if(member is System.Reflection.FieldInfo)
					componentType = (member as System.Reflection.FieldInfo).FieldType;
				else // It MUST be a PropertyInfo
					componentType = (member as System.Reflection.PropertyInfo).PropertyType;

			object[] componentAttributes = componentType.GetCustomAttributes(typeof(ComponentAttribute), false);
			if(componentAttributes.Length == 0)
				throw new MappingException(componentType.FullName + " doesn't have the attribute [Component]!");
			ComponentAttribute componentAttribute = componentAttributes[0] as ComponentAttribute;

			string componentName = attrib.PropertyName;
			if(componentName == null)
				componentName = member.Name; // Default value

			if(componentAttribute.Name != null && componentAttribute.Name != componentName)
				// Because, it will be used by the default implementation of "WriteComponent()"
				throw new MappingException(componentType.FullName + " must have a [Component] with a 'null' Name (or the name '" + componentName + "')");

			// Get the helper to set the componentName
			HbmWriterHelperEx helper = this.DefaultHelper as HbmWriterHelperEx;
			if(helper == null)
				throw new MappingException("DefaultHelper must be a HbmWriterHelperEx (or a subType) to use [ComponentProperty]");

			// Set the value that will be returned when WriteComponent() will call Get_Component_Name_DefaultValue()
			string savedValue = helper.DefaultValue;
			helper.DefaultValue = componentName;
			try
			{
				WriteComponent(writer, componentType);
			}
			finally
			{
				helper.DefaultValue = savedValue;
			}
		}
	}
}
