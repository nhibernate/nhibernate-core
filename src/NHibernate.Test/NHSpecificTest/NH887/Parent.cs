using System;

namespace NHibernate.Test.NHSpecificTest.NH887
{
	public class Parent
	{
		private int primaryKey;
		private int uniqueKey;

		public virtual int PrimaryKey
		{
			get { return primaryKey; }
			set { primaryKey = value; }
		}

		public virtual int UniqueKey
		{
			get { return uniqueKey; }
			set { uniqueKey = value; }
		}
	}
}