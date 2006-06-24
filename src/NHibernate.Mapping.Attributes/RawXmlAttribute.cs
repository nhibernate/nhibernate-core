//
// NHibernate.Mapping.Attributes
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes
{
	/// <summary> Use this attribute to add any XML at a specific place. </summary>
	[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple=false)]
	[System.Serializable]
	public class RawXmlAttribute : System.Attribute
	{
		private System.Type _after = null;
		private string _content;


		/// <summary> Default constructor </summary>
		public RawXmlAttribute()
		{
		}


		/// <summary> Gets or sets the type of XML elements after which this XML will be added (omit to put on top). </summary>
		public virtual System.Type After
		{
			get { return _after; }
			set { _after = value; }
		}

		/// <summary> Gets or sets the XML content. </summary>
		public virtual string Content
		{
			get { return _content; }
			set { _content = value; }
		}
	}
}
