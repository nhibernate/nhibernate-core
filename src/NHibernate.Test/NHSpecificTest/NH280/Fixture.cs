using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH280
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "NHSpecificTest.NH280.Foo.hbm.xml" }; }
		}

		[Test]
		public void ConstInSelect()
		{
			using (ISession s= OpenSession())
			{
				Foo f = new Foo("Fiammy");
				s.Save(f);
				s.Flush();

				IList l = s.CreateQuery("select 'TextConst', 123, 123.5, .5 from Foo").List();
				IList result = l[0] as IList;
				Assert.AreEqual(typeof(string), result[0].GetType());
				Assert.AreEqual(typeof(Int32), result[1].GetType());
				Assert.AreEqual(typeof(Double), result[2].GetType());
				Assert.AreEqual(typeof(Double), result[3].GetType());
				Assert.AreEqual("TextConst", result[0]);
				Assert.AreEqual(123, result[1]);
				Assert.AreEqual(123.5D, result[2]);
				Assert.AreEqual(0.5D, result[3]);

				l = s.CreateQuery("select 123, f from Foo f").List();
				result = l[0] as IList;
				Assert.AreEqual(typeof(Int32), result[0].GetType());
				Assert.AreEqual(typeof(Foo), result[1].GetType());
				Assert.AreEqual(123, result[0]);
				Assert.AreEqual("Fiammy", (result[1] as Foo).Description);


				l = s.CreateQuery("select 123, f.Description from Foo f").List();
				result = l[0] as IList;
				Assert.AreEqual(123, result[0]);
				Assert.AreEqual("Fiammy", result[1]);

				s.Delete(f);
				s.Flush();
			}
		}

		[Test]
		public void TokenUnknow()
		{
			using (ISession s = OpenSession())
			{
				Assert.Throws<QueryException>(() =>s.CreateQuery("select 123, notRecognized, f.Description from Foo f").List());
			}
		}
	}
}
