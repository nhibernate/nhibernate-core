using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for VersionAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class VersionAttribute : Attribute
	{
		#region Member Variables

		private string m_Column;
		private string m_Access = MapGenerator.DefaultAccessStrategy;
		
		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public VersionAttribute( string column )
		{
			this.Column = column;
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
	}
}
