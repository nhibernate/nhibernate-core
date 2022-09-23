﻿using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class MethodCallTests : LinqTestCase
	{
		[Test]
		public void CanExecuteAny()
		{
			var result = db.Users.Any();
			Assert.IsTrue(result);
		}

		[Test]
		public void CanExecuteAnyWithArguments()
		{
			var result = db.Users.Any(u => u.Name == "user-does-not-exist");
			Assert.IsFalse(result);
		}

		[Test]
		public void CanExecuteContains()
		{
			var user = db.Users.FirstOrDefault();
			var result = db.Users.Contains(user);
			Assert.That(result, Is.True);

			user = new User("test", DateTime.Now);
			result = db.Users.Contains(user);
			Assert.That(result, Is.False);
		}

		[Test]
		public void CanExecuteCountWithOrderByArguments()
		{
			var query = db.Users.OrderBy(u => u.Name);
			var count = query.Count();
			Assert.AreEqual(3, count);
		}

		[Test, Description("NH-2744")]
		public void CanSelectPropertiesIntoObjectArray()
		{
			var result = db.Users
				.Select(u => new object[] {u.Id, u.Name, u.InvalidLoginAttempts})
				.First();

			Assert.That(result.Length, Is.EqualTo(3));
			Assert.That(result[1], Is.EqualTo("ayende"));
			Assert.That(result[2], Is.EqualTo(4));
		}

		[Test, Description("NH-2744")]
		public void CanSelectComponentsIntoObjectArray()
		{
			var result = db.Users
				.Select(u => new object[] {u.Component, u.Component.OtherComponent})
				.First();

			Assert.That(result.Length, Is.EqualTo(2));
			Assert.That(result[0], Is.TypeOf<UserComponent>());
			Assert.That(result[1], Is.TypeOf<UserComponent2>());

			var component = (UserComponent)result[0];
			Assert.That(component.Property1, Is.EqualTo("test1"));

			var otherComponent = (UserComponent2) result[1];
			Assert.That(otherComponent.OtherProperty1, Is.EqualTo("othertest1"));
		}

		[Test, Description("NH-2744")]
		public void CanSelectEnumerationPropertiesIntoObjectArray()
		{
			var result = db.Users
				.Where(u => u.Name == "nhibernate")
				.Select(u => new object[] {u.Enum1, u.Enum2, u.Features})
				.First();

			Assert.That(result.Length, Is.EqualTo(3));
			Assert.That(result[0], Is.EqualTo(EnumStoredAsString.Medium));
			Assert.That(result[1], Is.EqualTo(EnumStoredAsInt32.Unspecified));
			Assert.That(result[2], Is.EqualTo(FeatureSet.HasAll));
		}

		[Test, Description("NH-2744")]
		public void CanSelectConstantsIntoObjectArray()
		{
			const decimal pi = 3.1415m;
			const string name = "Julian";

			var result = db.Users
				.Select(u => new object[] {u.Id, pi, name, DateTime.MinValue})
				.First();

			Assert.That(result.Length, Is.EqualTo(4));
			Assert.That(result[1], Is.EqualTo(pi));
			Assert.That(result[2], Is.EqualTo(name));
			Assert.That(result[3], Is.EqualTo(DateTime.MinValue));
		}

		[Test, Description("NH-2744")] 
		public void CanSelectPropertiesFromAssociationsIntoObjectArray()
		{
			var result = db.Users
				.Select(u => new object[] {u.Id, u.Role.Name, u.Role.Entity.Output})
				.First();

			Assert.That(result.Length, Is.EqualTo(3));
			Assert.That(result[1], Is.EqualTo("Admin"));
			Assert.That(result[2], Is.EqualTo("output"));
		}

		[Test(Description = "NH-2782")] 
		public void CanSelectPropertiesIntoObjectArrayInProperty()
		{
			var result = db.Users
				.Select(u => new { Cells = new object[] { u.Id, u.Name, new object[u.Id] } })
				.First();

			var cells = result.Cells;
			Assert.That(cells.Length, Is.EqualTo(3));
			Assert.That(cells[1], Is.EqualTo("ayende"));
			Assert.That(cells[2], Is.InstanceOf<object[]>().And.Length.EqualTo(cells[0]));
		}

		[Test(Description = "NH-2782")] 
		public void CanSelectPropertiesIntoPropertyListInProperty()
		{
			var result = db.Users
				.Select(u => new { Cells = new List<object> { u.Id, u.Name, new object[u.Id] } })
				.First();

			var cells = result.Cells;
			Assert.That(cells.Count, Is.EqualTo(3));
			Assert.That(cells[1], Is.EqualTo("ayende"));
			Assert.That(cells[2], Is.InstanceOf<object[]>().And.Length.EqualTo(cells[0]));
		} 

		[Test, Description("NH-2744")]
		public void CanSelectPropertiesIntoNestedObjectArrays()
		{
			var query = db.Users.Select(u => new object[] {"Root", new object[] {"Sub1", u.Name, new object[] {"Sub2", u.Name}}});
			var result = query.First();

			Assert.That(result.Length, Is.EqualTo(2));
			Assert.That(result[0], Is.EqualTo("Root"));
			Assert.That(result[1], Is.TypeOf<object[]>());

			var nestedObjectArray = (object[]) result[1];
			Assert.That(nestedObjectArray.Length, Is.EqualTo(3));
			Assert.That(nestedObjectArray[0], Is.EqualTo("Sub1"));
			Assert.That(nestedObjectArray[1], Is.EqualTo("ayende"));
			Assert.That(nestedObjectArray[2], Is.TypeOf<object[]>());

			var nestedNestedObjectArray = (object[]) nestedObjectArray[2];
			Assert.That(nestedNestedObjectArray.Length, Is.EqualTo(2));
			Assert.That(nestedNestedObjectArray[0], Is.EqualTo("Sub2"));
			Assert.That(nestedNestedObjectArray[1], Is.EqualTo("ayende"));
		}
	}
}
