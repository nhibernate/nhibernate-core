﻿using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2330
{
	[TestFixture]
	public class JoinedSubclassWithClauseFixture : TestCaseMappingByCode
	{
		private object _visit1Id;
		private object _visit2Id;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Node>(ca =>
			{
				ca.Id(x => x.Id, map => map.Generator(Generators.Identity));
				ca.Property(x => x.Deleted);
				ca.Property(x => x.FamilyName);
				ca.Table("Node");
				ca.Abstract(true);
			});

			mapper.JoinedSubclass<PersonBase>(
				ca =>
				{
					ca.Key(x => x.Column("FK_Node_ID"));
					ca.Extends(typeof(Node));
					ca.Property(x => x.Deleted);
					ca.Property(x => x.Login);
				});

			mapper.Class<UserEntityVisit>(
				ca =>
				{
					ca.Id(x => x.Id, map => map.Generator(Generators.Identity));
					ca.Property(x => x.Deleted);
					ca.ManyToOne(x => x.PersonBase);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var arrangeSession = OpenSession())
			using (var tx = arrangeSession.BeginTransaction())
			{
				var person = new PersonBase {Login = "dave", FamilyName = "grohl"};
				arrangeSession.Save(person);
				_visit1Id = arrangeSession.Save(new UserEntityVisit {PersonBase = person});
				_visit2Id = arrangeSession.Save(new UserEntityVisit {PersonBase = person});
				arrangeSession.Flush();

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				transaction.Commit();
			}
		}
		
		[Test]
		public void Join_Inheritance()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var results = session
					.CreateCriteria<UserEntityVisit>()
					.CreateCriteria(
						$"{nameof(UserEntityVisit.PersonBase)}",
						"f",
						SqlCommand.JoinType.LeftOuterJoin,
						Restrictions.Eq("Deleted", false))
					.List<UserEntityVisit>()
					.Select(x => x.Id);

				Assert.That(results, Is.EquivalentTo(new[] {_visit1Id, _visit2Id,}));
			}
		}

		[Test]
		public void Join_Inheritance_QueryOver()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				PersonBase f = null;
				var results = session.QueryOver<UserEntityVisit>()
					.JoinAlias(
						x => x.PersonBase,
						() => f,
						SqlCommand.JoinType.LeftOuterJoin,
						Restrictions.Where(() => f.Deleted == false))
					.List()
					.Select(x => x.Id);

				Assert.That(results, Is.EquivalentTo(new[] {_visit1Id, _visit2Id,}));
			}
		}
	}
}
