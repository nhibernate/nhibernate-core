using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2379
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Order>(rc =>
				{
					rc.Table("Orders");
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.Set(x => x.OrderLines, m =>
						{
							m.Inverse(true);
							m.Key(k =>
								{
									k.Column("OrderId");
									k.NotNullable(true);
								});
							m.Cascade(Mapping.ByCode.Cascade.All.Include(Mapping.ByCode.Cascade.DeleteOrphans));
							m.Access(Accessor.NoSetter);
						}, m => m.OneToMany());
				});
			mapper.Class<OrderLine>(rc =>
				{
					rc.Table("OrderLines");
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.ManyToOne(x => x.Order, m => m.Column("OrderId"));
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var o1 = new Order {Name = "Order 1"};
				session.Save(o1);

				var o2 = new Order {Name = "Order 2"};
				session.Save(o2);

				session.Save(new OrderLine {Name = "Order Line 2 - 1", Order = o2});

				var o3 = new Order {Name = "Order 3"};
				session.Save(o3);

				session.Save(new OrderLine {Name = "Order Line 3 - 1", Order = o3});
				session.Save(new OrderLine {Name = "Order Line 3 - 2", Order = o3});

				var o4 = new Order {Name = "Order 4"};
				session.Save(o4);

				session.Save(new OrderLine {Name = "Order Line 4 - 1", Order = o4});
				session.Save(new OrderLine {Name = "Order Line 4 - 2", Order = o4});
				session.Save(new OrderLine {Name = "Order Line 4 - 3", Order = o4});

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void InnerJoin()
		{
			// 
			// select
			//     order0_.Id as col_0_0_,
			//     orderlines1_.Id as col_1_0_ 
			// from
			//     Orders order0_ 
			// inner join
			//     OrderLines orderlines1_ 
			//         on order0_.Id=orderlines1_.OrderId
			// 

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from o in session.Query<Order>()
							  from ol in o.OrderLines
							  select new {OrderId = o.Id, OrderLineId = (Guid?) ol.Id}).ToList();

				Assert.AreEqual(6, result.Count);
			}
		}

		[Test]
		public void InnerJoinWithRestriction()
		{
			// 
			// select
			//     order0_.Id as col_0_0_,
			//     orderlines1_.Id as col_1_0_ 
			// from
			//     Orders order0_ 
			// inner join
			//     OrderLines orderlines1_ 
			//         on order0_.Id=orderlines1_.OrderId
			//         and orderlines1_.Name like ('Order Line 3') 
			// 

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from o in session.Query<Order>()
							  from ol in o.OrderLines.Where(x => x.Name.StartsWith("Order Line 3"))
							  select new { OrderId = o.Id, OrderLineId = (Guid?)ol.Id }).ToList();

				Assert.AreEqual(2, result.Count);
			}
		}

		[Test]
		public void InnerJoinWithOutermostRestriction()
		{
			// 
			// select
			//     order0_.Id as col_0_0_,
			//     orderlines1_.Id as col_1_0_ 
			// from
			//     Orders order0_ 
			// inner join
			//     OrderLines orderlines1_ 
			//         on order0_.Id=orderlines1_.OrderId
			// where
			//     orderlines1_.Name like ('Order Line 3')
			// 

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from o in session.Query<Order>()
							  from ol in o.OrderLines
							  where ol.Name.StartsWith("Order Line 3")
							  select new { OrderId = o.Id, OrderLineId = (Guid?)ol.Id }).ToList();

				Assert.AreEqual(2, result.Count);
			}
		}

		[Test]
		public void LeftOuterJoin()
		{
			// 
			// select
			//     order0_.Id as col_0_0_,
			//     orderlines1_.Id as col_1_0_ 
			// from
			//     Orders order0_ 
			// left outer join
			//     OrderLines orderlines1_ 
			//         on order0_.Id=orderlines1_.OrderId
			// 

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from o in session.Query<Order>()
							  from ol in o.OrderLines.DefaultIfEmpty()
							  select new {OrderId = o.Id, OrderLineId = (Guid?) ol.Id}).ToList();

				Assert.AreEqual(7, result.Count);
			}
		}

		[Test]
		public void LeftOuterJoinWithInnerRestriction()
		{
			// 
			// select
			//     order0_.Id as col_0_0_,
			//     orderlines1_.Id as col_1_0_ 
			// from
			//     Orders order0_ 
			// left outer join
			//     OrderLines orderlines1_ 
			//         on order0_.Id=orderlines1_.OrderId
			//         and orderlines1_.Name like ('Order Line 3') 
			// 

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from o in session.Query<Order>()
							  from ol in o.OrderLines.Where(x => x.Name.StartsWith("Order Line 3")).DefaultIfEmpty()
							  select new {OrderId = o.Id, OrderLineId = (Guid?) ol.Id}).ToList();

				Assert.AreEqual(5, result.Count);
			}
		}

		[Test]
		public void LeftOuterJoinWithOuterRestriction()
		{
			//TODO: should it be an inner join? As .DefaultIfEmpty() does not make any sense here.

			// 
			// select
			//     order0_.Id as col_0_0_,
			//     orderlines1_.Id as col_1_0_ 
			// from
			//     Orders order0_ 
			// left outer join
			//     OrderLines orderlines1_ 
			//         on order0_.Id=orderlines1_.OrderId
			// where
			//     orderlines1_.Name like ('Order Line 3')
			// 

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from o in session.Query<Order>()
							  from ol in o.OrderLines.DefaultIfEmpty().Where(x => x.Name.StartsWith("Order Line 3"))
							  select new {OrderId = o.Id, OrderLineId = (Guid?) ol.Id}).ToList();

				Assert.AreEqual(2, result.Count);
			}
		}

		[Test]
		public void LeftOuterJoinWithOutermostRestriction()
		{
			// 
			// select
			//     order0_.Id as col_0_0_,
			//     orderlines1_.Id as col_1_0_ 
			// from
			//     Orders order0_ 
			// left outer join
			//     OrderLines orderlines1_ 
			//         on order0_.Id=orderlines1_.OrderId
			// where
			//     orderlines1_.Name like ('Order Line 3')
			// 

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from o in session.Query<Order>()
							  from ol in o.OrderLines.DefaultIfEmpty()
							  where ol.Name.StartsWith("Order Line 3")
							  select new {OrderId = o.Id, OrderLineId = (Guid?) ol.Id}).ToList();

				Assert.AreEqual(2, result.Count);
			}
		}
	}
}