using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for CollectionOneToManyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]	
	public class CollectionOneToManyAttribute : Attribute
	{
		#region Member Variables

		private System.Type m_Type;
		
		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public CollectionOneToManyAttribute( System.Type type )
		{
			this.Type = type;
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
