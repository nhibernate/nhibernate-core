﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;

namespace NHibernate.Test.Any
{
	using System.Threading.Tasks;
	[TestFixture]
	public class AnyTypeTestAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] {"Any.Person.hbm.xml"}; }
		}

		[Test]
		public async Task FlushProcessingAsync()
		{
			var person = new Person();
			var address = new Address();
			//http://opensource.atlassian.com/projects/hibernate/browse/HHH-1663
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				person.Data = address;
				await (session.SaveOrUpdateAsync(person));
				await (session.SaveOrUpdateAsync(address));
				await (tran.CommitAsync());
				session.Close();
			}

			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				person = (Person) await (session.LoadAsync(typeof(Person), person.Id));
				person.Name = "makingpersondirty";
				await (tran.CommitAsync());
				session.Close();
			}

			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				await (session.DeleteAsync(person));
				await (session.DeleteAsync(address));
				await (tran.CommitAsync());
				session.Close();
			}
		}
	}
}
