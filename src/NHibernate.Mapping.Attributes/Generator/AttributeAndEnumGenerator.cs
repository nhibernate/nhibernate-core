//
// NHibernate.Mapping.Attributes.Generator
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes.Generator
{
	/// <summary>
	/// Generate Attributes and Enumerations.
	/// </summary>
	public class AttributeAndEnumGenerator
	{
		/// <summary> Log (debug) infos, warnings and errors </summary>
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );




		/// <summary> Fill the ClassDeclaration with Attributes of the XmlSchemaElement. </summary>
		public static void GenerateAttribute(System.Xml.Schema.XmlSchemaElement attElt, Refly.CodeDom.ClassDeclaration cd, System.Xml.Schema.XmlSchemaComplexType refSchemaType)
		{
			cd.Doc.Summary.AddText(AnnotationToString(attElt.Annotation)); // Create the <summary />
			cd.Parent = new Refly.CodeDom.StringTypeDeclaration("BaseAttribute");
			// Add [AttributeUsage(AttributeTargets, AllowMultiple]  and  [Serializable]
			// Note: There is a problem with the order of the arguments, that's why they are together
			Refly.CodeDom.AttributeDeclaration attribUsage = cd.CustomAttributes.Add("System.AttributeUsage");
			attribUsage.Arguments.Add( "", new Refly.CodeDom.Expressions.SnippetExpression( "System.AttributeTargets."
				+ (Utils.IsRoot(Utils.Capitalize(attElt.Name)) ? "Class | System.AttributeTargets.Struct" : "Property | System.AttributeTargets.Field")
				+ ", AllowMultiple=" + Utils.AllowMultipleValue(attElt.Name) ) );
			cd.CustomAttributes.Add("System.Serializable");

			// Add the constructors
			Refly.CodeDom.ConstructorDeclaration defCtor = cd.AddConstructor();
			defCtor.Doc.Summary.AddText(" Default constructor (position=0) "); // Create the <summary />
			defCtor.BaseContructorArgs.Add( new Refly.CodeDom.Expressions.SnippetExpression("0") );
			Refly.CodeDom.ConstructorDeclaration posCtor = cd.AddConstructor();
			posCtor.Doc.Summary.AddText(" Constructor taking the position of the attribute. "); // Create the <summary />
			posCtor.Signature.Parameters.Add(typeof(int), "position");
			posCtor.BaseContructorArgs.Add( new Refly.CodeDom.Expressions.SnippetExpression("position") );

			System.Xml.Schema.XmlSchemaComplexType type = attElt.SchemaType as System.Xml.Schema.XmlSchemaComplexType;
			if(type == null && !attElt.SchemaTypeName.IsEmpty) // eg:  <xs:element name="cache" type="cacheType" />
				type = refSchemaType;

			if(type != null)
			{
				// Add the attribute members
				System.Collections.ArrayList attribMembers = Utils.GetAttributes(type.Attributes);
				log.Debug("Add Attribute members: Count=" + attribMembers.Count);
				foreach(System.Xml.Schema.XmlSchemaAttribute attribMember in attribMembers)
				{
					string memberType = Utils.GetAttributeTypeName(attElt, attribMember);
					if(attribMember.SchemaTypeName.Name==string.Empty) // Create the dynamically generated enumeration
					{
						log.Debug("Generate Enumeration for SimpleType: " + memberType);
						GenerateEnumeration(attribMember.SchemaType, cd.Namespace.AddEnum(memberType, false));
					}

					// Add the field with its default value
					Refly.CodeDom.FieldDeclaration fd = cd.AddField(
						memberType, attribMember.Name.Replace("-", "").ToLower() );
					// Set the unspecified value (to know if specified by the user)
					fd.InitExpression = new Refly.CodeDom.Expressions.SnippetExpression( Utils.GetUnspecifiedValue(attElt, attribMember) );
					// Add its public property with a comment
					Refly.CodeDom.PropertyDeclaration pd = cd.AddProperty(fd, true, true, false);
					pd.Doc.Summary.AddText(AnnotationToString(attribMember.Annotation)); // Create the <summary />
					if(memberType == "System.Boolean")
					{
						// Add the field+property to know if this field has been specified
						Refly.CodeDom.FieldDeclaration boolIsSpec = cd.AddField(
							"System.Boolean", fd.Name + "specified" );
						cd.AddProperty(boolIsSpec, true, false, false).Doc.Summary.AddText(" Tells if " + pd.Name + " has been specified. "); // Create the <summary />
						pd.Set.Add( new Refly.CodeDom.Expressions.SnippetExpression(
							boolIsSpec.Name + " = true" ) );
					}

					// Add the System.Type property (to allow setting this attribute with a type)
					if(Utils.IsSystemType(attElt.Name, attribMember, memberType))
					{
						log.Debug("  Create System.Type for <" + attElt.Name + "> <" + attribMember.Name + ">");
						Refly.CodeDom.PropertyDeclaration pdType = cd.AddProperty(typeof(System.Type), pd.Name + "Type");
						pdType.Doc.Summary.AddText(AnnotationToString(attribMember.Annotation)); // Create the <summary />
						pdType.Get.Add( new Refly.CodeDom.Expressions.SnippetExpression(
							"return System.Type.GetType( this." + pd.Name + " )" ) );
						pdType.Set.Add( new Refly.CodeDom.Expressions.SnippetExpression( string.Format(
				@"if(value.Assembly == typeof(int).Assembly)
					this.{0} = value.FullName.Substring(7);
				else
					this.{0} = value.FullName + "", "" + value.Assembly.GetName().Name", pd.Name) ) );
					}

					// Add the object property (to allow setting this attribute with any object)
					if(Utils.IsSystemObject(attElt.Name, attribMember, memberType))
					{
						bool IsSystemEnum = Utils.IsSystemEnum(attElt.Name, attribMember, memberType);
						log.Debug("  Create object version " + (IsSystemEnum?"+ EnumFormat ":"") + "for <" + attElt.Name + "> <" + attribMember.Name + ">");
						Refly.CodeDom.PropertyDeclaration pdType = cd.AddProperty(typeof(object), pd.Name + "Object");
						pdType.Doc.Summary.AddText(AnnotationToString(attribMember.Annotation)); // Create the <summary />
						pdType.Get.Add( new Refly.CodeDom.Expressions.SnippetExpression(
							"return this." +  pd.Name) );
						// handle conversion of enum values
						if(IsSystemEnum)
							pdType.Set.Add( new Refly.CodeDom.Expressions.SnippetExpression( string.Format(
				@"if(value is System.Enum)
					this.{0} = System.Enum.Format(value.GetType(), value, this.{0}EnumFormat);
				else
					this.{0} = value==null ? ""null"" : value.ToString()", pd.Name) ) );
						else
							pdType.Set.Add( new Refly.CodeDom.Expressions.SnippetExpression( string.Format(
								@"this.{0} = value==null ? ""null"" : value.ToString()", pd.Name) ) );

						// Add the field xxxEnumFormat to set the string to use when formatting an Enum
						if(IsSystemEnum)
						{
							Refly.CodeDom.FieldDeclaration fdEnumFormat = cd.AddField(
								typeof(string), (fd.Name + "EnumFormat").ToLower() );
							// Set the default value
							fdEnumFormat.InitExpression = new Refly.CodeDom.Expressions.SnippetExpression("\"g\"");
							// Add its public property with a comment
							Refly.CodeDom.PropertyDeclaration pdEnumFormat = cd.AddProperty(fdEnumFormat, true, true, false);
							pdEnumFormat.Doc.Summary.AddText("'format' used by System.Enum.Format() in " + pdType.Name); // Create the <summary />
						}
					}
				}

				if(type.IsMixed) // used by elements like <param>
				{
					// Add the field with its default value
					Refly.CodeDom.FieldDeclaration fd = cd.AddField( "System.String", "Content" );
					// Add the unspecified value (to know if specified by the user)
					fd.InitExpression = new Refly.CodeDom.Expressions.SnippetExpression( "null" );
					// Add its public property with a comment
					Refly.CodeDom.PropertyDeclaration pd = cd.AddProperty(fd, true, true, false);
					pd.Doc.Summary.AddText(" Gets or sets the content of this element "); // Create the <summary />
				}
			}
			else
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("Unknow Element: ").Append(attElt.Name);
				if(attElt.SchemaType != null)
					sb.Append(", SchemaType = ").Append(attElt.SchemaType).Append(" - ").Append(attElt.SchemaType.Name);
				if(!attElt.SchemaTypeName.IsEmpty)
					sb.Append(", SchemaTypeName = ").Append(attElt.SchemaTypeName.Name);
				if(attElt.ElementType != null)
					sb.Append(", ElementType = ").Append(attElt.ElementType);
				log.Warn(sb.ToString());
			}
		}









		/// <summary> Fill the EnumDeclaration with Attributes of the XmlSchemaSimpleType. </summary>
		public static void GenerateEnumeration(System.Xml.Schema.XmlSchemaSimpleType enumElt, Refly.CodeDom.EnumDeclaration ed)
		{
			ed.Doc.Summary.AddText(AnnotationToString(enumElt.Annotation)); // Create the <summary />
			ed.AddField("unspecified").Doc.Summary.AddText( "Default value (don't use it)" ); // The default value

			if(enumElt.Content is System.Xml.Schema.XmlSchemaSimpleTypeRestriction)
			{
				System.Xml.Schema.XmlSchemaSimpleTypeRestriction restric = enumElt.Content as System.Xml.Schema.XmlSchemaSimpleTypeRestriction;
				foreach(System.Xml.Schema.XmlSchemaObject obj in restric.Facets)
				{
					if(obj is System.Xml.Schema.XmlSchemaEnumerationFacet)
					{
						System.Xml.Schema.XmlSchemaEnumerationFacet elt = obj as System.Xml.Schema.XmlSchemaEnumerationFacet;
						Refly.CodeDom.FieldDeclaration field = ed.AddField(elt.Value.Replace("-","").ToLower());
						field.Doc.Summary.AddText( elt.Value ); // Create the <summary />
						// Add the XmlEnumAttribute used in GetXmlEnumValue()
						Refly.CodeDom.AttributeDeclaration enumAttrib = field.CustomAttributes.Add("System.Xml.Serialization.XmlEnumAttribute");
						enumAttrib.Arguments.Add("", new Refly.CodeDom.Expressions.SnippetExpression("\""+elt.Value+"\""));
					}
					else
					{
						log.Warn("Unknow Enum: " + obj.ToString());
					}
				}
			}
			else
			{
				log.Warn("Unknow Enum Content: " + enumElt.Content.ToString());
			}
		}










		private static string AnnotationToString(System.Xml.Schema.XmlSchemaAnnotation annotation)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if(annotation == null)
				return " "; // Empty

			// Append all InnerTexts
			foreach(System.Xml.Schema.XmlSchemaObject obj in annotation.Items)
			{
				if(obj is System.Xml.Schema.XmlSchemaDocumentation)
					foreach(System.Xml.XmlNode node in ((System.Xml.Schema.XmlSchemaDocumentation)obj).Markup)
						sb.Append(node.InnerText);
			}
			return sb.ToString();
		}
	}
}
