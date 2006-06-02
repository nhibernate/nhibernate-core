using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for SetAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SetAttribute : Attribute 
	{
		#region Member Variables

		private string m_Table;
		private string m_Schema;
		private bool m_IsLazy;
		private CascadeType m_Cascade = CascadeType.None;
		private string m_SqlWhere;
		private string m_Access = MapGenerator.DefaultAccessStrategy;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public SetAttribute( string table )
		{
			this.Table = table;
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
		/// Gets and sets the Schema.
		/// </summary>
		public string Schema
		{
			get { return m_Schema; }
			set { m_Schema = value; }
		}
		
		/// <summary>
		/// Gets and sets the IsLazy.
		/// </summary
		public bool IsLazy
		{
			get { return m_IsLazy; }
			set { m_IsLazy = value; }
		}
		
		/// <summary>
		/// Gets and sets the Cascade.
		/// </summary>
		public CascadeType Cascade
		{
			get { return m_Cascade; }
			set { m_Cascade = value; }
		}
		
		/// <summary>
		/// Gets and sets the SqlWhere.
		/// </summary>
		public string SqlWhere
		{
			get { return m_SqlWhere; }
			set { m_SqlWhere = value; }
		}
		
		/// <summary>
		/// Gets and sets the Access.
		/// </summary>
		public string Access
		{
			get { return m_Access; }
			set { m_Access = value; }
		}
		
		/// <summary>
		/// Get and sets the access type.
		/// </summary>
		public Type AccessType
		{
			get { return Type.GetType( m_Access ); }
			set { m_Access = String.Format( "{0}, {1}", value.FullName, value.Assembly.GetName().Name ); }
		}		
	}
}
