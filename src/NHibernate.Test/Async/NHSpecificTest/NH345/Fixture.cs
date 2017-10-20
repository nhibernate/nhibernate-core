﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH345
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH345"; }
		}

		[Test]
		public async Task OrderByCompositePropertyAsync()
		{
			if (Dialect is MsSql2000Dialect)
			{
				Assert.Ignore("This test fails on MS SQL 2000 because of SQL Server bug");
			}

			using (ISession s = OpenSession())
			{
				Client client1 = new Client();
				client1.Name = "Client A";

				Client client2 = new Client();
				client2.Name = "Client B";

				Project project1 = new Project();
				project1.Client = client1;

				Project project2 = new Project();
				project2.Client = client2;

				await (s.SaveAsync(client1));
				await (s.SaveAsync(client2));
				await (s.SaveAsync(project1));
				await (s.SaveAsync(project2));
				await (s.FlushAsync());

				IList listAsc = await (s.CreateQuery(
					"select p from Project as p order by p.Client.Name asc").ListAsync());

				Assert.AreEqual(2, listAsc.Count);
				Assert.AreSame(project1, listAsc[0]);
				Assert.AreSame(project2, listAsc[1]);

				IList listDesc = await (s.CreateQuery(
					"select p from Project as p order by p.Client.Name desc").ListAsync());
				Assert.AreEqual(2, listDesc.Count);
				Assert.AreSame(project1, listDesc[1]);
				Assert.AreSame(project2, listDesc[0]);

				await (s.DeleteAsync("from Project"));
				await (s.DeleteAsync("from Client"));
				await (s.FlushAsync());
			}
		}
	}
}