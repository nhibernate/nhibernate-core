﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NUnit.Framework;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;

namespace NHibernate.Test.Linq
{
	using System.Threading.Tasks;
	[TestFixture]
	public class RegresstionTestsAsync : LinqTestCase
	{
		/// <summary>
		/// http://aspzone.com/tech/nhibernate-linq-troubles/
		/// </summary>
		[Test]
		public async Task HierarchicalQueries_InlineConstantAsync()
		{
			var children = from s in db.Role
						   where s.ParentRole != null
						   select s;

			Assert.AreEqual(0, await (children.CountAsync()));

			var roots = from s in db.Role
						where s.ParentRole == null
						select s;

			Assert.AreEqual(2, await (roots.CountAsync()));
		}

		[Test]
		public async Task HierarchicalQueries_VariableAsync()
		{
			Role testRole = null;
			var children = from s in db.Role
						   where s.ParentRole != testRole
						   select s;

			Assert.AreEqual(0, await (children.CountAsync()));

			var roots = from s in db.Role
						where s.ParentRole == testRole
						select s;

			Assert.AreEqual(2, await (roots.CountAsync()));
		}

		[Test]
		public async Task CanUseNullConstantAndRestrictionAsync()
		{
			var roots = from s in db.Role
						where s.ParentRole == null
						&& s.Name == "Admin"
						select s;

			Assert.AreEqual(1, await (roots.CountAsync()));
		}
	}
}