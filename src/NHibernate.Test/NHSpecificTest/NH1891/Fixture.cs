﻿using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1891
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] { "NHSpecificTest.NH1891.FormulaEscaping.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Mapping uses a scalar sub-select formula.
			return dialect.SupportsScalarSubSelects;
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from B");
				s.Flush();
				s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
		public void FormulaEscaping()
		{
			string name = "Test";

			B b = new B();
			b.Name = name;

			A a = new A();
			a.FormulaConstraint = name;

			ISession s = OpenSession();

			s.Save(b);
			s.Save(a);
			s.Flush();
			s.Close();

			s = OpenSession();

			a = s.Get<A>(a.Id);

			Assert.AreEqual(1, a.FormulaCount);

			s.Close();
		}
	}
}
