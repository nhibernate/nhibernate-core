
using System;
using System.Linq;
using System.Linq.Expressions;

using NHibernate.Criterion;
using NHibernate.Impl;

using NUnit.Framework;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class TestExpressionProcessor
	{

		[Test]
		public void TestFindMemberExpressionReference()
		{
			Expression<Func<Person, string>> e = (Person p) => p.Name;
			string property = ExpressionProcessor.FindMemberExpression(e.Body);
			Assert.AreEqual("Name", property);
		}

		[Test]
		public void TestFindMemberExpressionValue()
		{
			Expression<Func<Person, object>> e = (Person p) => p.Age;
			string property = ExpressionProcessor.FindMemberExpression(e.Body);
			Assert.AreEqual("Age", property);
		}

		[Test]
		public void TestFindMemberExpressionSubCollectionIndex()
		{
			Expression<Func<Person, object>> e = (Person p) => p.PersonList[0].Children;
			string property = ExpressionProcessor.FindMemberExpression(e.Body);
			Assert.AreEqual("PersonList.Children", property);
		}

		[Test]
		public void TestFindMemberExpressionSubCollectionExtensionMethod()
		{
			Expression<Func<Person, object>> e = (Person p) => p.PersonList.First().Children;
			string property = ExpressionProcessor.FindMemberExpression(e.Body);
			Assert.AreEqual("PersonList.Children", property);
		}

		[Test]
		public void TestEvaluatePropertyExpression()
		{
			string testName = "testName";
			ICriterion criterion = ExpressionProcessor.ProcessExpression<Person>(p => p.Name == testName);
			SimpleExpression simpleExpression = (SimpleExpression)criterion;
			Assert.AreEqual("testName", simpleExpression.Value);
		}

		[Test]
		public void TestEvaluateNullPropertyExpression()
		{
			Person person = new Person() { Name = null };
			ICriterion criterion = ExpressionProcessor.ProcessExpression<Person>(p => p.Name == person.Name);
			SimpleExpression simpleExpression = (SimpleExpression)criterion;
			Assert.AreEqual(null, simpleExpression.Value);
		}

		[Test]
		public void TestEvaluateStaticPropertyExpression()
		{
			Person.StaticName = "test name";
			ICriterion criterion = ExpressionProcessor.ProcessExpression<Person>(p => p.Name == Person.StaticName);
			SimpleExpression simpleExpression = (SimpleExpression)criterion;
			Assert.AreEqual("test name", simpleExpression.Value);
		}

		[Test]
		public void TestEvaluateEnumeration()
		{
			ICriterion before = Restrictions.Eq("Gender", PersonGender.Female);
			ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.Gender == PersonGender.Female);
			Assert.AreEqual(before.ToString(), after.ToString());
		}

		[Test]
		public void TestEvaluateSubclass()
		{
			Person person = new CustomPerson();
			ICriterion before = Restrictions.Eq("Father", person);
			ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.Father == person);
			Assert.AreEqual(before.ToString(), after.ToString());
		}

		[Test]
		public void TestEvaluateMemberExpression()
		{
			Person testPerson = new Person();
			testPerson.Name = "testName";
			ICriterion criterion = ExpressionProcessor.ProcessExpression<Person>(p => p.Name == testPerson.Name);
			SimpleExpression simpleExpression = (SimpleExpression)criterion;
			Assert.AreEqual("testName", simpleExpression.Value);
		}

		[Test]
		public void TestEvaluateBooleanMemberExpression()
		{
			{
				ICriterion before = Restrictions.Eq("IsParent", true);
				ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.IsParent);
				Assert.AreEqual(before.ToString(), after.ToString());
			}
			{
				ICriterion before = Restrictions.Eq("IsParent", false);
				ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => !p.IsParent);
				Assert.AreEqual(before.ToString(), after.ToString());
			}
		}

	}

}
