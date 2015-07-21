using System;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class AggregateTests : LinqTestCase
	{
		[Test]
		public void AggregateWithStartsWith()
		{
			var query = (from c in db.Customers where c.CustomerId.StartsWith("A") select c.CustomerId)
				.Aggregate(new StringBuilder(), (sb, id) => sb.Append(id).Append(","));

			Console.WriteLine(query);
			Assert.AreEqual("ALFKI,ANATR,ANTON,AROUT,", query.ToString());
		}

		[Test]
		public void AggregateWithEndsWith()
		{
			var query = (from c in db.Customers where c.CustomerId.EndsWith("TH") select c.CustomerId)
				.Aggregate(new StringBuilder(), (sb, id) => sb.Append(id).Append(","));

			Console.WriteLine(query);
			Assert.AreEqual("WARTH,", query.ToString());
		}

		[Test]
		public void AggregateWithContains()
		{
			var query = (from c in db.Customers where c.CustomerId.Contains("CH") select c.CustomerId)
				.Aggregate(new StringBuilder(), (sb, id) => sb.Append(id).Append(","));

			Console.WriteLine(query);
			Assert.AreEqual("CHOPS,RANCH,", query.ToString());
		}

		[Test]
		public void AggregateWithEquals()
		{
			var query = (from c in db.Customers
						 where c.CustomerId.Equals("ALFKI") || c.CustomerId.Equals("ANATR") || c.CustomerId.Equals("ANTON")
						 select c.CustomerId)
				.Aggregate((prev, next) => (prev + "," + next));

			Console.WriteLine(query);
			Assert.AreEqual("ALFKI,ANATR,ANTON", query);
		}

		[Test]
		public void AggregateWithNotStartsWith()
		{
			var query = (from c in db.Customers
						 where c.CustomerId.StartsWith("A") && !c.CustomerId.StartsWith("AN")
						 select c.CustomerId)
				.Aggregate(new StringBuilder(), (sb, id) => sb.Append(id).Append(","));

			Console.WriteLine(query);
			Assert.AreEqual("ALFKI,AROUT,", query.ToString());
		}

		[Test]
		public void AggregateWithMonthFunction()
		{
			var date = new DateTime(2007, 1, 1);

			var query = (from e in db.Employees
						 where e.BirthDate.Value.Month == date.Month
						 select e.FirstName)
				.Aggregate(new StringBuilder(), (sb, name) => sb.Length > 0 ? sb.Append(", ").Append(name) : sb.Append(name));

			Console.WriteLine("{0} Birthdays:", date.ToString("MMMM"));
			Console.WriteLine(query);
		}

		[Test]
		public void AggregateWithBeforeYearFunction()
		{
			var date = new DateTime(1960, 1, 1);

			var query = (from e in db.Employees
						 where e.BirthDate.Value.Year < date.Year
						 select e.FirstName.ToUpper())
				.Aggregate(new StringBuilder(), (sb, name) => sb.Length > 0 ? sb.Append(", ").Append(name) : sb.Append(name));

			Console.WriteLine("Birthdays before {0}:", date.ToString("yyyy"));
			Console.WriteLine(query);
		}

		[Test]
		public void AggregateWithOnOrAfterYearFunction()
		{
			var date = new DateTime(1960, 1, 1);

			var query = (from e in db.Employees
						 where e.BirthDate.Value.Year >= date.Year && e.FirstName.Length > 4
						 select e.FirstName)
				.Aggregate(new StringBuilder(), (sb, name) => sb.Length > 0 ? sb.Append(", ").Append(name) : sb.Append(name));

			Console.WriteLine("Birthdays after {0}:", date.ToString("yyyy"));
			Console.WriteLine(query);
		}

		[Test]
		public void AggregateWithUpperAndLowerFunctions()
		{
			var date = new DateTime(2007, 1, 1);

			var query = (from e in db.Employees
						 where e.BirthDate.Value.Month == date.Month
						 select new { First = e.FirstName.ToUpper(), Last = e.LastName.ToLower() })
				.Aggregate(new StringBuilder(), (sb, name) => sb.Length > 0 ? sb.Append(", ").Append(name) : sb.Append(name));

			Console.WriteLine("{0} Birthdays:", date.ToString("MMMM"));
			Console.WriteLine(query);
		}

		[Test]
		[Ignore("TODO: Custom functions")]
		public void AggregateWithCustomFunction()
		{
			/*
			var date = new DateTime(1960, 1, 1);

			var query = (from e in db.Employees
						 where e.BirthDate.Value.Year < date.Year
						 select db.Methods.fnEncrypt(e.FirstName))
				.Aggregate(new StringBuilder(), (sb, name) => sb.AppendLine(BitConverter.ToString(name)));

			Console.WriteLine(query);
			*/
		}
	}
}