using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.PropertyRef
{
	[TestFixture]
	public class ManyToManyPropertyRefFixture : TestCase
	{
		protected override string[] Mappings => new[] { "PropertyRef.ManyToManyWithPropertyRef.hbm.xml" };

		protected override string MappingsAssembly => "NHibernate.Test";

		private object _manyAId;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var manyA = new ManyA { Number = 6, Value = "a value" };

				var manyB1 = new ManyB { Number = 4, Value = "a value of b1" };
				var manyB2 = new ManyB { Number = 8, Value = "a value of b2" };
				var manyB3 = new ManyB { Number = 12, Value = "a value of b3" };

				_manyAId = session.Save(manyA);
				session.Save(manyB1);
				session.Save(manyB2);
				session.Save(manyB3);

				manyA.ManyBs.Add(manyB1);
				manyA.ManyBs.Add(manyB2);
				manyA.ManyBs.Add(manyB3);
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				s.Flush();
				t.Commit();
			}
		}

		[Test]
		public void Getting_a_ManyA_object_with_fetchmode_select_will_work()
		{
			ManyA loadedManyA;
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				loadedManyA =
					session
						.CreateCriteria<ManyA>()
						.Add(Restrictions.IdEq(_manyAId))
						// Below Fetch is a no-op indeed, provided the mapping does not ask for eager fetching.
						// It is the equivalent of obsoleted SetFetchMode Select, which was also no-op, contrary
						// to what the SelectMode xml comment could let think. (This comment was valid only when
						// the enum was used for mapping, but not when used ins queries.)
						.Fetch(SelectMode.Skip, "ManyBs")
						.UniqueResult<ManyA>();
				NHibernateUtil.Initialize(loadedManyA.ManyBs);
				transaction.Commit();
			}

			/******************the select statements *************************************************************
			SELECT	this_.Id as Id0_0_,
					this_.Number as Number0_0_,
					this_.Value as Value0_0_
			FROM	ManyA this_
			WHERE	this_.Id = 1 /# ?p0 /#

			SELECT	manybs0_.ManyBNumber as ManyBNum1_1_,
					manybs0_.ManyANumber as ManyANum2_1_,
					manyb1_.Id           as Id2_0_,
					manyb1_.Number       as Number2_0_,
					manyb1_.Value        as Value2_0_
			FROM	ManyBs manybs0_
			left outer join ManyB manyb1_ on manybs0_.ManyANumber = manyb1_.Number
			WHERE  manybs0_.ManyBNumber =6 /# ?p0 #/
			 */

			Assert.That(loadedManyA.ManyBs.Count, Is.EqualTo(3));
		}

		[Test, Ignore("Not fixed yet")]
		public void Getting_a_ManyA_object_with_fetchmode_join_will_work()
		{
			ManyA loadedManyA;
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					loadedManyA =
						session
							.CreateCriteria<ManyA>()
							.Add(Restrictions.IdEq(_manyAId))
							.Fetch(SelectMode.Fetch, "ManyBs")
							.UniqueResult<ManyA>();
					transaction.Commit();
				}
			}

			/******************the select statments *************************************************************
			SELECT	this_.Id as Id0_1_,
					this_.Number as Number0_1_,
					this_.Value as Value0_1_,

					manybs2_.ManyBNumber as ManyBNum1_3_,
					manyb3_.Id as ManyANum2_3_,
					manyb3_.Id as Id2_0_,
					manyb3_.Number as Number2_0_,
					manyb3_.Value as Value2_0_

			FROM	ManyA this_

			left outer join ManyBs manybs2_ on this_.Number=manybs2_.ManyBNumber
			left outer join ManyB manyb3_ on manybs2_.ManyANumber=manyb3_.Number

			WHERE	this_.Id = 1 /# ?p0 #/
Exception:
System.Collections.Generic.KeyNotFoundException: Der angegebene Schlüssel war nicht im Wörterbuch angegeben.
   bei System.ThrowHelper.ThrowKeyNotFoundException()
   bei System.Collections.Generic.Dictionary`2.get_Item(TKey key)
   bei NHibernate.Persister.Entity.AbstractEntityPersister.GetAppropriateUniqueKeyLoader(String propertyName, IDictionary`2 enabledFilters) in C:\Users\Armin\Projects\NHibernate\branches\2.1.x\nhibernate\src\NHibernate\Persister\Entity\AbstractEntityPersister.cs:Zeile 2047.
   bei NHibernate.Persister.Entity.AbstractEntityPersister.LoadByUniqueKey(String propertyName, Object uniqueKey, ISessionImplementor session) in C:\Users\Armin\Projects\NHibernate\branches\2.1.x\nhibernate\src\NHibernate\Persister\Entity\AbstractEntityPersister.cs:Zeile 2037.
   bei NHibernate.Type.EntityType.LoadByUniqueKey(String entityName, String uniqueKeyPropertyName, Object key, ISessionImplementor session) in C:\Users\Armin\Projects\NHibernate\branches\2.1.x\nhibernate\src\NHibernate\Type\EntityType.cs:Zeile 552.
			 */

			Assert.That(loadedManyA.ManyBs.Count, Is.EqualTo(3));
		}
	}
}
