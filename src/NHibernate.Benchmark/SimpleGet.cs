using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using NHibernate.Benchmark.Database.Northwind.Entities;

namespace NHibernate.Benchmark
{
	public class SimpleGet : NorthwindBenchmarkCase
	{
		private static readonly DateTime KnownDate = new DateTime(2010, 06, 17);

		protected override void OnGlobalSetup()
		{
			var roles = new[]
			{
				new Role
				{
					Name = "Admin",
					IsActive = true,
					Entity = new AnotherEntity
					{
						Output = "output"
					}
				},
				new Role
				{
					Name = "User",
					IsActive = false
				}
			};

			var users = new[]
			{
				new User("ayende", KnownDate)
				{
					Id = 1,
					Role = roles[0],
					InvalidLoginAttempts = 4,
					Enum1 = EnumStoredAsString.Medium,
					Enum2 = EnumStoredAsInt32.High,
					Component = new UserComponent
					{
						Property1 = "test1",
						Property2 = "test2",
						OtherComponent = new UserComponent2
						{
							OtherProperty1 = "othertest1"
						}
					}
				},
				new User("rahien", new DateTime(1998, 12, 31))
				{
					Id = 2,
					Role = roles[1],
					InvalidLoginAttempts = 5,
					Enum1 = EnumStoredAsString.Small,
					Component = new UserComponent
					{
						Property2 = "test2"
					}
				},
				new User("nhibernate", new DateTime(2000, 1, 1))
				{
					Id = 3,
					InvalidLoginAttempts = 6,
					LastLoginDate = DateTime.Now.AddDays(-1),
					Enum1 = EnumStoredAsString.Medium,
					Features = FeatureSet.HasAll
				}
			};

			var timesheets = new[]
			{
				new Timesheet
				{
					Id = 1,
					SubmittedDate = KnownDate,
					Submitted = true
				},
				new Timesheet
				{
					Id = 2,
					SubmittedDate = KnownDate.AddDays(-1),
					Submitted = false,
					Entries = new List<TimesheetEntry>
					{
						new TimesheetEntry
						{
							Id = 1,
							EntryDate = KnownDate,
							NumberOfHours = 6,
							Comments = "testing 123"
						},
						new TimesheetEntry
						{
							Id = 2,
							EntryDate = KnownDate.AddDays(1),
							NumberOfHours = 14
						}
					}
				},
				new Timesheet
				{
					Id = 3,
					SubmittedDate = DateTime.Now.AddDays(1),
					Submitted = true,
					Entries = new List<TimesheetEntry>
					{
						new TimesheetEntry
						{
							Id = 3,
							EntryDate = DateTime.Now.AddMinutes(20),
							NumberOfHours = 4
						},
						new TimesheetEntry
						{
							Id = 4,
							EntryDate = DateTime.Now.AddMinutes(10),
							NumberOfHours = 8,
							Comments = "testing 456"
						},
						new TimesheetEntry
						{
							Id = 5,
							EntryDate = DateTime.Now.AddMinutes(13),
							NumberOfHours = 7
						},
						new TimesheetEntry
						{
							Id = 6,
							EntryDate = DateTime.Now.AddMinutes(45),
							NumberOfHours = 38
						}
					}
				}
			};

			((IList<User>) timesheets[0].Users).Add(users[0]);
			((IList<User>) timesheets[1].Users).Add(users[0]);
			((IList<User>) timesheets[0].Users).Add(users[1]);

			using (var session = SessionFactory.OpenSession())
			{
				foreach (Role role in roles)
				{
					session.Save(role);
				}

				session.Flush();

				foreach (User user in users)
				{
					session.Save(user);
				}

				session.Flush();

				foreach (Timesheet timesheet in timesheets)
				{
					session.Save(timesheet);
				}

				session.Flush();
			}
		}


		private static int userCounter = 0;

		[Benchmark]
		public User GetUser()
		{
			var session = SessionFactory.OpenSession();
			return session.Get<User>((userCounter++) % 3);
		}
	}
}
