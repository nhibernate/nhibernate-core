using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for SubclassAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class SubclassAttribute : Attribute 
	{
		#region Member Variables

		private System.Type m_Extends;
		private string m_DiscriminatorValue;
		//		private System.Type m_Proxy;
		private bool m_DynamicUpdate;
		private bool m_DynamicInsert;		

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public SubclassAttribute()
		{
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="extends"></param>
		public SubclassAttribute( System.Type extends )
		{
			m_Extends = extends;
		}
		
		/// <summary>
		/// Gets and sets the DiscriminatorValue.
		/// </summary>
		public string DiscriminatorValue
		{
			get { return m_DiscriminatorValue; }
			set { m_DiscriminatorValue = value; }
		}
		
		/// <summary>
		/// Gets and sets the DynamicUpdate.
		/// </summary>
		public bool DynamicUpdate
		{
			get { return m_DynamicUpdate; }
			set { m_DynamicUpdate = value; }
		}
		
		/// <summary>
		/// Gets and sets the DynamicInsert.
		/// </summary>
		public bool DynamicInsert
		{
			get { return m_DynamicInsert; }
			set { m_DynamicInsert = value; }
		}		
		
		/// <summary>
		/// Gets and sets the Extends.
		/// </summary>
		public System.Type Extends
		{
			get { return m_Extends; }
			set { m_Extends = value; }
		}
		
//		/// <summary>
//		/// Gets and sets the Proxy.
//		/// </summary>
//		public System.Type Proxy
//		{
//			get { return m_Proxy; }
//			set { m_Proxy = value; }
//		}		
	}
}
