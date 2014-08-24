
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Criterion;
using NHibernate.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class ExpressionProcessorFixture
	{

		[Test]
		public void TestFindMemberExpressionReference()
		{
			Expression<Func<Person, string>> e = (Person p) => p.Name;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("Name", property);
		}

		[Test]
		public void TestFindMemberExpressionReferenceCast()
		{
			Expression<Func<Person, string>> e = (Person p) => ((CustomPerson)p).MiddleName;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("MiddleName", property);
		}

		[Test]
		public void TestFindMemberExpressionReferenceAlias()
		{
			Person personAlias = null;
			Expression<Func<string>> e = () => personAlias.Name;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.Name", property);
		}

		[Test]
		public void TestFindMemberExpressionReferenceCastAlias()
		{
			Person personAlias = null;
			Expression<Func<string>> e = () => ((CustomPerson)personAlias).MiddleName;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.MiddleName", property);
		}

		[Test]
		public void TestFindMemberExpressionComponent()
		{
			Expression<Func<Person, string>> e = (Person p) => p.Father.Name;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("Father.Name", property);
		}

		[Test]
		public void TestFindMemberExpressionComponentAlias()
		{
			Person personAlias = null;
			Expression<Func<string>> e = () => personAlias.Father.Name;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.Father.Name", property);
		}

		[Test]
		public void TestFindMemberExpressionValue()
		{
			Expression<Func<Person, object>> e = (Person p) => p.Age;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("Age", property);
		}

		[Test]
		public void TestFindMemberExpressionValueAlias()
		{
			Person personAlias = null;
			Expression<Func<object>> e = () => personAlias.Age;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.Age", property);
		}

		[Test]
		public void TestFindMemberExpressionSubCollectionIndex()
		{
			Expression<Func<Person, object>> e = (Person p) => p.PersonList[0].Children;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("PersonList.Children", property);
		}

		[Test]
		public void TestFindMemberExpressionSubCollectionIndexAlias()
		{
			Person personAlias = null;
			Expression<Func<object>> e = () => personAlias.PersonList[0].Children;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.PersonList.Children", property);
		}

		[Test]
		public void TestFindMemberExpressionSubCollectionFirst()
		{
			Expression<Func<Person, object>> e = (Person p) => p.PersonList.First().Children;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("PersonList.Children", property);
		}

		[Test]
		public void TestFindMemberExpressionSubCollectionFirstAlias()
		{
			Person personAlias = null;
			Expression<Func<object>> e = () => personAlias.PersonList.First().Children;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.PersonList.Children", property);
		}

		[Test]
		public void TestFindMemberExpressionSubCollectionExtensionMethod()
		{
			Expression<Func<Person, object>> e = (Person p) => p.PersonList.First().Children;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("PersonList.Children", property);
		}

		[Test]
		public void TestFindMemberExpressionSubCollectionExtensionMethodAlias()
		{
			Person personAlias = null;
			Expression<Func<object>> e = () => personAlias.PersonList.First().Children;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.PersonList.Children", property);
		}

		[Test]
		public void TestFindMemberExpressionClass()
		{
			Expression<Func<Person, object>> e = (Person p) => p.GetType();
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("class", property);
		}

		[Test]
		public void TestFindMemberExpressionClassAlias()
		{
			Person personAlias = null;
			Expression<Func<object>> e = () => personAlias.GetType();
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.class", property);
		}

		[Test]
		public void TestFindMemberExpressionNullableValue()
		{
			Expression<Func<Person, object>> e = (Person p) => p.NullableGender.Value;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("NullableGender", property);
		}

		[Test]
		public void TestFindMemberExpressionNullableValueAlias()
		{
			Person personAlias = null;
			Expression<Func<object>> e = () => personAlias.NullableGender.Value;
			string property = ExpressionProcessor.FindMemberProjection(e.Body).AsProperty();
			Assert.AreEqual("personAlias.NullableGender", property);
		}

		[Test]
		public void TestFindMemberExpressionConstants()
		{
			var children = new List<Child> { new Child { Nickname = "test nickname" } };
			Person person =
				new CustomPerson()
				{
					Name = "test name",
					MiddleName = "test middle name",
					NullableAge = 4,
					Children = children,
				};

			Assert.That(Projection(() => person.Name), Is.EqualTo("test name"));
			Assert.That(Projection(() => ((CustomPerson)person).MiddleName), Is.EqualTo("test middle name"));
			Assert.That(Projection(() => "test name"), Is.EqualTo("test name"));
			Assert.That(Projection(() => person.NullableAge.Value), Is.EqualTo(4));
			Assert.That(Projection(() => person.GetType()), Is.EqualTo(typeof(CustomPerson)));
			Assert.That(Projection(() => person.Children.First().Nickname), Is.EqualTo("test nickname"));
			Assert.That(Projection(() => children[0].Nickname), Is.EqualTo("test nickname"));
		}

		private T Projection<T>(Expression<Func<T>> e)
		{
			var constantProjection = ExpressionProcessor.FindMemberProjection(e.Body).AsProjection();
			return (T)typeof(ConstantProjection).GetField("value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(constantProjection);
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
			Assert.That(criterion, Is.InstanceOf<NullExpression>());
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
		public void TestUnaryConversionChecked()
		{
			checked
			{
				ICriterion before = Restrictions.Eq("Gender", PersonGender.Female);
				ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.Gender == PersonGender.Female);
				Assert.AreEqual(before.ToString(), after.ToString());
			}
		}

		[Test]
		public void TestUnaryConversionUnchecked()
		{
			unchecked
			{
				ICriterion before = Restrictions.Eq("Gender", PersonGender.Female);
				ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.Gender == PersonGender.Female);
				Assert.AreEqual(before.ToString(), after.ToString());
			}
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

		[Test]
		public void TestEvaluateNullableIntExpression()
		{
			ICriterion before = Restrictions.Eq("NullableAge", 5);
			ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.NullableAge == 5);
			Assert.AreEqual(before.ToString(), after.ToString());
		}

		[Test]
		public void TestEvaluateNullableEnumExpression()
		{
			ICriterion before = Restrictions.Eq("NullableGender", PersonGender.Female);
			ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.NullableGender == PersonGender.Female);
			Assert.AreEqual(before.ToString(), after.ToString());
		}

		[Test]
		public void TestEvaluateNullableEnumValueExpression()
		{
			ICriterion before = Restrictions.Eq("NullableGender", PersonGender.Female);
			ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.NullableGender.Value == PersonGender.Female);
			Assert.AreEqual(before.ToString(), after.ToString());
		}

		[Test]
		public void TestEvaluateNullableBoolExpression()
		{
			ICriterion before = Restrictions.Eq("NullableIsParent", true);
			ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.NullableIsParent.Value);
			Assert.AreEqual(before.ToString(), after.ToString());
		}

		[Test]
		public void TestEvaluateRestrictionExtension()
		{
			ICriterion before = Restrictions.Like("Name", "%test%");
			ICriterion after = ExpressionProcessor.ProcessExpression<Person>(p => p.Name.IsLike("%test%"));
			Assert.AreEqual(before.ToString(), after.ToString());
		}

		public void NonGenericMethod(string param1) { }
		public T GenericMethod<T>(T param1) { return param1; }

		[Test]
		public void TestSignatureNonGeneric()
		{
			MethodInfo thisMethod = GetType().GetMethod("NonGenericMethod");

			ExpressionProcessor.Signature(thisMethod).Should().Be("NHibernate.Test.Criteria.Lambda.ExpressionProcessorFixture:Void NonGenericMethod(System.String)");
		}

		[Test]
		public void TestSignatureGeneric()
		{
			MethodInfo thisMethod = GetType().GetMethod("GenericMethod");

			ExpressionProcessor.Signature(thisMethod).Should().Be("NHibernate.Test.Criteria.Lambda.ExpressionProcessorFixture:T GenericMethod[T](T)");
		}

		[Test]
		public void TestSignatureQualifiedGeneric()
		{
			Expression<Func<string>> expression = () => this.GenericMethod("test");
			MethodInfo genericMethodWithQualifiedType = (expression.Body as MethodCallExpression).Method;

			ExpressionProcessor.Signature(genericMethodWithQualifiedType).Should().Be("NHibernate.Test.Criteria.Lambda.ExpressionProcessorFixture:T GenericMethod[T](T)");
		}

	}

}
