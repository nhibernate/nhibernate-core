using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for CollectionIndexAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class CollectionIndexAttribute : Attribute
	{
		#region Member Variables
		
		private string m_Column;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public CollectionIndexAttribute( string column )
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
