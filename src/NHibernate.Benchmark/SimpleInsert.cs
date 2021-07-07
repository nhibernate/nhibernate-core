using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NHibernate.DomainModel.Northwind.Entities;

namespace NHibernate.Benchmark
{
	public class SimpleInsert : NorthwindBenchmarkCase
	{
		protected override void OnTearDown()
		{
			using (var session = SessionFactory.OpenSession())
			{
				session.CreateQuery("delete from Role").ExecuteUpdate();
			}
		}

		[Params(1, 10, 50, 100, 200)] public int Counter { get; set; }

		[Benchmark]
		public void InsertRole()
		{
			for (int i = 0; i < Counter; i++)
			{
				using (var session = SessionFactory.OpenSession())
				{
					session.Save(CreateRole(i));
					session.Flush();
				}
			}
		}

		[Benchmark]
		public void InsertMultiRoleOnce()
		{
			using (var session = SessionFactory.OpenSession())
			{
				for (int i = 0; i < Counter; i++)
				{
					session.Save(CreateRole(i));
				}

				session.Flush();
			}
		}

		[Benchmark]
		public void InsertRoleInParallel()
		{
			Parallel.For(
				0,
				Counter,
				i =>
				{
					using (var session = SessionFactory.OpenSession())
					{
						session.Save(CreateRole(i));
						session.Flush();
					}
				});
		}

		[Benchmark]
		public void InsertRoleWithTransaction()
		{
			for (int i = 0; i < Counter; i++)
			{
				using (var session = SessionFactory.OpenSession())
				{
					using (var trans = session.BeginTransaction())
					{
						session.Save(CreateRole(i));
						session.Flush();
						trans.Commit();
					}
				}
			}
		}

		[Benchmark]
		public void InsertMultiRoleWithTransactionOnce()
		{
			using (var session = SessionFactory.OpenSession())
			{
				using (var trans = session.BeginTransaction())
				{
					for (int i = 0; i < Counter; i++)
					{
						session.Save(CreateRole(i));
					}
					
					session.Flush();
					trans.Commit();
				}
			}
		}

		[Benchmark]
		public void InsertRoleWithTransactionInParallel()
		{
			Parallel.For(
				0,
				Counter,
				i =>
				{
					using (var session = SessionFactory.OpenSession())
					{
						using (var trans = session.BeginTransaction())
						{
							session.Save(CreateRole(i));
							session.Flush();
							trans.Commit();
						}
					}
				});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Role CreateRole(int id)
		{
			return new Role
			{
				Id = id,
				Name = Guid.NewGuid().ToString(),
				IsActive = id % 2 == 0
			};
		}
	}
}
