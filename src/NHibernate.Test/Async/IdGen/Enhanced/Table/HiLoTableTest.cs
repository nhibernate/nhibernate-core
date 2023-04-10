﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NHibernate.Id.Enhanced;
using NHibernate.Test.MultiTenancy;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.Enhanced.Table
{
	using System.Threading.Tasks;
	[TestFixture(null)]
	[TestFixture("test")]
	public class HiLoTableTestAsync : TestCaseWithMultiTenancy
	{
		public HiLoTableTestAsync(string tenantIdentifier) : base(tenantIdentifier)
		{
		}
		protected override string[] Mappings
		{
			get { return new[] { "IdGen.Enhanced.Table.HiLo.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public async Task TestNormalBoundaryAsync()
		{
			var persister = Sfi.GetEntityPersister(typeof(Entity).FullName);
			Assert.That(persister.IdentifierGenerator, Is.TypeOf<TableGenerator>());

			var generator = (TableGenerator)persister.IdentifierGenerator;
			Assert.That(generator.Optimizer, Is.TypeOf<OptimizerFactory.HiLoOptimizer>());
			var optimizer = (OptimizerFactory.HiLoOptimizer)generator.Optimizer;

			int increment = optimizer.IncrementSize;
			Entity[] entities = new Entity[increment + 1];

			using (ISession s = OpenSession())
			{
				using (ITransaction transaction = s.BeginTransaction())
				{
					for (int i = 0; i < increment; i++)
					{
						entities[i] = new Entity("" + (i + 1));
						await (s.SaveAsync(entities[i]));
						Assert.That(generator.TableAccessCount, Is.EqualTo(1)); // initialization
						Assert.That(optimizer.GetLastSourceValue(TenantIdentifier), Is.EqualTo(1)); // initialization
						Assert.That(optimizer.GetLastValue(TenantIdentifier), Is.EqualTo(i + 1));
						Assert.That(optimizer.GetHiValue(TenantIdentifier), Is.EqualTo(increment + 1));
					}

					// now force a "clock over"
					entities[increment] = new Entity("" + increment);
					await (s.SaveAsync(entities[increment]));
					Assert.That(generator.TableAccessCount, Is.EqualTo(2));
					Assert.That(optimizer.GetLastSourceValue(TenantIdentifier), Is.EqualTo(2));
					Assert.That(optimizer.GetLastValue(TenantIdentifier), Is.EqualTo(increment + 1));
					Assert.That(optimizer.GetHiValue(TenantIdentifier), Is.EqualTo((increment * 2) + 1));
					await (transaction.CommitAsync());
				}

				using (ITransaction transaction = s.BeginTransaction())
				{
					for (int i = 0; i < entities.Length; i++)
					{
						Assert.That(entities[i].Id, Is.EqualTo(i + 1));
						await (s.DeleteAsync(entities[i]));
					}
					await (transaction.CommitAsync());
				}
			}
		}
	}
}
