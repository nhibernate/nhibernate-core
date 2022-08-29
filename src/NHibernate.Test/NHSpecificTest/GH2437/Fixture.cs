using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2437
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<UserMapping>();
			mapper.AddMapping<UserSessionMapping>();
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var user = new User { UserCode = "gokhanabatay", IsOpen = true, UserName = "Gökhan Abatay" };
				session.Save(user);

				for (var i = 0; i < 10; i++)
				{
					var userSession = new UserSession
					{
						Claims = "My Claims",
						ExpireDateTime = DateTime.Now.AddDays(1),
						MbrId = 1,
						OpenDate = DateTime.Now,
						LocalIpAddress = "192.168.1.1",
						RemoteIpAddress = "127.0.0.1",
						LocalPort = 80 + i.ToString(),
						RemotePort = 80 + i.ToString(),
						DeviceId = "None",
						UserCode = "gokhanabatay",
						IsOpen = true
					};

					session.Save(userSession);
				}

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from UserSession").ExecuteUpdate();
				session.CreateQuery("delete from User").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void Get_DateCustomType_NullableDateValueEquals()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var sessions = session.Query<UserSession>().Where(x => x.OpenDate.Value == DateTime.Now).ToList();

				Assert.That(sessions, Has.Count.EqualTo(10));
			}
		}

		[Test]
		public void Get_DateCustomType_NullableDateValueEqualsMethod()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var sessions = session.Query<UserSession>().Where(x => x.OpenDate.Value.Equals(DateTime.Now)).ToList();

				Assert.That(sessions, Has.Count.EqualTo(10));
			}
		}

		[Test]
		public void Get_DateTimeCustomType_NullableDateValueEquals()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var sessions = session.Query<UserSession>().Where(x => x.ExpireDateTime.Value > DateTime.Now).ToList();

				Assert.That(sessions, Has.Count.EqualTo(10));
			}
		}

		[Test]
		public void Get_DateCustomType_DateEquals()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var sessions = session.Query<UserSession>().Where(x => x.OpenDate == DateTime.Now).ToList();

				Assert.That(sessions, Has.Count.EqualTo(10));
			}
		}

		[Test]
		public void Get_DateTimeCustomType_DateEquals()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var sessions = session.Query<UserSession>().Where(x => x.ExpireDateTime > DateTime.Now).ToList();

				Assert.That(sessions, Has.Count.EqualTo(10));
			}
		}

		[Test]
		public void Get_BooleanCustomType()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var results = session.Query<UserSession>()
					.Where(x => x.OpenDate == DateTime.Now)
					.Select(x => new NullableBooleanResult() { IsOpen = x.User.IsOpen })
					.ToList();

				Assert.That(results, Has.Count.EqualTo(10));
			}
		}

		public class NullableBooleanResult
		{
			public bool? IsOpen { get; set; }
		}
	}
}
