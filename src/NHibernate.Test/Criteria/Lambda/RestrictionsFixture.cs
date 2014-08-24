using System;
using System.Collections;

using NUnit.Framework;

using NHibernate.Criterion;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class RestrictionsFixture : LambdaFixtureBase
	{

		[Test]
		public void ArbitraryCriterion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Lt("Age", 65))
					.Add(Restrictions.Ge("personAlias.Age", 18))
					.Add(Restrictions.Not(Restrictions.Ge("Age", 65)))
					.Add(Restrictions.Not(Restrictions.Lt("personAlias.Age", 18)));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(Restrictions.Where<Person>(p => p.Age < 65))
					.And(Restrictions.Where(() => personAlias.Age >= 18))
					.And(Restrictions.WhereNot<Person>(p => p.Age >= 65))
					.And(Restrictions.WhereNot(() => personAlias.Age < 18));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SqlOperators()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Between("Age", 18, 65))
					.Add(Restrictions.Between("personAlias.Age", 18, 65))
					.Add(Restrictions.Not(Restrictions.Between("personAlias.Age", 10, 20)))
					.Add(!Restrictions.In("Name", new string[] { "name4" }))
					.Add(Restrictions.In("Name", new string[] { "name1", "name2", "name3" }))
					.Add(Restrictions.In("Name", new ArrayList() { "name1", "name2", "name3" }))
					.Add(Restrictions.InG<int>("Age", new int[] { 1, 2, 3 }))
					.Add(Restrictions.InsensitiveLike("Name", "test"))
					.Add(Restrictions.InsensitiveLike("Name", "tEsT", MatchMode.Anywhere))
					.Add(Restrictions.IsEmpty("Children"))
					.Add(Restrictions.Not(Restrictions.IsEmpty("Children")))
					.Add(Restrictions.IsNotEmpty("Children"))
					.Add(Restrictions.IsNotNull("Name"))
					.Add(Restrictions.IsNull("Name"))
					.Add(Restrictions.Like("Name", "%test%"))
					.Add(Restrictions.Like("Name", "test", MatchMode.Anywhere))
					.Add(Restrictions.Like("Name", "test", MatchMode.Anywhere, '?'))
					.Add(Restrictions.NaturalId()
							.Set("Name", "my name")
							.Set("personAlias.Age", 18));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(Restrictions.On<Person>(p => p.Age).IsBetween(18).And(65))
					.And(Restrictions.On(() => personAlias.Age).IsBetween(18).And(65))
					.And(Restrictions.On(() => personAlias.Age).Not.IsBetween(10).And(20))
					.And(!Restrictions.On<Person>(p => p.Name).IsIn(new string[] { "name4" }))
					.And(Restrictions.On<Person>(p => p.Name).IsIn(new string[] { "name1", "name2", "name3" }))
					.And(Restrictions.On<Person>(p => p.Name).IsIn(new ArrayList() { "name1", "name2", "name3" }))
					.And(Restrictions.On<Person>(p => p.Age).IsInG<int>(new int[] { 1, 2, 3 }))
					.And(Restrictions.On<Person>(p => p.Name).IsInsensitiveLike("test"))
					.And(Restrictions.On<Person>(p => p.Name).IsInsensitiveLike("tEsT", MatchMode.Anywhere))
					.And(Restrictions.On<Person>(p => p.Children).IsEmpty)
					.And(Restrictions.On<Person>(p => p.Children).Not.IsEmpty)
					.And(Restrictions.On<Person>(p => p.Children).IsNotEmpty)
					.And(Restrictions.On<Person>(p => p.Name).IsNotNull)
					.And(Restrictions.On<Person>(p => p.Name).IsNull)
					.And(Restrictions.On<Person>(p => p.Name).IsLike("%test%"))
					.And(Restrictions.On<Person>(p => p.Name).IsLike("test", MatchMode.Anywhere))
					.And(Restrictions.On<Person>(p => p.Name).IsLike("test", MatchMode.Anywhere, '?'))
					.And(Restrictions.NaturalId()
							.Set<Person>(p => p.Name).Is("my name")
							.Set(() => personAlias.Age).Is(18));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Junction()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Conjunction()
							.Add(Restrictions.Eq("Name", "test"))
							.Add(Restrictions.Eq("personAlias.Name", "test")))
					.Add(Restrictions.Disjunction()
							.Add(Restrictions.Eq("Name", "test"))
							.Add(Restrictions.Eq("personAlias.Name", "test")));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(Restrictions.Conjunction()
							.Add<Person>(p => p.Name == "test")
							.Add(() => personAlias.Name == "test"))
					.And(Restrictions.Disjunction()
							.Add<Person>(p => p.Name == "test")
							.Add(() => personAlias.Name == "test"));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SqlOperatorsInline()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Between("Age", 18, 65))
					.Add(Restrictions.Between("personAlias.Age", 18, 65))
					.Add(Restrictions.Not(Restrictions.Between("Age", 18, 65)))
					.Add(Restrictions.In("Name", new string[] { "name1", "name2", "name3" }))
					.Add(Restrictions.In("personAlias.Name", new ArrayList() { "name1", "name2", "name3" }))
					.Add(Restrictions.InG<int>("Age", new int[] { 1, 2, 3 }))
					.Add(Restrictions.InsensitiveLike("Name", "test"))
					.Add(Restrictions.InsensitiveLike("Name", "tEsT", MatchMode.Anywhere))
					.Add(Restrictions.IsEmpty("Children"))
					.Add(Restrictions.IsNotEmpty("Children"))
					.Add(Restrictions.IsNotNull("Name"))
					.Add(Restrictions.IsNull("Name"))
					.Add(Restrictions.Like("Name", "%test%"))
					.Add(Restrictions.Like("Name", "test", MatchMode.Anywhere))
					.Add(Restrictions.Like("Name", "test", MatchMode.Anywhere, '?'))
					.Add(Restrictions.Not(Restrictions.Like("Name", "%test%")));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.WhereRestrictionOn(p => p.Age).IsBetween(18).And(65)
					.WhereRestrictionOn(() => personAlias.Age).IsBetween(18).And(65)
					.WhereRestrictionOn(p => p.Age).Not.IsBetween(18).And(65)
					.AndRestrictionOn(p => p.Name).IsIn(new string[] { "name1", "name2", "name3" })
					.AndRestrictionOn(() => personAlias.Name).IsIn(new ArrayList() { "name1", "name2", "name3" })
					.AndRestrictionOn(p => p.Age).IsInG<int>(new int[] { 1, 2, 3 })
					.AndRestrictionOn(p => p.Name).IsInsensitiveLike("test")
					.AndRestrictionOn(p => p.Name).IsInsensitiveLike("tEsT", MatchMode.Anywhere)
					.AndRestrictionOn(p => p.Children).IsEmpty
					.AndRestrictionOn(p => p.Children).IsNotEmpty
					.AndRestrictionOn(p => p.Name).IsNotNull
					.AndRestrictionOn(p => p.Name).IsNull
					.AndRestrictionOn(p => p.Name).IsLike("%test%")
					.AndRestrictionOn(p => p.Name).IsLike("test", MatchMode.Anywhere)
					.AndRestrictionOn(p => p.Name).IsLike("test", MatchMode.Anywhere, '?')
					.AndRestrictionOn(p => p.Name).Not.IsLike("%test%");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void DetachedRestrictions()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>("personAlias")
					.Add(Restrictions.Between("Age", 18, 65))
					.Add(Restrictions.Between("personAlias.Age", 18, 65))
					.Add(Restrictions.In("Name", new string[] { "name1", "name2", "name3" }))
					.Add(Restrictions.In("personAlias.Name", new ArrayList() { "name1", "name2", "name3" }));

			Person personAlias = null;
			QueryOver<Person> actual =
				QueryOver.Of<Person>(() => personAlias)
					.WhereRestrictionOn(p => p.Age).IsBetween(18).And(65)
					.WhereRestrictionOn(() => personAlias.Age).IsBetween(18).And(65)
					.AndRestrictionOn(p => p.Name).IsIn(new string[] { "name1", "name2", "name3" })
					.AndRestrictionOn(() => personAlias.Name).IsIn(new ArrayList() { "name1", "name2", "name3" });

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void NullRestriction()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.IsNull("Name"))
					.Add(Restrictions.IsNull("Name"))
					.Add(Restrictions.IsNull("Name"))
					.Add(Restrictions.IsNull("Father"))
					.Add(Restrictions.IsNull("Father"))
					.Add(Restrictions.IsNull("NullableGender"))
					.Add(Restrictions.IsNull("NullableAge"))
					.Add(Restrictions.IsNull("NullableIsParent"))
					.Add(Restrictions.Not(Restrictions.IsNull("Name")))
					.Add(Restrictions.Not(Restrictions.IsNull("Name")))
					.Add(Restrictions.Not(Restrictions.IsNull("Name")))
					.Add(Restrictions.Not(Restrictions.IsNull("Father")))
					.Add(Restrictions.Not(Restrictions.IsNull("Father")))
					.Add(Restrictions.Not(Restrictions.IsNull("NullableGender")))
					.Add(Restrictions.Not(Restrictions.IsNull("NullableAge")))
					.Add(Restrictions.Not(Restrictions.IsNull("NullableIsParent")))
					.Add(Restrictions.IsNull("personAlias.Name"));

			Person personAlias = null;
			CustomPerson nullPerson = null;
			Person.StaticName = null;
			Person emptyPerson = new Person() { Name = null };
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(p => p.Name == null)
					.Where(p => p.Name == Person.StaticName)
					.Where(p => p.Name == emptyPerson.Name)
					.Where(p => p.Father == null)
					.Where(p => p.Father == nullPerson)
					.Where(p => p.NullableGender == null)
					.Where(p => p.NullableAge == null)
					.Where(p => p.NullableIsParent == null)
					.Where(p => p.Name != null)
					.Where(p => p.Name != Person.StaticName)
					.Where(p => p.Name != emptyPerson.Name)
					.Where(p => p.Father != null)
					.Where(p => p.Father != nullPerson)
					.Where(p => p.NullableGender != null)
					.Where(p => p.NullableAge != null)
					.Where(p => p.NullableIsParent != null)
					.Where(() => personAlias.Name == null);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void RestrictionsExtensions()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.Like("Name", "%test%"))
					.Add(Restrictions.Like("Name", "test", MatchMode.End))
					.Add(Restrictions.Like("Name", "test", MatchMode.Start, '?'))
					.Add(Restrictions.InsensitiveLike("Name", "%test%"))
					.Add(Restrictions.InsensitiveLike("Name", "test", MatchMode.Anywhere))
					.Add(Restrictions.In("Name", new string[] { "name1", "name2" }))
					.Add(Restrictions.In("Name", new ArrayList() { "name3", "name4" }))
					.Add(Restrictions.Between("Age", 10, 20));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Where(p => p.Name.IsLike("%test%"))
					.And(p => p.Name.IsLike("test", MatchMode.End))
					.And(p => p.Name.IsLike("test", MatchMode.Start, '?'))
					.And(p => p.Name.IsInsensitiveLike("%test%"))
					.And(p => p.Name.IsInsensitiveLike("test", MatchMode.Anywhere))
					.And(p => p.Name.IsIn(new string[] { "name1", "name2" }))
					.And(p => p.Name.IsIn(new ArrayList() { "name3", "name4" }))
					.And(p => p.Age.IsBetween(10).And(20));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void FunctionExtensions()
		{
			var date = new DateTime(1970, 1, 1);

			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.Eq(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("BirthDate")), 1970))
					.Add(Restrictions.Eq(Projections.SqlFunction("day", NHibernateUtil.Int32, Projections.Property("BirthDate")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("month", NHibernateUtil.Int32, Projections.Property("BirthDate")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("hour", NHibernateUtil.Int32, Projections.Property("BirthDate")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("minute", NHibernateUtil.Int32, Projections.Property("BirthDate")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("second", NHibernateUtil.Int32, Projections.Property("BirthDate")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("date", NHibernateUtil.Date, Projections.Property("BirthDate")), date))
					.Add(Restrictions.Eq(Projections.SqlFunction("date", NHibernateUtil.Date, Projections.Property("BirthDateAsDateTimeOffset")), date))
					.Add(Restrictions.Eq(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("BirthDateAsDateTimeOffset")), 1970))
					.Add(Restrictions.Eq(Projections.SqlFunction("day", NHibernateUtil.Int32, Projections.Property("BirthDateAsDateTimeOffset")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("month", NHibernateUtil.Int32, Projections.Property("BirthDateAsDateTimeOffset")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("hour", NHibernateUtil.Int32, Projections.Property("BirthDateAsDateTimeOffset")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("minute", NHibernateUtil.Int32, Projections.Property("BirthDateAsDateTimeOffset")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("second", NHibernateUtil.Int32, Projections.Property("BirthDateAsDateTimeOffset")), 1))
					.Add(Restrictions.Eq(Projections.SqlFunction("sqrt", NHibernateUtil.Double, Projections.Property("Height")), 10d))
					.Add(Restrictions.Eq(Projections.SqlFunction("lower", NHibernateUtil.String, Projections.Property("Name")), "test"))
					.Add(Restrictions.Eq(Projections.SqlFunction("upper", NHibernateUtil.String, Projections.Property("Name")), "TEST"))
					.Add(Restrictions.Eq(Projections.SqlFunction("abs", NHibernateUtil.Int32, Projections.Property("Height")), 150))
					.Add(Restrictions.Eq(Projections.SqlFunction("trim", NHibernateUtil.String, Projections.Property("Name")), "test"))
					.Add(Restrictions.Eq(Projections.SqlFunction("length", NHibernateUtil.String, Projections.Property("Name")), 4))
					.Add(Restrictions.Eq(Projections.SqlFunction("bit_length", NHibernateUtil.String, Projections.Property("Name")), 32))
					.Add(Restrictions.Eq(Projections.SqlFunction("substring", NHibernateUtil.String, Projections.Property("Name"), Projections.Constant(1), Projections.Constant(2)), "te"))
					.Add(Restrictions.Eq(Projections.SqlFunction("locate", NHibernateUtil.String, Projections.Constant("e"), Projections.Property("Name"), Projections.Constant(1)), 2))
					.Add(Restrictions.Eq(Projections.SqlFunction("coalesce", NHibernateUtil.Object, Projections.Property("Name"), Projections.Constant("not-null-val")), "test"))
					.Add(Restrictions.Eq(Projections.SqlFunction("coalesce", NHibernateUtil.Object, Projections.Property("NullableIsParent"), Projections.Constant(true)), true))
					.Add(Restrictions.Eq(Projections.SqlFunction("concat", NHibernateUtil.String, Projections.Property("Name"), Projections.Constant(", "), Projections.Property("Name")), "test, test"))
					.Add(Restrictions.Eq(Projections.SqlFunction("mod", NHibernateUtil.Int32, Projections.Property("Height"), Projections.Constant(10)), 0));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Where(p => p.BirthDate.Year == 1970)
					.And(p => p.BirthDate.Day == 1)
					.And(p => p.BirthDate.Month == 1)
					.And(p => p.BirthDate.Hour == 1)
					.And(p => p.BirthDate.Minute == 1)
					.And(p => p.BirthDate.Second == 1)
					.And(p => p.BirthDate.Date == date)
					.And(p => p.BirthDateAsDateTimeOffset.Date == date)
					.And(p => p.BirthDateAsDateTimeOffset.Year == 1970)
					.And(p => p.BirthDateAsDateTimeOffset.Day == 1)
					.And(p => p.BirthDateAsDateTimeOffset.Month == 1)
					.And(p => p.BirthDateAsDateTimeOffset.Hour == 1)
					.And(p => p.BirthDateAsDateTimeOffset.Minute == 1)
					.And(p => p.BirthDateAsDateTimeOffset.Second == 1)
					.And(p => p.Height.Sqrt() == 10)
					.And(p => p.Name.Lower() == "test")
					.And(p => p.Name.Upper() == "TEST")
					.And(p => p.Height.Abs() == 150)
					.And(p => p.Name.TrimStr() == "test")
					.And(p => p.Name.StrLength() == 4)
					.And(p => p.Name.BitLength() == 32)
					.And(p => p.Name.Substr(1, 2) == "te")
					.And(p => p.Name.CharIndex("e", 1) == 2)
					.And(p => p.Name.Coalesce("not-null-val") == "test")
					.And(p => p.NullableIsParent.Coalesce(true) == true)
					.And(p => Projections.Concat(p.Name, ", ", p.Name) == "test, test")
					.And(p => p.Height.Mod(10) == 0);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void FunctionExtensionsProperty()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.EqProperty(
						Projections.SqlFunction("month", NHibernateUtil.Int32, Projections.Property("BirthDate")),
						Projections.SqlFunction("day", NHibernateUtil.Int32, Projections.Property("BirthDate"))));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Where(p => p.BirthDate.Month == p.BirthDate.Day);

			AssertCriteriaAreEqual(expected, actual);
		}
	}
}