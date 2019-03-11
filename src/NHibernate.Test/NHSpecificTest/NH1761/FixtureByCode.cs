using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1761
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<FundingCategory>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Bag(x => x.FundingPrograms, m => {}, r => r.OneToMany());
				
			});
			mapper.Class<FundingProgram>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.ManyToOne(x => x.Recipient);
				rc.Property(x => x.ObligatedAmount);
				rc.Bag(x => x.Projects, m => { m.OrderBy("Name asc"); }, r => r.OneToMany());
			});
			mapper.Class<Project>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.ManyToOne(x => x.Recipient);
				rc.Property(x => x.ObligatedAmount);
			});
			mapper.Class<Recipient>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void InvalidOrderByClause()
		{
			using (var session = OpenSession())
			{
				var recipientId = Guid.NewGuid();
				session.CreateCriteria(typeof(FundingCategory), "fc")
							.CreateCriteria("FundingPrograms", "fp")
							.CreateCriteria("Projects", "p", JoinType.LeftOuterJoin)
							.Add(
								Restrictions.Disjunction()
											.Add(Restrictions.Eq("fp.Recipient.Id", recipientId))
											.Add(Restrictions.Eq("p.Recipient.Id", recipientId))
							)
							.SetProjection(
								Projections.ProjectionList()
											.Add(Projections.GroupProperty("fc.Name"), "fcn")
											.Add(Projections.Sum("fp.ObligatedAmount"), "fpo")
											.Add(Projections.Sum("p.ObligatedAmount"), "po")
							)
							.AddOrder(Order.Desc("fpo"))
							.AddOrder(Order.Desc("po"))
							.AddOrder(Order.Asc("fcn"))
							.List<object[]>();
			}
		}
	}

	public class FundingCategory
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<FundingProgram> FundingPrograms { get; set; } = new List<FundingProgram>();

	}

	public class FundingProgram
	{
		public virtual Guid Id { get; set; }
		public virtual Recipient Recipient { get; set; }
		public virtual decimal ObligatedAmount { get; set; }
		public virtual IList<Project> Projects { get; set; } = new List<Project>();
	}

	public class Project
	{
		public virtual Guid Id { get; set; }
		public virtual Recipient Recipient { get; set; }
		public virtual decimal ObligatedAmount { get; set; }
	}

	public class Recipient
	{
		public virtual Guid Id { get; set; }
	}
}
