using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2819
{
	public class Address
	{
		public virtual Guid Id { get; protected set; }

		public virtual T GenericMethod<T>(T arg)
		{
			return arg;
		}
	}
}