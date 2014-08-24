using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.NativeGuid
{
	[TestFixture]
	public class NativeGuidFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"IdGen.NativeGuid.NativeGuidPoid.hbm.xml"}; }
		}

		[Test]
		public void Crd()
		{
			object savedId;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var nativeGuidPoid = new NativeGuidPoid();
				savedId = s.Save(nativeGuidPoid);
				tx.Commit();
				Assert.That(savedId, Is.Not.Null);
				Assert.That(savedId, Is.EqualTo(nativeGuidPoid.Id));
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var nativeGuidPoid = s.Get<NativeGuidPoid>(savedId);
				Assert.That(nativeGuidPoid, Is.Not.Null);
				s.Delete(nativeGuidPoid);
				tx.Commit();
			}
		}
	}
}