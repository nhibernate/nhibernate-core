using System;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldClass.
	/// </summary>
	public class FieldClass 
	{
		private int Id;
		private int _id;
		private int m_Id;
		private int id;

		public FieldClass(int Id, int underscoreId, int mUnderscoreId, int camelId ) 
		{
			this.Id = Id;
			_id = underscoreId;
			m_Id = mUnderscoreId;
			id = camelId;
		}

		public void Increment() 
		{
			Id++;
			_id++;
			m_Id++;
			id++;
		}
	}
}
