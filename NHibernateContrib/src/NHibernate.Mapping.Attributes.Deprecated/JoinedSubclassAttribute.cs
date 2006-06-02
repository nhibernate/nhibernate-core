using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for JoinedSubclassAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class JoinedSubclassAttribute : Attribute 
	{
		#region Member Variables

		private System.Type m_Extends;
//		private System.Type m_Proxy;
		private bool m_DynamicUpdate;
		private bool m_DynamicInsert;
		private string m_Table;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public JoinedSubclassAttribute( string table )
		{
			this.Table = table;
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="extends"></param>
		public JoinedSubclassAttribute( string table, System.Type extends ) : this( table )
		{
			this.Extends = extends;
		}
		
		/// <summary>
		/// Gets and sets the Table.
		/// </summary>
		public string Table
		{
			get { return m_Table; }
			set { m_Table = value; }
		}
		
		/// <summary>
		/// Gets and sets the Extends.
		/// </summary>
		public System.Type Extends
		{
			get { return m_Extends; }
			set { m_Extends = value; }
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
