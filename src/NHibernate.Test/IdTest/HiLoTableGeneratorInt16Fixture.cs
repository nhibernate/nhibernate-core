using System;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	public class HiLoTableGeneratorInt16Fixture : IdFixtureBase
	{
		protected override string TypeName
		{
			get { return "HiLoInt16"; }
		}

		[Test]
		public void ReadWrite()
		{
			Int16 id;
			ISession s = OpenSession();
			HiLoInt16Class b = new HiLoInt16Class();
			s.Save(b);
			s.Flush();
			id = b.Id;
			s.Close();

			s = OpenSession();
			b = (HiLoInt16Class) s.Load(typeof(HiLoInt16Class), b.Id);
			Assert.AreEqual(id, b.Id);
			s.Delete(b);
			s.Flush();
			s.Close();
		}
	}
}