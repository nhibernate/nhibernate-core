//
// NHibernate.Mapping.Attributes
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Customized HbmWriterHelper.
	/// Support ComponentPropertyAttribute.
	/// </summary>
	public class HbmWriterHelperEx : HbmWriterHelper
	{
		private string _defaultValue;

		/// <summary> Value returned by overridden methods. </summary>
		public virtual string DefaultValue
		{
			get { return _defaultValue; }
			set { _defaultValue = value; }
		}


		public override string Get_Component_Name_DefaultValue(System.Type type)
		{
			if(DefaultValue != null)
				return DefaultValue;

			return base.Get_Component_Name_DefaultValue(type);
		}
	}
}
