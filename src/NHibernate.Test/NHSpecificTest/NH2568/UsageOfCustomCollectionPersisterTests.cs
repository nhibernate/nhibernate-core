using System;
using System.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.Persister.Collection;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2568
{
	public class MyEntity
	{
		public virtual int Id { get; set; }
		public virtual ICollection<MyRelated> Relateds { get; set; }
	}
	public class MyRelated
	{
		public virtual int Id { get; set; }
	}

	public class UsageOfCustomCollectionPersisterTests
	{
		private HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyRelated>(rm=> rm.Id(x=> x.Id));
			mapper.Class<MyEntity>(rm =>
			                       {
			                       	rm.Id(x => x.Id);
															rm.Bag(x => x.Relateds, am => am.Persister<MyCollectionPersister>(), rel=> rel.OneToMany());
			                       });
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			return mappings;
		}

		[Test]
		public void BuildingSessionFactoryShouldNotThrows()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddMapping(GetMappings());
			cfg.Executing(c=>c.BuildSessionFactory()).NotThrows();
		}
	}

	public class MyCollectionPersister: OneToManyPersister
	{
		public MyCollectionPersister(Mapping.Collection collection, ICacheConcurrencyStrategy cache, Configuration cfg, ISessionFactoryImplementor factory) : base(collection, cache, cfg, factory) {}
	}
}