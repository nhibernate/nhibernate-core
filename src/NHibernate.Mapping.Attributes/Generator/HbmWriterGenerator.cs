//
// NHibernate.Mapping.Attributes.Generator
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes.Generator
{
	/// <summary>
	/// Generate the HbmWriter class.
	/// </summary>
	public class HbmWriterGenerator
	{
		/// <summary> Log (debug) infos, warnings and errors </summary>
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );


		/// <summary> Generate a Writer method for a XmlSchemaElement. </summary>
		public static void GenerateElementWriter(System.Xml.Schema.XmlSchemaElement schemaElt, string schemaEltName, Refly.CodeDom.MethodDeclaration method, System.Xml.Schema.XmlSchemaComplexType refSchemaType)
		{
			bool schemaEltIsRoot = Utils.IsRoot(schemaEltName);
			method.Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Overloaded;
			method.Doc.Summary.AddText(" Write a " + schemaEltName + " XML Element from attributes in a " + (schemaEltIsRoot?"type":"member") + ". "); // Create the <summary />
			method.Signature.Parameters.Add(typeof(System.Xml.XmlWriter), "writer");
			if(schemaEltIsRoot)
				method.Signature.Parameters.Add(typeof(System.Type), "type");
			else
			{
				method.Signature.Parameters.Add(typeof(System.Reflection.MemberInfo), "member");
				method.Signature.Parameters.Add(schemaEltName + "Attribute", "attribute");
				method.Signature.Parameters.Add("BaseAttribute", "parentAttribute");
			}

			// Beginning of the method's body
			if(schemaEltIsRoot)
			{
				method.Body.Add(Refly.CodeDom.Stm.Snippet(string.Format(
					@"object[] attributes = {1}.GetCustomAttributes(typeof({0}Attribute), true);
			if(attributes.Length == 0)
				return;
			{0}Attribute attribute = attributes[0] as {0}Attribute;
", schemaEltName, schemaEltIsRoot ? "type" : "member"))); // Note : Root should never allow multiple !
			}
			method.Body.Add(Refly.CodeDom.Stm.Snippet( string.Format(
				"writer.WriteStartElement( \"{0}\" );", schemaElt.Name) ));


			System.Xml.Schema.XmlSchemaComplexType type = schemaElt.SchemaType as System.Xml.Schema.XmlSchemaComplexType;
			if(type == null && !schemaElt.SchemaTypeName.IsEmpty) // eg:  <xs:element name="cache" type="cacheType" />
				type = refSchemaType;

			if(type != null)
			{
				// Add the attributes
				System.Collections.ArrayList attributes = Utils.GetAttributes(type.Attributes);
				log.Debug("Add Attributes: Count=" + attributes.Count);
				foreach(System.Xml.Schema.XmlSchemaAttribute attrib in attributes)
				{
					string attribName = attrib.Name + attrib.RefName.Name; // One is empty
					string methodAttribName = "attribute." + Utils.Capitalize(attribName);
					method.Body.Add(Refly.CodeDom.Stm.Comment("Attribute: <" + attribName + ">"));

					// If the attribute is not explicitly marked required, then we consider it optionnal
					if(attrib.Use != System.Xml.Schema.XmlSchemaUse.Required)
					{
						if( "System.Boolean" == Utils.GetAttributeTypeName(schemaElt, attrib) )
							method.Body.Add(Refly.CodeDom.Stm.Snippet(
								string.Format( @"if( {0} )", methodAttribName + "Specified" ) ));
						else
							method.Body.Add(Refly.CodeDom.Stm.Snippet(
								string.Format( @"if({0} != {1})", methodAttribName,
								Utils.GetUnspecifiedValue(schemaElt, attrib)) ));
					}
					// Write the value
					if(attrib.Use == System.Xml.Schema.XmlSchemaUse.Required)
					{
						// Here, we use a helper if the field is not specified (mainly used for "name" which is the name of the member)
						log.Debug("  Create Helper for " + (schemaEltIsRoot ? "ClassType" : "MemberInfo") + " <" + schemaElt.Name + "> <" + attribName + ">");
						string helper = string.Format( @"{0}=={1} ? DefaultHelper.Get_{2}_{3}_DefaultValue({4}) : ", // "typed" method name :)
							methodAttribName, Utils.GetUnspecifiedValue(schemaElt, attrib), schemaEltName,
							Utils.Capitalize(attribName),
							schemaEltIsRoot ? "type" : "member" );
						method.Body.Add(Refly.CodeDom.Stm.Snippet(
							string.Format( @"writer.WriteAttributeString(""{0}"", {2}{1});",
							attribName, AttributeToXmlValue(schemaElt, attrib, attribName), helper) ));
					}
					else
					{
						method.Body.Add(Refly.CodeDom.Stm.Snippet(
							string.Format( @"writer.WriteAttributeString(""{0}"", {1});",
							attribName, AttributeToXmlValue(schemaElt, attrib, attribName)) ));
						if( schemaEltName=="Property" && attribName=="type" )
						{
							// Special case to set Nullables.NHibernate.NullableXXXType for Nullables.NullableXXX
							method.Body.Add(Refly.CodeDom.Stm.Snippet(@"else
			{
				System.Type type = null;
				string typeName = string.Empty;
				if(member is System.Reflection.PropertyInfo)
					type = (member as System.Reflection.PropertyInfo).PropertyType;
				else if(member is System.Reflection.FieldInfo)
					type = (member as System.Reflection.FieldInfo).FieldType;
				typeName = type.FullName + "", "" + type.Assembly.GetName().Name;
				if(type!=null && System.Text.RegularExpressions.Regex.IsMatch(typeName, @""Nullables.Nullable(\w+), Nullables""))
					writer.WriteAttributeString( ""type"",
						System.Text.RegularExpressions.Regex.Replace(typeName,
							@""Nullables.Nullable(\w+), Nullables"",
							""Nullables.NHibernate.Nullable$1Type, Nullables.NHibernate"") );
			}"));
						}
					}
				}

				// Add the elements
				if(type.Particle is System.Xml.Schema.XmlSchemaSequence)
				{
					System.Xml.Schema.XmlSchemaSequence seq = (schemaElt.SchemaType as System.Xml.Schema.XmlSchemaComplexType).Particle as System.Xml.Schema.XmlSchemaSequence;
					System.Collections.ArrayList members = Utils.GetElements(seq.Items);
					if( ! schemaEltIsRoot )
						method.Body.Add(Refly.CodeDom.Stm.Snippet( string.Format( @"
			System.Collections.ArrayList memberAttribs = GetSortedAttributes(member);
			int attribPos; // Find the position of the {0}Attribute (its <sub-element>s must be after it)
			for(attribPos=0; attribPos<memberAttribs.Count; attribPos++)
				if( memberAttribs[attribPos] is {0}Attribute
					&& ((BaseAttribute)memberAttribs[attribPos]).Position == attribute.Position )
					break; // found
			int i = attribPos + 1;
", schemaEltName ) ));

					foreach(System.Xml.Schema.XmlSchemaElement member in members)
					{
						if( member.RefName.Name == string.Empty )
							continue; // Ignore elements like <query> and <sql-query>
						string realMemberName = member.Name + member.RefName.Name; // One of them is empty
						string memberName = Utils.Capitalize(realMemberName);
						string listName = memberName + "List";
						string attributeName = memberName + "Attribute";
						log.Debug(schemaEltName + " Element: <" + realMemberName + ">");
						method.Body.Add(Refly.CodeDom.Stm.Comment("Element: <" + realMemberName + ">"));
						// There are three way to treat elements:
						// if(eltName is root)
						//     if(memberName is root)
						//         => They are both root, so we process the nestedTypes (eg. <class> to <sub-class>)
						//     else
						//         => It is an element of a root => we add them as element
						// else
						//     => They are both members, so we use the member again (eg. <list> to <one-to-many>)
						if(schemaEltIsRoot)
						{
							if(Utils.IsRoot(memberName))
							{
								method.Body.Add(Refly.CodeDom.Stm.Snippet( string.Format(
									@"foreach(System.Type nestedType in type.GetNestedTypes(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
				Write{0}(writer, nestedType);", memberName ) ));
							}
							else
							{
								method.Body.Add(Refly.CodeDom.Stm.Snippet( string.Format(
			@"System.Collections.ArrayList {1} = FindAttributedMembers( attribute, typeof({2}), type );
			foreach( System.Reflection.MemberInfo member in {1} )
			", memberName, listName, attributeName ) +"{"
								+ string.Format(
			@"
				object[] objects = member.GetCustomAttributes(typeof({2}), true);
				System.Collections.ArrayList memberAttribs = new System.Collections.ArrayList();
				memberAttribs.AddRange(objects);
				memberAttribs.Sort();
"								+ ( Utils.CanContainItself(memberName) ?
									// => Just take the first (others will be inside it)
@"				Write{0}(writer, member, memberAttribs[0] as {2}, attribute);"
									:
@"				foreach(object memberAttrib in memberAttribs)
					Write{0}(writer, member, memberAttrib as {2}, attribute);" ),
									memberName, listName, attributeName ) + @"
			}" ));
							}
						}
						else
						{
							method.Body.Add(Refly.CodeDom.Stm.Snippet(
			@"for(; i<memberAttribs.Count; i++)
			{
				BaseAttribute memberAttrib = memberAttribs[i] as BaseAttribute;
				if( IsNextElement(memberAttrib, parentAttribute, attribute.GetType())
					|| IsNextElement(memberAttrib, attribute, typeof(" + attributeName + @")) )
					break; // next attributes are 'elements' of the same level OR for 'sub-elements'"
								+ ( Utils.IsRoot(memberName) ? "" // A NotRoot can not contain a Root! eg. [DynComp] can't contain a [Comp] (instead, use [CompProp])
								: @"
				else
				{"
								+ ( Utils.CanContainItself(memberName) ? "" : @"
					if( memberAttrib is " + schemaEltName + @"Attribute )
						break; // Following attributes are for this " + schemaEltName ) + @"
					if( memberAttrib is " + attributeName + @" )
						Write" + memberName + @"(writer, member, memberAttrib as " + attributeName + @", attribute);
				}" ) + @"
			}" ));
						}
					}
				}

				if(type.IsMixed) // used by elements like <param>
				{
					method.Body.Add(Refly.CodeDom.Stm.Snippet(@"
			// Write the content of this element (mixed=""true"")
			writer.WriteRaw(attribute.Content);"));
				}
			}
			else
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("Unknow Element: ").Append(schemaElt.Name);
				if(schemaElt.SchemaType != null)
					sb.Append(", SchemaType = ").Append(schemaElt.SchemaType).Append(" - ").Append(schemaElt.SchemaType.Name);
				if(!schemaElt.SchemaTypeName.IsEmpty)
					sb.Append(", SchemaTypeName = ").Append(schemaElt.SchemaTypeName.Name);
				if(schemaElt.ElementType != null)
					sb.Append(", ElementType = ").Append(schemaElt.ElementType);
				log.Warn(sb.ToString());
			}

			method.Body.Add(Refly.CodeDom.Stm.Snippet(@"
			writer.WriteEndElement();"));
		}


		/// <summary> Add the content of the method FindAttributedMembers(). </summary>
		public static void FillFindAttributedMembers(Refly.CodeDom.MethodDeclaration method)
		{
			method.Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Overloaded;
			method.Doc.Summary.AddText(" Searches the members of the class for any member with the attribute defined. "); // Create the <summary />
			method.Signature.Parameters.Add("BaseAttribute", "rootAttrib");
			method.Signature.Parameters.Add(typeof(System.Type), "attributeType");
			method.Signature.Parameters.Add(typeof(System.Type), "classType");
			method.Signature.ReturnType = new Refly.CodeDom.TypeTypeDeclaration(typeof(System.Collections.ArrayList));

			method.Body.Add(Refly.CodeDom.Stm.Snippet(
			@"// Return all members from the classType (and its base types) decorated with this attributeType
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			System.Reflection.BindingFlags bindings = System.Reflection.BindingFlags.Instance
				| System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly;

			System.Type type = classType;
			while( type != null )
			{
				foreach( System.Reflection.MemberInfo member in type.GetMembers(bindings) )
				{
					foreach(BaseAttribute memberAttrib in GetSortedAttributes(member))
					{
						if( IsNextElement(memberAttrib, rootAttrib, attributeType) )
							break; // next attributes are for <sub-elements>
						else if( memberAttrib.GetType() == attributeType || memberAttrib.GetType().IsSubclassOf(attributeType) )
						{
							list.Add( member );
							break;
						}
					}
				}
				type = type.BaseType;
				if( type!=null && ( type.IsDefined(typeof(ComponentAttribute), false) || type.IsDefined(typeof(ClassAttribute), false)
					|| type.IsDefined(typeof(SubclassAttribute), false) || type.IsDefined(typeof(JoinedSubclassAttribute), false) ) )
					break; // don't use members of a mapped base class
			}

			return list;"));
			// -----------------------------------------------------------------
		}



		/// <summary> Add the content of the method GetSortedAttributes(). </summary>
		public static void FillGetSortedAttributes(Refly.CodeDom.MethodDeclaration method)
		{
			method.Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Overloaded;
			method.Doc.Summary.AddText(" Return all (BaseAttribute derived) attributes in the MethodDeclaration sorted using their position. "); // Create the <summary />
			method.Signature.Parameters.Add(typeof(System.Reflection.MemberInfo), "member");
			method.Signature.ReturnType = new Refly.CodeDom.TypeTypeDeclaration(typeof(System.Collections.ArrayList));

			method.Body.Add(Refly.CodeDom.Stm.Snippet(
				@"System.Collections.ArrayList list = new System.Collections.ArrayList();
			foreach(object attrib in member.GetCustomAttributes(false))
				if(attrib is BaseAttribute)
					list.Add(attrib);
			list.Sort();
			return list;"));
			// -----------------------------------------------------------------
		}



		/// <summary> Add the content of the method IsNextElement(). </summary>
		public static void FillIsNextElement(Refly.CodeDom.MethodDeclaration method, System.Xml.Schema.XmlSchemaObjectCollection items)
		{
			method.Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Overloaded;
			method.Doc.Summary.AddText(" Tells if 'element1' come after 'element2' in rootType's 'sub-elements' order. "); // Create the <summary />
			method.Signature.Parameters.Add("BaseAttribute", "element1");
			method.Signature.Parameters.Add("BaseAttribute", "baseAttrib");
			method.Signature.Parameters.Add(typeof(System.Type), "typeOfElement2");
			method.Signature.ReturnType = new Refly.CodeDom.TypeTypeDeclaration(typeof(bool));

			method.Body.Add(Refly.CodeDom.Stm.Snippet(
			@"if( element1 == null )
				return false;" ));

			foreach(System.Xml.Schema.XmlSchemaObject obj in items)
			{
				if(obj is System.Xml.Schema.XmlSchemaElement)
				{
					System.Xml.Schema.XmlSchemaElement elt = obj as System.Xml.Schema.XmlSchemaElement;
					method.Body.Add(Refly.CodeDom.Stm.Snippet( @"
			if( baseAttrib is " + Utils.Capitalize(elt.Name) + @"Attribute )
			{" ));

					if(elt.SchemaType is System.Xml.Schema.XmlSchemaComplexType)
					{
						System.Xml.Schema.XmlSchemaComplexType type = elt.SchemaType as System.Xml.Schema.XmlSchemaComplexType;

						// Add the elements
						if(type.Particle is System.Xml.Schema.XmlSchemaSequence)
						{
							System.Xml.Schema.XmlSchemaSequence seq = (elt.SchemaType as System.Xml.Schema.XmlSchemaComplexType).Particle as System.Xml.Schema.XmlSchemaSequence;
							System.Collections.ArrayList members = Utils.GetElements(seq.Items);
							for(int i=0; i<members.Count; i++)
							{
								System.Xml.Schema.XmlSchemaElement member = members[i] as System.Xml.Schema.XmlSchemaElement;
								if( member.RefName.Name == string.Empty )
									continue; // Ignore elements like <query> and <sql-query>
								string memberName = Utils.Capitalize(member.Name + member.RefName.Name); // One of them is empty

								System.Text.StringBuilder sb = new System.Text.StringBuilder();
								sb.Append(
			@"	if( typeOfElement2 == typeof(" + memberName + @"Attribute) )
				{
					if( ");
								for(int j=i+1; j<members.Count; j++)
								{
									System.Xml.Schema.XmlSchemaElement nextMember = members[j] as System.Xml.Schema.XmlSchemaElement;
									if( nextMember.RefName.Name == string.Empty )
										continue; // Ignore elements like <query> and <sql-query>
									string nextMemberName = Utils.Capitalize(nextMember.Name + nextMember.RefName.Name); // One of them is empty
									sb.Append("element1 is " + nextMemberName + "Attribute || ");
								}
								// Add "typeOfElement2 == null" at the end to handle "||" and empty "if()" without compilation warning
								sb.Append(@"typeOfElement2 == null )
						return true;
				}");
								method.Body.Add(Refly.CodeDom.Stm.Snippet( sb.ToString() ));
							}
						}
					}
					method.Body.Add(Refly.CodeDom.Stm.Snippet("}"));
				}
			}

			method.Body.Add(Refly.CodeDom.Stm.Snippet("return false;"));
			// -----------------------------------------------------------------
		}





		/// <summary> Add the content of the method GetXmlEnumValue(). </summary>
		public static void FillGetXmlEnumValue(Refly.CodeDom.MethodDeclaration method)
		{
			method.Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Overloaded;
			method.Doc.Summary.AddText(" Gets the xml enum value from the associated attributed enum. "); // Create the <summary />
			method.Signature.Parameters.Add(typeof(System.Type), "enumType");
			method.Signature.Parameters.Add(typeof(object), "enumValue");
			method.Signature.ReturnType = new Refly.CodeDom.TypeTypeDeclaration(typeof(string));

			method.Body.Add(Refly.CodeDom.Stm.Snippet(
			@"// Enumeration's members have a XmlEnumAttribute; its Name is the value to return
			System.Reflection.FieldInfo[] fields = enumType.GetFields();
			foreach( System.Reflection.FieldInfo field in fields )
			{
				if( field.Name == System.Enum.GetName(enumType, enumValue) )
				{
					System.Xml.Serialization.XmlEnumAttribute attribute =
						System.Attribute.GetCustomAttribute( field, typeof(System.Xml.Serialization.XmlEnumAttribute), false ) as System.Xml.Serialization.XmlEnumAttribute;
					if( attribute != null )
						return attribute.Name;
					else
						throw new System.ApplicationException( string.Format( ""{0} is missing XmlEnumAttribute on {1} value."", enumType, enumValue ) );
				}
			}
			throw new System.MissingFieldException(enumType.ToString(), enumValue.ToString());"));
			// -----------------------------------------------------------------
		}





		private static string AttributeToXmlValue(System.Xml.Schema.XmlSchemaElement schemaElt, System.Xml.Schema.XmlSchemaAttribute attrib, string attribName)
		{
			string fieldType = Utils.GetAttributeTypeName(schemaElt, attrib);
			string val = "attribute." + Utils.Capitalize(attribName);

			switch(fieldType)
			{
				case "System.Boolean" : return val + " ? \"true\" : \"false\"";
				case "System.Int32" : return val + ".ToString()";
				case "System.String" : return val;
				default: // => Enum
					return "GetXmlEnumValue(typeof(" + fieldType + "), " + val + ")";
			}
		}
	}
}
