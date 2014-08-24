using System;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	public class HiLoTableGeneratorInt64Fixture : IdFixtureBase
	{
		protected override string TypeName
		{
			get { return "HiLoInt64"; }
		}

		[Test]
		public void ReadWrite()
		{
			Int64 id;
			ISession s = OpenSession();
			HiLoInt64Class b = new HiLoInt64Class();
			s.Save(b);
			s.Flush();
			id = b.Id;
			s.Close();

			s = OpenSession();
			b = (HiLoInt64Class) s.Load(typeof(HiLoInt64Class), b.Id);
			Assert.AreEqual(id, b.Id);
			s.Delete(b);
			s.Flush();
			s.Close();
		}
	}
}