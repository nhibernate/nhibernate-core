using System;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	public class HiLoTableGeneratorInt32Fixture : IdFixtureBase
	{
		protected override string TypeName
		{
			get { return "HiLoInt32"; }
		}

		[Test]
		public void ReadWrite()
		{
			Int32 id;
			ISession s = OpenSession();
			HiLoInt32Class b = new HiLoInt32Class();
			s.Save(b);
			s.Flush();
			id = b.Id;
			s.Close();

			s = OpenSession();
			b = (HiLoInt32Class) s.Load(typeof(HiLoInt32Class), b.Id);
			Assert.AreEqual(id, b.Id);
			s.Delete(b);
			s.Flush();
			s.Close();
		}
	}
}