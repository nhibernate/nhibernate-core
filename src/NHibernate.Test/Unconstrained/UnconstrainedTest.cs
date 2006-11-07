using System;
using NUnit.Framework;
using System.Collections;

namespace NHibernate.Test.Unconstrained
{
	[TestFixture]
	public class UnconstrainedTest : UnconstrainedNoLazyTest
	{
		protected override IList Mappings
		{
			get
			{	return new string[] { "Unconstrained.Person.hbm.xml" };	}
		}
	}
}
