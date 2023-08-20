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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.Criteria
{
	using System.Threading.Tasks;
	[TestFixture]
	public class ProjectionsTestAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get
			{
				return new string[]
					{
						"Criteria.Enrolment.hbm.xml",
						"Criteria.Animal.hbm.xml",
						"Criteria.MaterialResource.hbm.xml"
					};
			}
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();

				Student gavin = new Student();
				gavin.Name = "ayende";
				gavin.StudentNumber = 27;
				session.Save(gavin);

				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = Sfi.OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}

		[Test]
		public async Task UsingSqlFunctions_ConcatAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				string result = await (session.CreateCriteria(typeof(Student))
					.SetProjection(new SqlFunctionProjection("concat",
						NHibernateUtil.String,
						Projections.Property("Name"),
						new ConstantProjection(" "),
						Projections.Property("Name")
					))
					.UniqueResultAsync<string>());
				Assert.AreEqual("ayende ayende", result);
			}
		}

		[Test]
		public async Task UsingSqlFunctions_Concat_WithCastAsync()
		{
			if (Dialect is Oracle8iDialect)
				Assert.Ignore("Not supported by the active dialect:{0}.", Dialect);

			using (ISession session = Sfi.OpenSession())
			{
				string result = await (session.CreateCriteria(typeof(Student))
					.SetProjection(Projections.SqlFunction("concat",
						NHibernateUtil.String,
						Projections.Cast(NHibernateUtil.String, Projections.Id()),
						Projections.Constant(" "),
						Projections.Property("Name")
					))
					.UniqueResultAsync<string>());
				Assert.AreEqual("27 ayende", result);
			}
		}

		[Test]
		public async Task CastWithLengthAsync()
		{
			if (Regex.IsMatch(Dialect.GetCastTypeName(SqlTypeFactory.GetString(3)), @"^[^(]*$"))
			{
				Assert.Ignore($"Dialect {Dialect} does not seem to handle string length in cast");
			}

			using (var s = OpenSession())
			{
				try
				{
					var shortName = await (s
						.CreateCriteria<Student>()
						.SetProjection(
							Projections.Cast(
								TypeFactory.GetStringType(3),
								Projections.Property("Name")))
						.UniqueResultAsync<string>());
					Assert.That(shortName, Is.EqualTo("aye"));
				}
				catch (Exception e)
				{
					if (e.InnerException == null || !e.InnerException.Message.Contains("truncation"))
						throw;
				}
			}
		}

		[Test]
		public async Task CastWithPrecisionScaleAsync()
		{
			if (TestDialect.HasBrokenDecimalType)
				Assert.Ignore("Dialect does not correctly handle decimal.");

			using (var s = OpenSession())
			{
				var value = await (s
					.CreateCriteria<Student>()
					.SetProjection(
						Projections.Cast(
							TypeFactory.Basic("decimal(18,9)"),
							Projections.Constant(123456789.123456789m, TypeFactory.Basic("decimal(18,9)"))))
					.UniqueResultAsync<decimal>());
				Assert.That(value, Is.EqualTo(123456789.123456789m), "Same type cast");

				value = await (s
					.CreateCriteria<Student>()
					.SetProjection(
						Projections.Cast(
							TypeFactory.Basic("decimal(18,7)"),
							Projections.Constant(123456789.987654321m, TypeFactory.Basic("decimal(18,9)"))))
					.UniqueResultAsync<decimal>());
				Assert.That(value, Is.EqualTo(123456789.9876543m), "Reduced scale cast");
			}
		}

		[Test]
		public async Task CanUseParametersWithProjectionsAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				long result = await (session.CreateCriteria(typeof(Student))
					.SetProjection(new AddNumberProjection("id", 15))
					.UniqueResultAsync<long>());
				Assert.AreEqual(42L, result);
			}
		}

		[Test]
		public async Task UsingConditionalsAsync()
		{
			if (TestDialect.HasBrokenTypeInferenceOnSelectedParameters)
				Assert.Ignore("Current dialect does not support this test");

			using (ISession session = Sfi.OpenSession())
			{
				string result = await (session.CreateCriteria(typeof(Student))
					.SetProjection(
						Projections.Conditional(
							Expression.Eq("id", 27L),
							Projections.Constant("yes"),
							Projections.Constant("no"))
					)
					.UniqueResultAsync<string>());
				Assert.AreEqual("yes", result);

				result = await (session.CreateCriteria(typeof(Student))
					.SetProjection(
						Projections.Conditional(
							Expression.Eq("id", 42L),
							Projections.Constant("yes"),
							Projections.Constant("no"))
					)
					.UniqueResultAsync<string>());
				Assert.AreEqual("no", result);
			}
		}

		[Test]
		public async Task UseInWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.In(Projections.Id(), new object[] { 27 }))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseLikeWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.Like(Projections.Property("Name"), "aye", MatchMode.Start))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseInsensitiveLikeWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.InsensitiveLike(Projections.Property("Name"), "AYE", MatchMode.Start))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseIdEqWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.IdEq(Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseEqWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.Eq(Projections.Id(), 27L))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseGtWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.Gt(Projections.Id(), 2L))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseLtWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.Lt(Projections.Id(), 200L))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseLeWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.Le(Projections.Id(), 27L))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseGeWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.Ge(Projections.Id(), 27L))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseBetweenWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.Between(Projections.Id(), 10L, 28L))
					.ListAsync<Student>());
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public async Task UseIsNullWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.IsNull(Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public async Task UseIsNotNullWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.IsNotNull(Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public async Task UseEqPropertyWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.EqProperty(Projections.Id(), Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public async Task UseGePropertyWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.GeProperty(Projections.Id(), Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public async Task UseGtPropertyWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.GtProperty(Projections.Id(), Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public async Task UseLtPropertyWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.LtProperty(Projections.Id(), Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public async Task UseLePropertyWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.LeProperty(Projections.Id(), Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public async Task UseNotEqPropertyWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = await (session.CreateCriteria(typeof(Student))
					.Add(Expression.NotEqProperty("id", Projections.Id()))
					.ListAsync<Student>());
				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public async Task UseSumWithNullResultWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				long sum = await (session.CreateCriteria(typeof(Reptile))
					.SetProjection(Projections.Sum(Projections.Id()))
					.UniqueResultAsync<long>());
				Assert.AreEqual(0, sum);
			}
		}

		[Test]
		public async Task UseSubquerySumWithNullResultWithProjectionAsync()
		{
			using (ISession session = Sfi.OpenSession())
			{
				int sum = await (session.CreateCriteria(typeof(Enrolment))
					.CreateCriteria("Student", "s")
					.SetProjection(Projections.Sum(Projections.SqlFunction("length", NHibernateUtil.Int32, Projections.Property("s.Name"))))
					.UniqueResultAsync<int>());
				Assert.AreEqual(0, sum);
			}
		}
	}
}
