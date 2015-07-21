using System;

namespace NHibernate.Test.NHSpecificTest.NH2195
{
	public class DomainClass
	{
		private string stringData;
		private int intData;
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string StringData
		{
			get { return stringData; }
			set { stringData = value; }
		}

		public int IntData
		{
			get { return intData; }
			set { intData = value; }
		}		
	}
}