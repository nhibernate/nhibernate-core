﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Hql
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class SubQueryTestAsync : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Root>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Native));
					rc.Property(x => x.RootName);
					rc.ManyToOne(x => x.Branch);
				});

			mapper.Class<Branch>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Native));
					rc.Property(x => x.BranchName);
					rc.Bag(x => x.Leafs, cm => cm.Cascade(Mapping.ByCode.Cascade.All), x => x.OneToMany());
				});
			mapper.Class<Leaf>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Native));
					rc.Property(x => x.LeafName);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsScalarSubSelects;
		}

		[TestCase("SELECT CASE WHEN l.id IS NOT NULL THEN (SELECT COUNT(r.id) FROM Root r) ELSE 0 END FROM Leaf l")]
		[TestCase("SELECT CASE WHEN (SELECT COUNT(r.id) FROM Root r) > 1 THEN 1 ELSE 0 END FROM Leaf l")]
		[TestCase("SELECT CASE WHEN  l.id > 1 THEN 1 ELSE (SELECT COUNT(r.id) FROM Root r) END FROM Leaf l")]
		[TestCase("SELECT CASE (SELECT COUNT(r.id) FROM Root r) WHEN  1 THEN 1 ELSE 0 END FROM Leaf l")]
		[TestCase("SELECT CASE l.id WHEN (SELECT COUNT(r.id) FROM Root r) THEN 1 ELSE 0 END FROM Leaf l")]
		public async Task TestSubQueryAsync(string query, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// Simple syntax check
				await (session.CreateQuery(query).ListAsync(cancellationToken));
				await (transaction.CommitAsync(cancellationToken));
			}
		}
	}
}
