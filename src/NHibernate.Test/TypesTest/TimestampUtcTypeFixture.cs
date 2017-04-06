using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Test fixture for type <see cref="TimestampUtcType"/>.
	/// </summary>
	[TestFixture]
	public class TimestampUtcTypeFixture : TypeFixtureBase
	{
		readonly TimestampUtcType _type = NHibernateUtil.TimestampUtc;
		readonly DateTime _utc = new DateTime(1976, 11, 30, 10, 0, 0, 300, DateTimeKind.Utc);
		readonly DateTime _local = new DateTime(1976, 11, 30, 10, 0, 0, 300, DateTimeKind.Local);
		readonly DateTime _unspecified = new DateTime(1976, 11, 30, 10, 0, 0, 300, DateTimeKind.Unspecified);

		/// <summary>
		/// 1976-11-30T10:00:00.3000000
		/// </summary>
		const long DateInTicks = 623537928003000000;
		
		protected override string TypeName => "TimestampUtc";

		[Test]
		public void Next()
		{
			var current = DateTime.Parse("2004-01-01");
			var next = (DateTime)_type.Next(current, null);

			Assert.AreEqual(DateTimeKind.Utc, next.Kind, "Kind is not Utc");
			Assert.IsTrue(next > current, "next should be greater than current (could be equal depending on how quickly this occurs)");
		}

		/// <summary>
		/// Perform a 'seed' and check if the result is a datetime with kind set to Utc.
		/// </summary>
		[Test]
		public void Seed()
		{
			var type = NHibernateUtil.TimestampUtc;
			Assert.IsTrue(type.Seed(null) is DateTime, "Seed should be DateTime");

			var value = (DateTime)type.Seed(null);
			Assert.AreEqual(DateTimeKind.Utc, value.Kind, "Kind should be Utc");
		}

		/// <summary>
		/// Perform a basis write with a DateTime value where Kind is Local which should fail.
		/// </summary>
		[Test]
		[TestCase(DateTimeKind.Unspecified)]
		[TestCase(DateTimeKind.Local)]
		public void LocalReadWrite_Fail(DateTimeKind kind)
		{
			var entity = new TimestampUtcClass
			{
				Id = 1,
				Value = DateTime.SpecifyKind(DateTime.Now, kind)
			};

			using(var session = OpenSession())
			using(var tx = session.BeginTransaction())
			{
				session.Save(entity);
				Assert.That(() => session.Flush(), Throws.TypeOf<PropertyValueException>());
				tx.Rollback();
			}
		}

		/// <summary>
		/// Create two session. Write entity in the first and read it in the second and compare if
		/// the retrieved timestamp value still equals the original value.
		/// </summary>
		/// <remarks> This test takes the database precision into consideration.</remarks>
		[Test]
		public void UtcReadWrite_Success()
		{
			TimestampUtcClass entity;

			// Save
			using(var session = OpenSession())
			using(var tx = session.BeginTransaction())
			{
				// Create a new datetime value and round it to the precision that the database supports. This
				// code basically the same as in the implementation but here to guard posible changes.
				var resolution = session.GetSessionImplementation().Factory.Dialect.TimestampResolutionInTicks;
				var next = DateTime.UtcNow;
				next = next.AddTicks(-(next.Ticks % resolution));

				entity = new TimestampUtcClass
				{
					Id = 1,
					Value = next
				};

				session.Save(entity);
				tx.Commit();
				session.Close();
			}

			// Retrieve and compare
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var result = session.Get<TimestampUtcClass>(entity.Id);
				Assert.IsNotNull(result, "Entity not saved or cannot be retrieved by its key.");

				// Property: Value
				Assert.AreEqual(DateTimeKind.Utc, result.Value.Kind, "Kind is NOT Utc");
				Assert.AreEqual(entity.Value.Ticks, result.Value.Ticks, "Value should be the same.");

				// Property: Revision
				var revision = result.Revision;
				Assert.AreEqual(DateTimeKind.Utc, revision.Kind, "Kind is NOT Utc");

				var differenceInSeconds = Math.Abs((revision - DateTime.UtcNow).TotalSeconds);
				Assert.IsTrue(differenceInSeconds < 1d, "Difference should be less then 1 second.");

				tx.Commit();
				session.Close();
			}

			// Delete
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var result = session.Get<TimestampUtcClass>(entity.Id);
				session.Delete(result);
				tx.Commit();
				session.Close();
			}
		}

		/// <summary>
		/// Tests if the type FromStringValue implementation behaves as expected.
		/// </summary>
		/// <param name="timestampValue"></param>
		[Test]
		[TestCase("2011-01-27T15:50:59.6220000+02:00")]
		[TestCase("2011-01-27T14:50:59.6220000+01:00")]
		[TestCase("2011-01-27T13:50:59.6220000Z")]
		public void FromStringValue_ParseValidValues(string timestampValue)
		{
			var timestamp = DateTime.Parse(timestampValue);

			Assert.AreEqual(DateTimeKind.Local, timestamp.Kind, "Kind is NOT Local. dotnet framework parses datetime values with kind set to Local and time correct to local timezone.");

			timestamp = timestamp.ToUniversalTime();

			var value = (DateTime)_type.FromStringValue(timestampValue);

			Assert.AreEqual(timestamp, value, timestampValue);
			Assert.AreEqual(DateTimeKind.Utc, value.Kind, "Kind is NOT Utc");
		}

		/// <summary>
		/// Test the framework tostring behavior. If the test fails then the <see cref="TimestampType"/> and <see cref="TimestampUtcType"/> implemention could not work propertly at run-time.
		/// </summary>
		[Test, Category("Expected framework behavior")]
		[TestCase(623537928003000000, DateTimeKind.Utc, ExpectedResult = "1976-11-30T10:00:00.3000000Z")]
		[TestCase(623537928003000000, DateTimeKind.Unspecified, ExpectedResult = "1976-11-30T10:00:00.3000000")]
		[TestCase(623537928003000000, DateTimeKind.Local, ExpectedResult = "1976-11-30T10:00:00.3000000+01:00",
			Ignore = "Offset depends on which system this test is run and can currently now be influenced via the .net framework",
			Description ="This test will ONLY succeed when the test is run on a system which if currently in a timezone with offset +01:00")]
		public string ExpectedToStringDotnetFrameworkBehavior(long ticks, DateTimeKind kind)
		{
			return new DateTime(ticks, kind).ToString("o");
		}

		/// <summary>
		/// Test the framework tostring behavior. If the test fails then the <see cref="TimestampType"/> and <see cref="TimestampUtcType"/> implemention could not work propertly at run-time.
		/// </summary>
		[Test, Category("Expected framework behavior")]
		public void ExpectedIsEqualDotnetFrameworkBehavior()
		{
			const string assertMessage = "Values should be equal dotnet framework ignores Kind value.";
			Assert.AreEqual(_utc, _local, assertMessage);
			Assert.AreEqual(_utc, _unspecified, assertMessage);
			Assert.AreEqual(_unspecified, _local, assertMessage);
		}
	}
}
