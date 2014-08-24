using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2100
{
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ConventionModelMapper();
			System.Type baseEntityType = typeof (DomainObject);
			mapper.IsEntity((t, declared) => baseEntityType.IsAssignableFrom(t) && baseEntityType != t);
			mapper.IsRootEntity((t, declared) => baseEntityType == t.BaseType);
			mapper.Class<DomainObject>(r =>
									   {
										r.Version(x => x.EntityVersion, map => { });
										r.Id(x => x.ID, map => map.Generator(Generators.Native));
									   });
			mapper.Class<Class1>(r => r.IdBag(x => x.Class2List, map => map.Inverse(true), rel => rel.ManyToMany()));
			mapper.Class<Class2>(r => r.IdBag<Class1>("_class1List", map => map.Table("Class1List"), rel => rel.ManyToMany()));
			HbmMapping mappings = mapper.CompileMappingFor(new[] {typeof (Class1), typeof (Class2)});
			return mappings;
		}

		[Test]
		public void WhenTwoTransactionInSameSessionThenNotChangeVersion()
		{
			// the second transaction does not change the entity state
			Class1 c1_1;
			Class1 c1_2;
			Class2 c2_1;
			Class2 c2_2;
			int originalVersionC1_1;
			int originalVersionC1_2;
			int originalVersionC2_1;
			int originalVersionC2_2;
			c1_1 = new Class1();
			c1_2 = new Class1();

			c2_1 = new Class2();
			c2_2 = new Class2();

			c1_1.AddClass2(c2_1);
			c1_2.AddClass2(c2_2);
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Save(c2_1);
					s.Save(c1_1);
					s.Save(c2_2);
					s.Save(c1_2);
					tx.Commit();
				}
				originalVersionC1_1 = c1_1.EntityVersion;
				originalVersionC1_2 = c1_2.EntityVersion;
				originalVersionC2_1 = c2_1.EntityVersion;
				originalVersionC2_2 = c2_2.EntityVersion;

				using (ITransaction tx = s.BeginTransaction())
				{
					s.Refresh(c1_1); // The addition of these two Refresh calls fixes the entity version issue
					s.Refresh(c1_2);

					var class1dto =
						new Class1DTO {ID = c1_1.ID, EntityVersion = c1_1.EntityVersion};

					if (c1_1.Class2List.Count > 0)
					{
						class1dto.Class2Ary = new Class2DTO[c1_1.Class2List.Count];
						for (int i = 0; i < c1_1.Class2List.Count; ++i)
						{
							Class2 cl2 = c1_1.Class2List[i];
							class1dto.Class2Ary[i] = new Class2DTO {ID = cl2.ID, EntityVersion = cl2.EntityVersion};
						}
					}

					tx.Commit();
				}
				// After close the second transaction the version was not changed
				c1_1.EntityVersion.Should().Be(originalVersionC1_1);
				c1_2.EntityVersion.Should().Be(originalVersionC1_2);
				c2_1.EntityVersion.Should().Be(originalVersionC2_1);
				c2_2.EntityVersion.Should().Be(originalVersionC2_2);
			}

			// After close the session the version was not changed
			c1_1.EntityVersion.Should().Be(originalVersionC1_1);
			c1_2.EntityVersion.Should().Be(originalVersionC1_2);
			c2_1.EntityVersion.Should().Be(originalVersionC2_1);
			c2_2.EntityVersion.Should().Be(originalVersionC2_2);

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					c2_1 = s.Get<Class2>(c2_1.ID);
					c1_1 = s.Get<Class1>(c1_1.ID);
					c2_2 = s.Get<Class2>(c2_2.ID);
					c1_2 = s.Get<Class1>(c1_2.ID);

					// to be 100% sure the version was not changed in DB
					c1_1.EntityVersion.Should().Be(originalVersionC1_1);
					c1_2.EntityVersion.Should().Be(originalVersionC1_2);
					c2_1.EntityVersion.Should().Be(originalVersionC2_1);
					c2_2.EntityVersion.Should().Be(originalVersionC2_2);

					s.Delete(c2_1);
					s.Delete(c1_1);
					s.Delete(c2_2);
					s.Delete(c1_2);
					tx.Commit();
				}
			}
		}
	}
}