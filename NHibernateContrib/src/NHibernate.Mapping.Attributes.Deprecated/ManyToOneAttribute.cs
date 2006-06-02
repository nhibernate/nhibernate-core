using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for ManyToOneAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ManyToOneAttribute : Attribute
	{
		#region Member Variables
		
		private string m_Column;
		private System.Type m_Type;
		private bool m_Update = true;
		private bool m_Insert = true;
		private OuterJoinType m_OuterJoin;
		private CascadeType m_Cascade;
		private bool m_Inheritable = true;
		private string m_Access = MapGenerator.DefaultAccessStrategy;
		
		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="type"></param>
		public ManyToOneAttribute( System.Type type )
		{
			this.Type = type;
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		public ManyToOneAttribute( string column )
		{
			this.Column = column;
		}

		/// <summary>
		/// Class constructor.
		/// </summary>
		public ManyToOneAttribute( string column, System.Type type ) : this( column )
		{
			this.Type = type;
		}

		/// <summary>
		/// Gets and sets the Column.
		/// </summary>
		public string Column
		{
			get { return m_Column; }
			set { m_Column = value; }
		}
		
		/// <summary>
		/// Gets and sets the Update.
		/// </summary>
		public bool Update
		{
			get { return m_Update; }
			set { m_Update = value; }
		}
		
		/// <summary>
		/// Gets and sets the Insert.
		/// </summary>
		public bool Insert
		{
			get { return m_Insert; }
			set { m_Insert = value; }
		}
		
		/// <summary>
		/// Gets and sets the OuterJoin.
		/// </summary>
		public OuterJoinType OuterJoin
		{
			get { return m_OuterJoin; }
			set { m_OuterJoin = value; }
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
		/// Gets and sets the Inheritable.
		/// </summary>
		public bool Inheritable
		{
			get { return m_Inheritable; }
			set { m_Inheritable = value; }
		}		
		
		/// <summary>
		/// Gets and sets the Type.
		/// </summary>
		public System.Type Type
		{
			get { return m_Type; }
			set { m_Type = value; }
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
