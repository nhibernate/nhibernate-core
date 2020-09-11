using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2529
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// This fixture is identical to <see cref="Fixture" /> except the <see cref="User" /> mapping is performed 
	/// by code in the GetMappings method, and does not require the <c>Mappings.hbm.xml</c> file. Use this approach
	/// if you prefer.
	/// </remarks>
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			configuration.LinqToHqlGeneratorsRegistry<EnhancedLinqToHqlGeneratorsRegistry>();
			configuration.SetProperty(Cfg.Environment.Dialect, typeof(EnhancedMsSql2008Dialect).AssemblyQualifiedName);
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is EnhancedMsSql2008Dialect;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<User>(rc =>
			{
				rc.Table("Users");
				rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
				rc.Property(x => x.Birthday);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test, Explicit]
		public void List_DealsWithYearTransitionProperly()
		{
			// arrange
			DateTimeOffset today = new DateTimeOffset(2001, 12, 27, 08, 00, 0, TimeSpan.Zero);

			CreateUser(birthday: new DateTime(1991, 12, 31));
			CreateUser(birthday: new DateTime(1992, 01, 01));

			// act
			IReadOnlyCollection<BirthdayListItem> result = List(
				today
			);

			// arrange
			Assert.That(result.Count, Is.EqualTo(2));
			BirthdayListItem firstBirthday = result.First();
			BirthdayListItem secondBirthday = result.Skip(1).Single();
			Assert.That(firstBirthday.IsThisYearsBirthdayInThePastNHibernate, Is.EqualTo(firstBirthday.IsThisYearsBirthdayInThePastManual));
			Assert.That(secondBirthday.IsThisYearsBirthdayInThePastNHibernate, Is.EqualTo(secondBirthday.IsThisYearsBirthdayInThePastManual));
		}

		[Test, Explicit]
		public void List_WorksForTomorrow()
		{
			// arrange
			DateTimeOffset today = new DateTime(2000, 05, 11, 00, 00, 0, DateTimeKind.Utc);
			CreateUser(birthday: today.AddDays(1).DateTime);

			// act
			IReadOnlyCollection<BirthdayListItem> result = List(
				today
			);

			// arrange
			Assert.That(result.Count, Is.EqualTo(1));
			BirthdayListItem firstBirthday = result.First();
			Assert.That(firstBirthday.IsThisYearsBirthdayInThePastNHibernate, Is.EqualTo(firstBirthday.IsThisYearsBirthdayInThePastManual));
		}

		[Test]
		public void List_Both()
		{
			// Running these methods individual passes the test
			// Running them in sequence fails on List_WorksForTomorrow
			List_DealsWithYearTransitionProperly();
			DeleteAllUsers();
			List_WorksForTomorrow();
		}

		private User CreateUser(
			DateTime birthday
		)
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var user = new User
				{
					Birthday = birthday,
				};

				session.Save(user);
				transaction.Commit();

				return user;
			}
		}

		private void DeleteAllUsers()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				string queryString = $"DELETE {typeof(User).FullName}";
				IQuery query = session.CreateQuery(queryString);
				query.ExecuteUpdate();

				transaction.Commit();
			}
		}

		private IReadOnlyCollection<BirthdayListItem> List(
			DateTimeOffset today
		)
		{
			using (ISession session = OpenSession())
			{
				return session.Query<User>()
					.Where(x => x.Birthday != null)
					.Select(x => new
					{
						BirthdayThisYear = x.Birthday.Value.AddYears(today.Year - x.Birthday.Value.Year),
						BirthdayNextYear = x.Birthday.Value.AddYears((today.Year - x.Birthday.Value.Year) + 1),
						IsThisYearsBirthdayInThePast = x.Birthday.Value.AddYears(today.Year - x.Birthday.Value.Year).DayOfYear < today.DayOfYear,
					})
					.OrderBy(x => x.IsThisYearsBirthdayInThePast ? x.BirthdayNextYear : x.BirthdayThisYear)
					.ToArray()
					.Select(x => new BirthdayListItem
					{
						IsThisYearsBirthdayInThePastNHibernate = x.IsThisYearsBirthdayInThePast,
						IsThisYearsBirthdayInThePastManual = x.BirthdayThisYear.DayOfYear < today.DayOfYear,
					})
					.ToArray();
			}
		}

		public class BirthdayListItem
		{
			public bool IsThisYearsBirthdayInThePastNHibernate { get; set; }
			public bool IsThisYearsBirthdayInThePastManual { get; set; }
		}
	}
}
