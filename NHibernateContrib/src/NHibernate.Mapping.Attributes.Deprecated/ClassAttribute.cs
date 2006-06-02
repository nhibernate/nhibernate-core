using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for ClassAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ClassAttribute : Attribute 
	{
		#region Member Variables

		private string m_Table;
		private bool m_Mutable;
		private string m_Schema;
		private string m_DiscriminatorValue;
		//		private System.Type m_Proxy;
		private bool m_DynamicUpdate;
		private bool m_DynamicInsert;
		private ClassPolymorphismType m_Polymorphism = ClassPolymorphismType.Implicit;
		private string m_SqlWhere;
		private System.Type m_Persister;
				
		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public ClassAttribute( string table )
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
		/// Gets and sets the DiscriminatorValue.
		/// </summary>
		public string DiscriminatorValue
		{
			get { return m_DiscriminatorValue; }
			set { m_DiscriminatorValue = value; }
		}
		
		/// <summary>
		/// Gets and sets the Mutable.
		/// </summary>
		public bool Mutable
		{
			get { return m_Mutable; }
			set { m_Mutable = value; }
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
		/// Gets and sets the Polymorphism.
		/// </summary>
		public ClassPolymorphismType Polymorphism
		{
			get { return m_Polymorphism; }
			set { m_Polymorphism = value; }
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
		/// Gets and sets the Persister.
		/// </summary>
		public System.Type Persister
		{
			get { return m_Persister; }
			set { m_Persister = value; }
		}
		
		/// <summary>
		/// Gets and sets the Schema.
		/// </summary>
		public string Schema
		{
			get { return m_Schema; }
			set { m_Schema = value; }
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
