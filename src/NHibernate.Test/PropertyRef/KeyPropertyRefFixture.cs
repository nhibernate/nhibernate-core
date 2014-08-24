using System;
using NHibernate;
using NUnit.Framework;

namespace NHibernate.Test.PropertyRef
{
	[TestFixture]
	public class KeyPropertyRefFixture : TestCase
	{
		protected override System.Collections.IList Mappings
		{
			get { return new string[] { "PropertyRef.KeyPropertyRef.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
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
		public void PropertyRefUsesOtherColumn()
		{
			const int ExtraId = 500;

			A a = new A();
			a.Name = "First";
			a.ExtraId = ExtraId;

			B b = new B();
			b.Id = ExtraId;
			b.Name = "Second";

			ISession s = OpenSession();
			s.Save(a);
			s.Save(b);
			s.Flush();
			s.Close();

			s = OpenSession();
			A newA = s.Get<A>(a.Id);

			Assert.AreEqual(1, newA.Items.Count);
			s.Close();

		}
	}
}