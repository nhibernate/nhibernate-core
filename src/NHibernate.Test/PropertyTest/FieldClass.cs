using System;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldClass.
	/// </summary>
	public class FieldClass 
	{
		private int _id;
		private int m_id;
		private int id;

		public FieldClass( int underscoreId, int mUnderscoreId, int Id ) 
		{
			_id = underscoreId;
			m_id = mUnderscoreId;
			id = Id;
		}

		public void Increment() 
		{
			_id++;
			m_id++;
			id++;
		}
	}
}
