//
// NHibernate.Mapping.Attributes
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes
{
	/// <summary> Use this attribute to give a value to a field identified in a mapping attribute. </summary>
	[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple=true)]
	[System.Serializable]
	public class AttributeIdentifierAttribute : System.Attribute
	{
		private string _name = null;
		private string _value = null;
		private string _valueFormat = "g";


		/// <summary> Default constructor </summary>
		public AttributeIdentifierAttribute(string name)
		{
			this.Name = name;
		}


		/// <summary> Gets or sets the name used to identify the field (without the quotes). </summary>
		public virtual string Name
		{
			get { return _name; }
			set
			{
				if(value==null || value.Length==0)
					throw new System.ArgumentNullException("value");
				_name = value;
			}
		}


		/// <summary> Gets or sets the value of the identified field. </summary>
		public virtual string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		/// <summary> Gets or sets the value of the identified field. </summary>
		public virtual object ValueObject
		{
			get { return _value; }
			set
			{
				if(value is System.Enum)
					this.Value = System.Enum.Format(value.GetType(), value, this.ValueFormat);
				else
					this.Value = value==null ? "null" : value.ToString();
			}
		}

		/// <summary> Gets or sets the value of the identified field. </summary>
		public virtual System.Type ValueType
		{
			get { return System.Type.GetType( this.Value ); }
			set
			{
				if(value.Assembly == typeof(int).Assembly)
					this.Value = value.FullName.Substring(7);
				else
					this.Value = value.FullName + ", " + value.Assembly.GetName().Name;
			}
		}


		/// <summary> Gets or sets the 'format' used by System.Enum.Format() in ValueObject. </summary>
		public virtual string ValueFormat
		{
			get { return _valueFormat; }
			set { _valueFormat = value; }
		}
	}
}
