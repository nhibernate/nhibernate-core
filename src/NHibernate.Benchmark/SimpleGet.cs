using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using NHibernate.DomainModel.Northwind.Entities;

namespace NHibernate.Benchmark
{
	public class SimpleGet : NorthwindBenchmarkCase
	{
		private readonly Fixture fixture;

		public SimpleGet()
		{
			fixture = new Fixture();

			fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());
		}

		private static readonly DateTime KnownDate = new DateTime(2010, 06, 17);

		protected override void OnGlobalSetup()
		{
			using (var session = SessionFactory.OpenSession())
			{
				if (session.Query<Role>().Any())
				{
					return;
				}
			}

			var roles = new List<Role>(200);

			for (int i = 0; i < 200; i++)
			{
				roles.Add(
					fixture.Build<Role>()
					       .With(x => x.ParentRole, (Role)null)
					       .With(x => x.Entity, (AnotherEntity)null)
					       .Create());
			}

			using (var session = SessionFactory.OpenSession())
			{
				foreach (var role in roles)
				{
					session.Save(role);
				}

				session.Flush();
			}
		}

		[Params(1, 10, 50, 100, 200)] 
		public int Counter { get; set; }


		[Benchmark]
		public void GetRole()
		{
			for (int i = 0; i < 200; i++)
			{
				using (var session = SessionFactory.OpenSession())
				{
					session.Get<Role>(i);
				}
			}
		}

		[Benchmark]
		public void GetMultiRoleInSameSession()
		{
			using (var session = SessionFactory.OpenSession())
			{
				for (int i = 0; i < 200; i++)
				{
					session.Get<Role>(i);
				}
			}
		}
	}
}
