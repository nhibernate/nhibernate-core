using System;
using NHibernate.DomainModel.NHSpecific;

namespace NHibernate.Test.NHSpecificTest.NH864
{
	public class ObjectWithNullableInt32
	{
		private int id;
		private NullableInt32 nullInt32;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual NullableInt32 NullInt32
		{
			get { return nullInt32; }
			set { nullInt32 = value; }
		}
	}
}