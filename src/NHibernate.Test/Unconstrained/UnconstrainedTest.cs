using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Unconstrained
{
	[TestFixture]
	public class UnconstrainedTest : UnconstrainedNoLazyTest
	{
		protected override IList Mappings
		{
			get { return new string[] {"Unconstrained.Person.hbm.xml"}; }
		}
	}
}