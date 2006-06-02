using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for PropertyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyAttribute : Attribute
	{
		#region Member Variables

		private string m_Column;
		private System.Type m_Type;
		private string m_Access = MapGenerator.DefaultAccessStrategy;
		private bool m_Update = true;
		private bool m_Insert = true;
		private string m_Formula;
		private int m_Size;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public PropertyAttribute()
		{
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="type"></param>
		public PropertyAttribute( System.Type type )
		{
			this.Type = type;
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="type"></param>
		public PropertyAttribute( string column, System.Type type ) : this( column )
		{
			this.Type = type;
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="type"></param>
		/// <param name="size"></param>
		public PropertyAttribute( string column, System.Type type, int size ) : this( column, size )
		{
			this.Type = type;
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="size"></param>
		public PropertyAttribute( int size )
		{
			this.Size = size;
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		public PropertyAttribute( string column )
		{
			this.Column = column;
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="size"></param>
		public PropertyAttribute( string column, int size ) : this( column )
		{
			this.Size = size;
		}
		
		/// <summary>
		/// Gets and sets the Size.
		/// </summary>
		public int Size
		{
			get { return m_Size; }
			set { m_Size = value; }
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
		
		/// <summary>
		/// Gets and sets the Column.
		/// </summary>
		public string Column
		{
			get { return m_Column; }
			set { m_Column = value; }
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
		/// Gets and sets the Formula.
		/// </summary>
		public string Formula
		{
			get { return m_Formula; }
			set { m_Formula = value; }
		}
		
	}
}
