//
// NHibernate.Mapping.Attributes.Generator
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes.Generator
{
	/// <summary>
	/// Useful static methods.
	/// </summary>
	public class Utils
	{
		/// <summary> Log (debug) infos, warnings and errors </summary>
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );


		private static System.Collections.Specialized.StringCollection _knowEnums = null;

		/// <summary> Gets the enumerations used in NHibernate mapping schema. </summary>
		public static System.Collections.Specialized.StringCollection KnowEnums
		{
			get
			{
				if(_knowEnums == null)
				{
					_knowEnums = new System.Collections.Specialized.StringCollection();
					_knowEnums.Add("cascadeStyle");
					_knowEnums.Add("collectionFetchMode");
					_knowEnums.Add("customSQLCheck");
					_knowEnums.Add("fetchMode");
					_knowEnums.Add("flushMode");
					_knowEnums.Add("laziness");
					_knowEnums.Add("lockMode");
					_knowEnums.Add("notFoundMode");
					_knowEnums.Add("optimisticLockMode");
					_knowEnums.Add("outerJoinStrategy");
					_knowEnums.Add("polymorphismType");
					_knowEnums.Add("propertyGeneration");
					_knowEnums.Add("restrictedLaziness");
					_knowEnums.Add("unsavedValueType");
					_knowEnums.Add("versionGeneration");
				}
				return _knowEnums;
			}
		}


		/// <summary> Returns "true" or "false" for the AttributeUsage of this element. </summary>
		public static string AllowMultipleValue(string elt)
		{
			if( elt=="hibernate-mapping" || elt=="class"
				|| elt=="subclass" || elt=="joined-subclass" )
				return "false";

			return "true"; // TODO: Put back the following code? (harder to maintain)

			/*// Note : <property> and <many-to-one> are here because of <nested-composite-element>
			if( elt=="meta" || elt=="import" || elt=="param"
				|| elt=="column" || elt=="key-property" || elt=="key-many-to-one"
				|| elt=="property" || elt=="many-to-one" || elt=="nested-composite-element" )
				return "true";

			return "false";*/
		}


		/// <summary> Tells if it is the name of an attribute that can be applied on classes and properties. </summary>
		public static bool TargetsAll(string name)
		{
			return name=="Cache" || name=="JcsCache"
				|| name=="Discriminator" || name=="Key";
		}


		/// <summary> Tells if it is the name of a class-level attribute. </summary>
		public static bool IsRoot(string name)
		{
			return name=="HibernateMapping" || name=="Class"
				|| name=="Subclass" || name=="JoinedSubclass"
				|| name=="Component" || name=="Import";
		}


		/// <summary> Tells if the attribute's type must be convert to System.Type (easier to write/maintain and compile-time checking). </summary>
		public static bool IsSystemType(string parentEltName, System.Xml.Schema.XmlSchemaAttribute attribMember, string attribType)
		{
			if(attribType != "System.String")
				return false;

			if(attribMember.Name == "name" || attribMember.Name == "proxy")
				return parentEltName=="class" || parentEltName=="subclass"
					|| parentEltName=="joined-subclass" || parentEltName == "type";

			if(attribMember.Name == "extends")
				return parentEltName=="subclass" || parentEltName == "joined-subclass";

			// Note : Easier to write and it can't hurt :D (and ATM they are all System.Type)
			if( attribMember.Name == "access" || attribMember.Name == "default-access"
				|| attribMember.Name == "extends" || attribMember.Name == "id-type"
				|| attribMember.Name == "meta-type" || attribMember.Name == "persister"
				|| attribMember.Name == "collection-type" )
				return true;

			if(attribMember.Name == "class")
				return parentEltName == "component" || parentEltName == "composite-element" || parentEltName == "composite-id"
					|| parentEltName == "composite-index" || parentEltName == "index-many-to-many" || parentEltName == "key-many-to-one"
					|| parentEltName == "many-to-many" || parentEltName == "many-to-one" || parentEltName == "meta-value"
					|| parentEltName == "nested-composite-element" || parentEltName == "one-to-many" || parentEltName == "one-to-one"
					|| parentEltName == "import"  || parentEltName == "definition";

			if(attribMember.Name == "type")
				return parentEltName == "collection-id" || parentEltName == "discriminator" || parentEltName == "element"
					|| parentEltName == "id" || parentEltName == "index" || parentEltName == "key-property"
					|| parentEltName == "property" || parentEltName == "version";

			if(attribMember.Name == "sort")
				return parentEltName == "map" || parentEltName == "set";

			return false; // Default
		}


		/// <summary> Tells if the attribute can take any object (easier to write/maintain and compile-time checking). </summary>
		public static bool IsSystemObject(string parentEltName, System.Xml.Schema.XmlSchemaAttribute attribMember, string attribType)
		{
			if(attribType != "System.String")
				return false;

			return attribMember.Name == "unsaved-value"
				|| IsSystemEnum(parentEltName, attribMember, attribType);
		}


		/// <summary> Tells if the attribute's type must be convert from a System.Enum value (easier to write/maintain and compile-time checking). </summary>
		public static bool IsSystemEnum(string parentEltName, System.Xml.Schema.XmlSchemaAttribute attribMember, string attribType)
		{
			return attribMember.Name == "discriminator-value";
		}


		/// <summary> Tells if this (non-root) element has itself as a sub-element. </summary>
		public static bool CanContainItself(string name)
		{
			return name == "DynamicComponent" || name == "NestedCompositeElement";
		}







		/// <summary> Convert the attributes in the Collection to an ArrayList </summary>
		public static System.Collections.ArrayList GetAttributes(System.Xml.Schema.XmlSchemaObjectCollection typeAttributes)
		{
			System.Collections.ArrayList attributes = new System.Collections.ArrayList();
			foreach(System.Xml.Schema.XmlSchemaObject obj in typeAttributes)
			{
				if(obj is System.Xml.Schema.XmlSchemaAttribute)
					attributes.Add(obj);
				else if(obj is System.Xml.Schema.XmlSchemaAttributeGroupRef)
				{
					System.Xml.Schema.XmlSchemaAttributeGroupRef elt = obj as System.Xml.Schema.XmlSchemaAttributeGroupRef;
					System.Xml.Schema.XmlSchemaAttributeGroup group = null;
					// Find the referenced group
					foreach(System.Xml.Schema.XmlSchemaObject item in Program.Schema.Items)
						if(item is System.Xml.Schema.XmlSchemaAttributeGroup)
						{
							group = item as System.Xml.Schema.XmlSchemaAttributeGroup;
							if(group.Name == elt.RefName.Name) break;
							else group = null; // not the good one
						}
					if(group != null) // => Found; add its attributes
						foreach(System.Xml.Schema.XmlSchemaObject att in group.Attributes)
							attributes.Add(att);
					else
						log.Warn("Unknow AttributeGroup: " + elt.RefName.Name);
				}
				else
				{
					log.Warn("Unknow Attribute: " + obj.ToString());
				}
			}
			return attributes;
		}


		/// <summary> Convert the elements in the Collection to an ArrayList </summary>
		public static System.Collections.ArrayList GetElements(System.Xml.Schema.XmlSchemaObjectCollection seqItems, System.Xml.Schema.XmlSchemaObjectCollection schemaItems)
		{
			System.Collections.ArrayList elements = new System.Collections.ArrayList();
			foreach(System.Xml.Schema.XmlSchemaObject obj in seqItems)
			{
				if(obj is System.Xml.Schema.XmlSchemaElement)
						   elements.Add(obj);
				else if(obj is System.Xml.Schema.XmlSchemaChoice)
				{
					System.Xml.Schema.XmlSchemaChoice choice = obj as System.Xml.Schema.XmlSchemaChoice;
					// Add each element of the choice
					foreach(System.Xml.Schema.XmlSchemaObject item in choice.Items)
						if(item is System.Xml.Schema.XmlSchemaElement)
							elements.Add(item);
						else if(item is System.Xml.Schema.XmlSchemaGroupRef)
						{
							// Find the Group
							string groupName = (obj as System.Xml.Schema.XmlSchemaGroupRef).RefName.Name;
							System.Xml.Schema.XmlSchemaGroup group = null;
							foreach(System.Xml.Schema.XmlSchemaObject schemaItem in schemaItems)
								if(schemaItem is System.Xml.Schema.XmlSchemaGroup && groupName==(schemaItem as System.Xml.Schema.XmlSchemaGroup).Name)
								{
									group = schemaItem as System.Xml.Schema.XmlSchemaGroup; // Found
									break;
								}
							if(group==null) // Not found
								throw new System.Exception("Unknown xs:group " + groupName + string.Format(", nh-mapping.xsd({0})", obj.LineNumber));
							elements.AddRange( GetElements(group.Particle.Items, schemaItems) );
						}
						else if(item is System.Xml.Schema.XmlSchemaSequence)
						{
							System.Xml.Schema.XmlSchemaSequence subSeq = item as System.Xml.Schema.XmlSchemaSequence;
							elements.AddRange( GetElements(subSeq.Items, schemaItems) );
						}
						else
							log.Warn("Unknown Object: " + item.ToString() + string.Format(", nh-mapping.xsd({0})", item.LineNumber));
				}
				else if(obj is System.Xml.Schema.XmlSchemaGroupRef)
				{
					// Find the Group
					string groupName = (obj as System.Xml.Schema.XmlSchemaGroupRef).RefName.Name;
					System.Xml.Schema.XmlSchemaGroup group = null;
					foreach(System.Xml.Schema.XmlSchemaObject schemaItem in schemaItems)
						if(schemaItem is System.Xml.Schema.XmlSchemaGroup && groupName==(schemaItem as System.Xml.Schema.XmlSchemaGroup).Name)
						{
							group = schemaItem as System.Xml.Schema.XmlSchemaGroup; // Found
							break;
						}
					if(group==null) // Not found
						throw new System.Exception("Unknown xs:group " + groupName + string.Format(", nh-mapping.xsd({0})", obj.LineNumber));
					elements.AddRange( GetElements(group.Particle.Items, schemaItems) );
				}
				else if(obj is System.Xml.Schema.XmlSchemaSequence)
				{
					System.Xml.Schema.XmlSchemaSequence subSeq = obj as System.Xml.Schema.XmlSchemaSequence;
					elements.AddRange( GetElements(subSeq.Items, schemaItems) );
				}
				else
					log.Warn("Unknown Object: " + obj.ToString() + string.Format(", nh-mapping.xsd({0})", obj.LineNumber));
			}
			return elements;
		}







		/// <summary> Returns the unspecified value of the type of this attribute. </summary>
		public static string GetUnspecifiedValue(System.Xml.Schema.XmlSchemaElement parentElt, System.Xml.Schema.XmlSchemaAttribute attrib)
		{
			// The unspecified value is used to know if the user set a value
			// (in this case, we should write it; even if it is the default value)
			switch(attrib.SchemaTypeName.Name)
			{
				case "boolean" : return attrib.DefaultValue==null ? "false" : attrib.DefaultValue; // This value is meaningless because of its "bool XXXIsSpecified" property
				case "positiveInteger" : return "-1"; // value should be positive (so we can use -1 as unspecified value)
				case "string" : return "null";
				default: // => Treat it as Enumeration
					return GetAttributeTypeName(parentElt, attrib) + ".Unspecified";
			}
		}





		/// <summary> Return the C# type of this XmlSchemaAttribute. </summary>
		public static string GetAttributeTypeName(System.Xml.Schema.XmlSchemaElement parentElt, System.Xml.Schema.XmlSchemaAttribute attrib)
		{
			string attribTypeName = attrib.SchemaTypeName.Name;
			switch(attribTypeName)
			{
				case "" : // => Dynamically generated enumeration
					return Capitalize(parentElt.Name)
						+ Capitalize(attrib.Name);
				case "boolean" : return "System.Boolean";
				case "positiveInteger" : return "System.Int32";
				case "string" : return "System.String";
				default:
					if( ! KnowEnums.Contains(attribTypeName) && attribTypeName.Length>0 )
						log.Warn("\n\nUnknow TYPE (can be an enum): " + attribTypeName + "\n\n");
					return Program.Conformer.ToCapitalized(attribTypeName);
			}
		}


		public static string Capitalize(string s)
		{
			return Program.Conformer.ToCapitalized(s.Replace("-","").ToLower());
		}
	}
}
