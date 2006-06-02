using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for CollectionKeyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class CollectionKeyAttribute : Attribute 
	{
		#region Member Variables
		
		private string m_Column;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public CollectionKeyAttribute( string column )
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
