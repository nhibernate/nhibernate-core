using System.Collections.Generic;
using System;

namespace NHibernate.Test.NHSpecificTest.DtcFailures
{
	public class Person
	{
		private int id;

        public virtual DateTime CreatedAt { get; set; }

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}