﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using NHibernate.Util;
using NHibernate.Linq;

namespace NHibernate.Test.Linq
{
	using System.Threading.Tasks;

	[TestFixture]
	public class BooleanMethodExtensionExampleAsync : LinqTestCase
	{
		public class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
		{
			public MyLinqToHqlGeneratorsRegistry()
			{
				RegisterGenerator(ReflectHelper.GetMethodDefinition(() => BooleanLinqExtensions.FreeText(null, null)),
								  new FreetextGenerator());
			}
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();
		}

		[Test, Ignore("It work only with full-text indexes enabled.")]
		public async Task CanUseMyCustomExtensionAsync()
		{
			List<Customer> contacts = await ((from c in db.Customers where c.ContactName.FreeText("Thomas") select c).ToListAsync());
			Assert.That(contacts.Count, Is.GreaterThan(0));
			Assert.That(contacts.Select(c => c.ContactName).All(c => c.Contains("Thomas")), Is.True);
		}
	}
}