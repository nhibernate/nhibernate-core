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
		public override void WriteDynamicComponent(System.Xml.XmlWriter writer, System.Reflection.MemberInfo member, DynamicComponentAttribute attribute, BaseAttribute parentAttribute)
		{
			if(attribute is ComponentPropertyAttribute)
			{
				ComponentPropertyAttribute attrib = attribute as ComponentPropertyAttribute;
				System.Type componentType = attrib.ComponentType;
				if(componentType == null)
					if(member is System.Reflection.FieldInfo)
						componentType = (member as System.Reflection.FieldInfo).FieldType;
					else // It MUST be a PropertyInfo
						componentType = (member as System.Reflection.PropertyInfo).PropertyType;

				object[] componentAttributes = componentType.GetCustomAttributes(typeof(ComponentAttribute), true);
				if(componentAttributes.Length == 0)
					throw new System.ApplicationException(componentType.FullName + " doesn't have the attribute [Component]!");
				ComponentAttribute componentAttribute = componentAttributes[0] as ComponentAttribute;

				string componentName = attrib.PropertyName;
				if(componentName == null)
					componentName = member.Name; // Default value

				if(componentAttribute.Name != null && componentAttribute.Name != componentName)
					// Because, it will be used by the default implementation of "WriteComponent()"
					throw new System.ApplicationException(componentType.FullName + " MUST have a [Component] with a 'null' Name (or the name '" + componentName + "')");

				// Get the helper to set the componentName
				HbmWriterHelperEx helper = this.DefaultHelper as HbmWriterHelperEx;
				if(helper == null)
					throw new System.ApplicationException("DefaultHelper must be a HbmWriterHelperEx (or a subType) to use [ComponentPropertyAttribute]");

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
			else
				base.WriteDynamicComponent(writer, member, attribute, parentAttribute);
		}
	}
}
