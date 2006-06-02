using System;

namespace NHibernate.Mapping.Attributes
{	
	/// <summary>
	/// Summary description for IdAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IdAttribute : Attribute
	{
		#region Member Variables
		
		private string m_Column;
		private string m_Access = MapGenerator.DefaultAccessStrategy;
		private UnsavedValueType m_UnsavedValueType = UnsavedValueType.Null;
		private string m_UnsavedValue;
		private System.Type m_Generator;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public IdAttribute( string column, System.Type generator )
		{
			this.Column = column;
			this.Generator = generator;
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
		/// Gets and sets the UnsavedValueType.
		/// </summary>
		public UnsavedValueType UnsavedValueType
		{
			get { return m_UnsavedValueType; }
			set { m_UnsavedValueType = value; }
		}
		
		/// <summary>
		/// Gets and sets the UnsavedValue.
		/// </summary>
		public string UnsavedValue
		{
			get { return m_UnsavedValue; }
			set { m_UnsavedValue = value; }
		}
		
		/// <summary>
		/// Gets and sets the Generator.
		/// </summary>
		public System.Type Generator
		{
			get { return m_Generator; }
			set { m_Generator = value; }
		}
	}
}
