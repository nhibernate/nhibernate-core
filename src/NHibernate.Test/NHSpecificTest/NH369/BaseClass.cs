using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH369
{
	public class BaseClass
	{
		private KeyManyToOneClass _id;

		public KeyManyToOneClass Id
		{
			get { return _id; }
			set { _id = value; }
		}

		// Just to make NH happy, these methods are not used
		public override bool Equals(object obj)
		{
			return base.Equals (obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

	}
}
