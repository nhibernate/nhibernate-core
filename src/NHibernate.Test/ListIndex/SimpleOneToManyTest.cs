using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.ListIndex
{
	[TestFixture]
	public class SimpleOneToManyTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] { "ListIndex.SimpleOneToMany.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void ShouldIncludeTheListIdxInserting()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var galery = new Galery();
				galery.Images.Add(new Image {Path = "image01.jpg"});
				s.Persist(galery);
				Assert.DoesNotThrow(tx.Commit);
			}
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Image").ExecuteUpdate();
				s.CreateQuery("delete from Galery").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}