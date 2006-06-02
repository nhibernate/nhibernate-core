using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for DiscriminatorAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DiscriminatorAttribute : Attribute 
	{
		#region Member Variables

		private string m_Column;
		private System.Type m_Type;
		private bool m_Force;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public DiscriminatorAttribute( string column, System.Type type )
		{
			m_Column = column;
			m_Type = type;
		}
		
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="force"></param>
		public DiscriminatorAttribute( string column, System.Type type, bool force ) : this( column, type )
		{
			m_Force = force;
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
		/// Gets and sets the Force.
		/// </summary>
		public bool Force
		{
			get { return m_Force; }
			set { m_Force = value; }
		}
		
		/// <summary>
		/// Gets and sets the Type.
		/// </summary>
		public System.Type Type
		{
			get { return m_Type; }
			set { m_Type = value; }
		}
		
		
	}
}
