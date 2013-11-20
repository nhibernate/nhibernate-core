using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3555
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CanModifyKeyEntity()
		{
			var keyEntity = new KeyEntity { Number = 10 };
			var valueEntity = new ValueEntity {Number = 20};
			var entity = new MapEntity();
			entity.Dic[keyEntity] = valueEntity;
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(keyEntity);
					s.Save(valueEntity);
					s.Save(entity);
					tx.Commit();
				}
				using (var tx = s.BeginTransaction())
				{
					keyEntity.Number = 20;
					tx.Commit();
				}
			}

			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Get<MapEntity>(entity.Id)
						.Dic.Count.Should().Be.EqualTo(1);					
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = sessions.OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Delete("from MapEntity");
					s.Delete("from KeyEntity");
					s.Delete("from ValueEntity");
					tx.Commit();	
				}
			}
		}
	}
}