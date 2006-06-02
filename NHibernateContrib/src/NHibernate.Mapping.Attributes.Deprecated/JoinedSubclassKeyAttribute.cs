using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for JoinedSubclassKeyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class JoinedSubclassKeyAttribute : Attribute 
	{
		#region Member Variables
		
		private string m_Column;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public JoinedSubclassKeyAttribute( string column )
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
	}
}
