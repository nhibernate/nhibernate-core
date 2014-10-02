using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class ExampleTests : LinqTestCase
	{
		[Test]
		public void CanQueryByExample()
		{
			//NH-3714
			using (this.session.BeginTransaction())
			{
				var customerExample = new Customer { CompanyName = "Alfreds Futterkiste", ContactTitle = "Sales Representative" };

				var result = this.session.Query<Customer>().ByExample(customerExample).ToList();

				Assert.IsTrue(result.All(x => x.CompanyName == customerExample.CompanyName && x.ContactTitle == customerExample.ContactTitle));
			}
		}

		[Test]
		public void CanQueryByExampleExcludingProperty()
		{
			//NH-3714
			using (this.session.BeginTransaction())
			{
				var customerExample = new Customer { CompanyName = "Alfreds Futterkiste", ContactTitle = "Sales Representative" };

				var result = this.session.Query<Customer>().ByExample(customerExample).Exclude(x => x.ContactTitle).ToList();

				Assert.IsTrue(result.All(x => x.CompanyName == customerExample.CompanyName));
			}
		}

		[Test]
		public void CanQueryByExampleIncludingComponent()
		{
			//NH-3714
			using (this.session.BeginTransaction())
			{
				var customerExample = new Customer { Address = new Address(string.Empty, "Berlin", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) };

				var result = this.session.Query<Customer>().ByExample(customerExample).ToList();

				Assert.IsTrue(result.All(x => x.CompanyName == customerExample.CompanyName));
			}
		}

		[Test]
		public void CanQueryByExampleIgnoringCase()
		{
			//NH-3714
			using (this.session.BeginTransaction())
			{
				var customerExample = new Customer { CompanyName = "alfreds futterkiste" };

				var result = this.session.Query<Customer>().ByExample(customerExample).SetMatchMode(MatchMode.IgnoreCase).ToList();

				Assert.IsTrue(result.All(x => x.CompanyName.Equals(customerExample.CompanyName, StringComparison.OrdinalIgnoreCase)));
			}
		}

		[Test]
		public void CanQueryByExampleUsingLikeAtStart()
		{
			//NH-3714
			using (this.session.BeginTransaction())
			{
				var customerExample = new Customer { CompanyName = "Alfred" };

				var result = this.session.Query<Customer>().ByExample(customerExample).SetMatchMode(MatchMode.Start).ToList();

				Assert.IsTrue(result.All(x => x.CompanyName.StartsWith(customerExample.CompanyName)));
			}
		}

		[Test]
		public void CanQueryByExampleUsingLikeAtEnd()
		{
			//NH-3714
			using (this.session.BeginTransaction())
			{
				var customerExample = new Customer { CompanyName = "Futterkiste" };

				var result = this.session.Query<Customer>().ByExample(customerExample).SetMatchMode(MatchMode.End).ToList();

				Assert.IsTrue(result.All(x => x.CompanyName.EndsWith(customerExample.CompanyName)));
			}
		}

		[Test]
		public void CanQueryByExampleUsingLikeInMiddle()
		{
			//NH-3714
			using (this.session.BeginTransaction())
			{
				var customerExample = new Customer { CompanyName = "fred" };

				var result = this.session.Query<Customer>().ByExample(customerExample).SetMatchMode(MatchMode.Anywhere).ToList();

				Assert.IsTrue(result.All(x => x.CompanyName.Contains(customerExample.CompanyName)));
			}
		}
	}
}
