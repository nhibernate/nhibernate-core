//
// NHibernate.Mapping.Attributes
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes
{
	/// <summary> Use this attribute to define a "component" on a property/field. If you set properties of [DynamicComponent], they will be ignored. </summary>
	[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple=true)]
	[System.Serializable]
	public class ComponentPropertyAttribute : DynamicComponentAttribute
	{
		private System.Type _componentType;
		private string _propertyName;


		/// <summary> Default constructor (position=0) </summary>
		public ComponentPropertyAttribute() : 
				base(0)
		{
		}

		/// <summary> Constructor taking the position of the attribute. </summary>
		public ComponentPropertyAttribute(int position) : 
				base(position)
		{
		}


		/// <summary> Gets or sets the type of the component. </summary>
		public virtual System.Type ComponentType
		{
			get { return _componentType; }
			set { _componentType = value; }
		}

		/// <summary> Gets or sets the name of the property which has the type of the component. </summary>
		public virtual string PropertyName
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		}
	}
}
