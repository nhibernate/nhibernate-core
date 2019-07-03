using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NHibernate.Benchmark.Database.Northwind.Entities;

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

		[Params(200)]
		public int Counter { get; set; }
		
		[Benchmark]
		public void AddRole()
		{
			for (int i = 0; i < Counter; i++)
			{
				var role = new Role
				{
					Name = Guid.NewGuid().ToString(),
					IsActive = true
				};
			
				using (var session = SessionFactory.OpenSession())
				{
					session.Save(role);
					session.Flush();
				}	
			}
		}
		
		[Benchmark]
		public void AddRoleInParallel()
		{
			Parallel.For(0, Counter, i =>
			{
				var role = new Role
				{
					Name = Guid.NewGuid().ToString(),
					IsActive = true
				};

				using (var session = SessionFactory.OpenSession())
				{
					session.Save(role);
					session.Flush();
				}
			});
		}
		
		[Benchmark]
		public void AddRoleWithTransaction()
		{
			for (int i = 0; i < Counter; i++)
			{
				var role = new Role
				{
					Name = Guid.NewGuid().ToString(),
					IsActive = true
				};

				using (var session = SessionFactory.OpenSession())
				{
					using (var trans = session.BeginTransaction())
					{
						session.Save(role);
						session.Flush();
						trans.Commit();
					}
				}
			}
		}
		
		[Benchmark]
		public void AddRoleWithTransactionInParallel()
		{
			Parallel.For(0, Counter, i =>
			{
				var role = new Role
				{
					Name = Guid.NewGuid().ToString(),
					IsActive = true
				};

				using (var session = SessionFactory.OpenSession())
				{
					using (var trans = session.BeginTransaction())
					{
						session.Save(role);
						session.Flush();
						trans.Commit();
					}
				}
			});
		}
	}
}
