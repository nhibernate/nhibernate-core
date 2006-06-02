using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for CollectionManyToManyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class CollectionManyToManyAttribute : Attribute 
	{
		#region Member Variables

		private string m_Column;
		private OuterJoinType m_OuterJoin;
		private System.Type m_Type;
		
		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public CollectionManyToManyAttribute( string column, System.Type type )
		{
			this.Column = column;
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
		/// Gets and sets the OuterJoin.
		/// </summary>
		public OuterJoinType OuterJoin
		{
			get { return m_OuterJoin; }
			set { m_OuterJoin = value; }
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
