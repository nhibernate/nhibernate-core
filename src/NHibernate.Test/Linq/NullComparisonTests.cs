using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class NullComparisonTests : LinqTestCase
	{
		private static readonly AnotherEntity OutputSet = new AnotherEntity {Output = "output"};
		private static readonly AnotherEntity InputSet = new AnotherEntity {Input = "input"};
		private static readonly AnotherEntity BothSame = new AnotherEntity {Input = "i/o", Output = "i/o"};
		private static readonly AnotherEntity BothNull = new AnotherEntity();
		private static readonly AnotherEntity BothDifferent = new AnotherEntity {Input = "input", Output = "output"};

		[Test]
		public void NullInequalityWithNotNull()
		{
			var q = session.Query<AnotherEntityRequired>().Where(o => o.Input != null);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, InputSet, BothSame, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => null != o.Input);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, InputSet, BothSame, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => o.InputNullability != AnotherEntityNullability.True);
			Expect(q, Does.Not.Contain("end is null").IgnoreCase, InputSet, BothSame, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => AnotherEntityNullability.True != o.InputNullability);
			Expect(q, Does.Not.Contain("end is null").IgnoreCase, InputSet, BothSame, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => "input" != o.Input);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input != "input");
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input != o.Output);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Output != o.Input);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input != o.NullableOutput);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothDifferent, InputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput != o.Input);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothDifferent, InputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequired.Output != o.Input);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothDifferent, InputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input != o.NullableAnotherEntityRequired.Output);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothDifferent, InputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequired.Input != o.Output);
			Expect(q, Does.Contain("Input is null").IgnoreCase, BothDifferent, OutputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Output != o.NullableAnotherEntityRequired.Input);
			Expect(q, Does.Contain("Input is null").IgnoreCase, BothDifferent, OutputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => 3 != o.NullableOutput.Length);
			Expect(q, Does.Contain("is null").IgnoreCase, InputSet, BothDifferent, BothNull, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput.Length != 3);
			Expect(q, Does.Contain("is null").IgnoreCase, InputSet, BothDifferent, BothNull, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => 3 != o.Input.Length);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, InputSet, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input.Length != 3);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, InputSet, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => (o.NullableAnotherEntityRequiredId ?? 0) != (o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId ?? 0));
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => (o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId ?? 0) != (o.NullableAnotherEntityRequiredId ?? 0));
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.GetValueOrDefault() != o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.GetValueOrDefault());
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.GetValueOrDefault() != o.NullableAnotherEntityRequiredId.GetValueOrDefault());
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.HasValue && o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.HasValue && o.NullableAnotherEntityRequiredId.Value != o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.Value);
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.Value != o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.Value && o.NullableAnotherEntityRequiredId.HasValue && o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.HasValue);
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.HasValue && o.NullableAnotherEntityRequiredId.Value != 0);
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.Value != 0 && o.NullableAnotherEntityRequiredId.HasValue);
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.HasValue || o.NullableAnotherEntityRequiredId.Value != 0);
			ExpectAll(q, Does.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.Value != 0 || o.NullableAnotherEntityRequiredId.HasValue);
			ExpectAll(q, Does.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput != null && o.NullableOutput != "test");
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent, BothSame, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput != "test" && o.NullableOutput != null);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent, BothSame, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput != null || o.NullableOutput != "test");
			ExpectAll(q, Does.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput != "test" || o.NullableOutput != null);
			ExpectAll(q, Does.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput != "test" && (o.NullableAnotherEntityRequiredId > 0 && o.NullableOutput != null));
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent, BothSame, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput != null && (o.NullableAnotherEntityRequiredId > 0 && o.NullableOutput != "test"));
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent, BothSame, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.Value != o.NullableAnotherEntityRequiredId.Value);
			Expect(q, Does.Contain("or case").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.Any(r => r.Output != o.Input));
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase.And.Contain("Output is null").IgnoreCase, BothDifferent, InputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.All(r => r.Output != o.Input));
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase.And.Contain("Output is null").IgnoreCase, InputSet, OutputSet, BothDifferent, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.All(r => r.Output != null && r.Output != o.Input));
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase.And.Not.Contain("Output is null").IgnoreCase, BothDifferent, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => (o.NullableOutput + o.Output) != o.Output);
			ExpectAll(q, Does.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => (o.Input + o.Output) != o.Output);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothSame, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Address.Street != o.Output);
			Expect(q, Does.Contain("Input is null").IgnoreCase, BothDifferent, OutputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Address.City != o.Output);
			Expect(q, Does.Contain("Output is null").IgnoreCase, InputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Address.City != null && o.Address.City != o.Output);
			Expect(q, Does.Not.Contain("Output is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Address.Street != null && o.Address.Street != o.NullableOutput);
			Expect(q, Does.Contain("Output is null").IgnoreCase, InputSet, BothDifferent);

			Expect(session.Query<Customer>().Where(o => o.CustomerId != null), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<Customer>().Where(o => null != o.CustomerId), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<Customer>().Where(o => o.CustomerId != "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<Customer>().Where(o => "test" != o.CustomerId), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<OrderLine>().Where(o => o.Order.Customer.CustomerId != "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<OrderLine>().Where(o => "test" != o.Order.Customer.CustomerId), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<OrderLine>().Where(o => o.Order.Customer.CompanyName != "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<OrderLine>().Where(o => "test" != o.Order.Customer.CompanyName), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.CreatedBy.CreatedBy.Name != "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => "test" != o.CreatedBy.CreatedBy.CreatedBy.Name), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.CreatedBy.Id != 5), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => 5 != o.CreatedBy.CreatedBy.Id), Does.Not.Contain("is null").IgnoreCase);
		}

		[Test]
		public void NullInequalityWithNotNullSubSelect()
		{
			if (!Dialect.SupportsScalarSubSelects)
			{
				Assert.Ignore("Dialect does not support scalar subselects");
			}

			var q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.Count != 1);
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.Max(r => r.Id) != 0);
			ExpectAll(q, Does.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.All(r => r.Output != null) != o.NullableBool);
			ExpectAll(q, Does.Not.Contain("or case").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.Where(r => r.Id == 0).Sum(r => r.Input.Length) != 5);
			ExpectAll(q, Does.Contain("or (").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.All(r => r.Output != null) != (o.NullableOutput.Length > 0));
			Expect(q, Does.Not.Contain("or case").IgnoreCase);
		}

		[Test]
		public void NullEqualityWithNotNull()
		{
			var q = session.Query<AnotherEntityRequired>().Where(o => o.Input == null);
			Expect(q, Does.Contain("is null").IgnoreCase, OutputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => null == o.Input);
			Expect(q, Does.Contain("is null").IgnoreCase, OutputSet, BothNull);

			q = session.Query<AnotherEntityRequired>().Where(o => o.InputNullability == AnotherEntityNullability.True);
			Expect(q, Does.Not.Contain("end is null").IgnoreCase, BothNull, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => AnotherEntityNullability.True == o.InputNullability);
			Expect(q, Does.Not.Contain("end is null").IgnoreCase, BothNull, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => "input" == o.Input);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, InputSet, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input == "input");
			Expect(q, Does.Not.Contain("is null").IgnoreCase, InputSet, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input == o.Output);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Output == o.Input);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input == o.NullableOutput);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput == o.Input);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequired.Output == o.Input);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input == o.NullableAnotherEntityRequired.Output);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequired.Input == o.Output);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Output == o.NullableAnotherEntityRequired.Input);
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => 3 == o.Input.Length);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Input.Length == 3);
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => (o.NullableAnotherEntityRequiredId ?? 0) == (o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId ?? 0));
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => (o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId ?? 0) == (o.NullableAnotherEntityRequiredId ?? 0));
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.GetValueOrDefault() == o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.GetValueOrDefault());
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.GetValueOrDefault() == o.NullableAnotherEntityRequiredId.GetValueOrDefault());
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.HasValue && o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.HasValue && o.NullableAnotherEntityRequiredId.Value == o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.Value);
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.Value == o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.Value && o.NullableAnotherEntityRequiredId.HasValue && o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.HasValue);
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.HasValue && o.NullableAnotherEntityRequiredId.Value == 0);
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.Value == 0 && o.NullableAnotherEntityRequiredId.HasValue);
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.HasValue || o.NullableAnotherEntityRequiredId.Value == 0);
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequiredId.Value == 0 || o.NullableAnotherEntityRequiredId.HasValue);
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput == "test");
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableOutput != null || o.NullableOutput == "test");
			Expect(q, Does.Not.Contain("is null").IgnoreCase, OutputSet, BothDifferent, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.NullableAnotherEntityRequired.NullableAnotherEntityRequiredId.Value == o.NullableAnotherEntityRequiredId.Value);
			ExpectAll(q, Does.Contain("Id is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.Any(r => r.Output == o.Input));
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase.And.Not.Contain("Output is null").IgnoreCase, BothSame);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.All(r => r.Output == o.Input));
			Expect(q, Does.Not.Contain("Input is null").IgnoreCase.And.Not.Contain("Output is null").IgnoreCase, BothSame, BothNull, InputSet, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.All(r => r.Output == o.NullableOutput));
			ExpectAll(q, Does.Contain("Output is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.All(r => r.Output != null && o.NullableOutput != null && r.Output == o.NullableOutput));
			Expect(q, Does.Not.Contain("Output is null").IgnoreCase, BothSame, BothDifferent, OutputSet);

			q = session.Query<AnotherEntityRequired>().Where(o => (o.NullableOutput + o.Output) == o.Output);
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => (o.Output + o.Output) == o.Output);
			Expect(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => !o.Input.Equals(o.Output));
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => !o.Output.Equals(o.Input));
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => !o.Input.Equals(o.NullableOutput));
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => !o.NullableOutput.Equals(o.Input));
			Expect(q, Does.Not.Contain("is null").IgnoreCase, BothDifferent);

			q = session.Query<AnotherEntityRequired>().Where(o => !o.NullableOutput.Equals(o.NullableOutput));
			Expect(q, Does.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => !o.NullableOutput.Equals(o.NullableOutput));
			Expect(q, Does.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Address.City == o.NullableOutput);
			ExpectAll(q, Does.Contain("Output is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.Address.Street != null && o.Address.Street == o.NullableOutput);
			Expect(q, Does.Not.Contain("Output is null").IgnoreCase, BothSame);

			Expect(session.Query<Customer>().Where(o => o.CustomerId == null), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<Customer>().Where(o => null == o.CustomerId), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<Customer>().Where(o => o.CustomerId == "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<Customer>().Where(o => "test" == o.CustomerId), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<OrderLine>().Where(o => o.Order.Customer.CustomerId == "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<OrderLine>().Where(o => "test" == o.Order.Customer.CustomerId), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<OrderLine>().Where(o => o.Order.Customer.CompanyName == "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<OrderLine>().Where(o => "test" == o.Order.Customer.CompanyName), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.CreatedBy.CreatedBy.Name == "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => "test" == o.CreatedBy.CreatedBy.CreatedBy.Name), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.CreatedBy.Id == 5), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => 5 == o.CreatedBy.CreatedBy.Id), Does.Not.Contain("is null").IgnoreCase);
		}

		[Test]
		public void NullEqualityWithNotNullSubSelect()
		{
			if (!Dialect.SupportsScalarSubSelects)
			{
				Assert.Ignore("Dialect does not support scalar subselects");
			}

			var q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.Count == 1);
			ExpectAll(q, Does.Not.Contain("is null").IgnoreCase);

			q = session.Query<AnotherEntityRequired>().Where(o => o.RelatedItems.Max(r => r.Id) == 0);
			Expect(q, Does.Not.Contain("is null").IgnoreCase);
		}

		[Test]
		public void NullEquality()
		{
			string nullVariable = null;
			string nullVariable2 = null;
			string notNullVariable = "input";

			Assert.AreEqual(5, session.CreateCriteria<AnotherEntity>().List<AnotherEntity>().Count);

			IQueryable<AnotherEntity> q;

			// Null literal against itself
			q = from x in session.Query<AnotherEntity>() where null == null select x;
			ExpectAll(q);

			// Null against constants
			q = from x in session.Query<AnotherEntity>() where null == "value" select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where "value" == null select x;
			ExpectNone(q);

			// Null against variables
			q = from x in session.Query<AnotherEntity>() where null == nullVariable select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where null == notNullVariable select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where nullVariable == null select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where notNullVariable == null select x;
			ExpectNone(q);

			// Null against columns
			q = from x in session.Query<AnotherEntity>() where x.Input == null select x;
			ExpectInputIsNull(q);
			q = from x in session.Query<AnotherEntity>() where null == x.Input select x;
			ExpectInputIsNull(q);

			// All null pairings with two columns.
			q = from x in session.Query<AnotherEntity>() where x.Input == null && x.Output == null select x;
			Expect(q, BothNull);
			q = from x in session.Query<AnotherEntity>() where x.Input != null && x.Output == null select x;
			Expect(q, InputSet);
			q = from x in session.Query<AnotherEntity>() where x.Input == null && x.Output != null select x;
			Expect(q, OutputSet);
			q = from x in session.Query<AnotherEntity>() where x.Input != null && x.Output != null select x;
			Expect(q, BothSame, BothDifferent);

			// Variables against variables
			q = from x in session.Query<AnotherEntity>() where nullVariable == nullVariable2 select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where nullVariable == notNullVariable select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where notNullVariable == nullVariable select x;
			ExpectNone(q);

			//// Variables against columns
			q = from x in session.Query<AnotherEntity>() where nullVariable == x.Input select x;
			ExpectInputIsNull(q);
			q = from x in session.Query<AnotherEntity>() where notNullVariable == x.Input select x;
			Expect(q, InputSet, BothDifferent);
			q = from x in session.Query<AnotherEntity>() where x.Input == nullVariable select x;
			ExpectInputIsNull(q);
			q = from x in session.Query<AnotherEntity>() where x.Input == notNullVariable select x;
			Expect(q, InputSet, BothDifferent);

			// Columns against columns
			q = from x in session.Query<AnotherEntity>() where x.Input == x.Output select x;
			Expect(q, BothSame, BothNull);

			Expect(session.Query<OrderLine>().Where(o => o.Order.Customer.ContactName == null), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<OrderLine>().Where(o => null == o.Order.Customer.ContactName), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<OrderLine>().Where(o => o.Order.Customer.ContactName == "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<OrderLine>().Where(o => "test" == o.Order.Customer.ContactName), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => null == o.Component.Property1), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => o.Component.Property1 == null), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => "test" == o.Component.Property1), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => o.Component.Property1 == "test"), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => null == o.Component.OtherComponent.OtherProperty1), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => o.Component.OtherComponent.OtherProperty1 == null), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => "test" == o.Component.OtherComponent.OtherProperty1), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => o.Component.OtherComponent.OtherProperty1 == "test"), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.ModifiedBy.CreatedBy.Name == "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => "test" == o.CreatedBy.ModifiedBy.CreatedBy.Name), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.CreatedBy.Component.OtherComponent.OtherProperty1 == "test"), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => "test" == o.CreatedBy.CreatedBy.Component.OtherComponent.OtherProperty1), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.ModifiedBy.CreatedBy.Id == 5), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => 5 == o.ModifiedBy.CreatedBy.Id), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.ModifiedBy.Id == 5), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => 5 == o.CreatedBy.ModifiedBy.Id), Does.Not.Contain("is null").IgnoreCase);

			short value = 3;
			Expect(session.Query<User>().Where(o => o.NullableShort == value), Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast"));
			Expect(session.Query<User>().Where(o => value == o.NullableShort), Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast"));

			Expect(session.Query<User>().Where(o => o.NullableShort.Value == value), Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast"));
			Expect(session.Query<User>().Where(o => value == o.NullableShort.Value), Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast"));
			Expect(session.Query<User>().Where(o => o.Short == value), Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast"));
			Expect(session.Query<User>().Where(o => value == o.Short), Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast"));

			var shouldCast = Sfi.Dialect is SQLiteDialect || // transparent cast is translated to sql
			                 Sfi.Dialect.TryGetCastTypeName(NHibernateUtil.Int16.SqlType, out var shortCast) &&
			                 Sfi.Dialect.TryGetCastTypeName(NHibernateUtil.Int32.SqlType, out var intCast) &&
			                 shortCast != intCast;
			Expect(session.Query<User>().Where(o => o.NullableShort == 3), shouldCast ? WithoutIsNullAndWithCast() : WithoutIsNullAndWithoutCast());
			Expect(session.Query<User>().Where(o => 3 == o.NullableShort), shouldCast ? WithoutIsNullAndWithCast() : WithoutIsNullAndWithoutCast());

			Expect(session.Query<User>().Where(o => o.NullableShort.Value == 3), shouldCast ? WithoutIsNullAndWithCast() : WithoutIsNullAndWithoutCast());
			Expect(session.Query<User>().Where(o => 3 == o.NullableShort.Value), shouldCast ? WithoutIsNullAndWithCast() : WithoutIsNullAndWithoutCast());
			Expect(session.Query<User>().Where(o => o.Short == 3), shouldCast ? WithoutIsNullAndWithCast() : WithoutIsNullAndWithoutCast());
			Expect(session.Query<User>().Where(o => 3 == o.Short), shouldCast ? WithoutIsNullAndWithCast() : WithoutIsNullAndWithoutCast());
		}

		[Test]
		public void NullInequality()
		{
			string nullVariable = null;
			string nullVariable2 = null;
			string notNullVariable = "input";

			IQueryable<AnotherEntity> q;

			// Null literal against itself
			q = from x in session.Query<AnotherEntity>() where null != null select x;
			ExpectNone(q);

			// Null against constants
			q = from x in session.Query<AnotherEntity>() where null != "value" select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where "value" != null select x;
			ExpectAll(q);

			// Null against variables
			q = from x in session.Query<AnotherEntity>() where null != nullVariable select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where null != notNullVariable select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where nullVariable != null select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where notNullVariable != null select x;
			ExpectAll(q);

			// Null against columns.
			q = from x in session.Query<AnotherEntity>() where x.Input != null select x;
			ExpectInputIsNotNull(q);
			q = from x in session.Query<AnotherEntity>() where null != x.Input select x;
			ExpectInputIsNotNull(q);

			// Variables against variables.
			q = from x in session.Query<AnotherEntity>() where nullVariable != nullVariable2 select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where nullVariable != notNullVariable select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where notNullVariable != nullVariable select x;
			ExpectAll(q);

			// Variables against columns.
			q = from x in session.Query<AnotherEntity>() where nullVariable != x.Input select x;
			ExpectInputIsNotNull(q);
			q = from x in session.Query<AnotherEntity>() where notNullVariable != x.Input select x;
			Expect(q, BothSame, BothNull, OutputSet);
			q = from x in session.Query<AnotherEntity>() where x.Input != nullVariable select x;
			ExpectInputIsNotNull(q);
			q = from x in session.Query<AnotherEntity>() where x.Input != notNullVariable select x;
			Expect(q, BothSame, OutputSet, BothNull);

			// Columns against columns
			q = from x in session.Query<AnotherEntity>() where x.Input != x.Output select x;
			Expect(q, BothDifferent, InputSet, OutputSet);

			Expect(session.Query<OrderLine>().Where(o => o.Order.Customer.ContactName != null), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<OrderLine>().Where(o => null != o.Order.Customer.ContactName), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<OrderLine>().Where(o => o.Order.Customer.ContactName != "test"), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<OrderLine>().Where(o => "test" != o.Order.Customer.ContactName), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => null != o.Component.Property1), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => o.Component.Property1 != null), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => "test" != o.Component.Property1), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => o.Component.Property1 != "test"), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => null != o.Component.OtherComponent.OtherProperty1), Does.Not.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => o.Component.OtherComponent.OtherProperty1 != null), Does.Not.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => "test" != o.Component.OtherComponent.OtherProperty1), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => o.Component.OtherComponent.OtherProperty1 != "test"), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.ModifiedBy.CreatedBy.Name != "test"), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => "test" != o.CreatedBy.ModifiedBy.CreatedBy.Name), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.CreatedBy.Component.OtherComponent.OtherProperty1 != "test"), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => "test" != o.CreatedBy.CreatedBy.Component.OtherComponent.OtherProperty1), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.ModifiedBy.CreatedBy.Id != 5), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => 5 != o.ModifiedBy.CreatedBy.Id), Does.Contain("is null").IgnoreCase);

			Expect(session.Query<User>().Where(o => o.CreatedBy.ModifiedBy.Id != 5), Does.Contain("is null").IgnoreCase);
			Expect(session.Query<User>().Where(o => 5 != o.CreatedBy.ModifiedBy.Id), Does.Contain("is null").IgnoreCase);

			short value = 3;
			Expect(session.Query<User>().Where(o => o.NullableShort != value), Does.Contain("is null").IgnoreCase.And.Not.Contain("cast"));
			Expect(session.Query<User>().Where(o => value != o.NullableShort), Does.Contain("is null").IgnoreCase.And.Not.Contain("cast"));

			Expect(session.Query<User>().Where(o => o.NullableShort.Value != value), Does.Contain("is null").IgnoreCase.And.Not.Contain("cast"));
			Expect(session.Query<User>().Where(o => value != o.NullableShort.Value), Does.Contain("is null").IgnoreCase.And.Not.Contain("cast"));
			Expect(session.Query<User>().Where(o => o.Short != value), Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast"));
			Expect(session.Query<User>().Where(o => value != o.Short), Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast"));

			var shouldCast = Sfi.Dialect is SQLiteDialect || // transparent cast is translated to sql
							 Sfi.Dialect.TryGetCastTypeName(NHibernateUtil.Int16.SqlType, out var shortCast) &&
							 Sfi.Dialect.TryGetCastTypeName(NHibernateUtil.Int32.SqlType, out var intCast) &&
							 shortCast != intCast;
			Expect(session.Query<User>().Where(o => o.NullableShort != 3), shouldCast ? WithIsNullAndWithCast() : WithIsNullAndWithoutCast());
			Expect(session.Query<User>().Where(o => 3 != o.NullableShort), shouldCast ? WithIsNullAndWithCast() : WithIsNullAndWithoutCast());

			Expect(session.Query<User>().Where(o => o.NullableShort.Value != 3), shouldCast ? WithIsNullAndWithCast() : WithIsNullAndWithoutCast());
			Expect(session.Query<User>().Where(o => 3 != o.NullableShort.Value), shouldCast ? WithIsNullAndWithCast() : WithIsNullAndWithoutCast());
			Expect(session.Query<User>().Where(o => o.Short != 3), shouldCast ? WithoutIsNullAndWithCast() : WithoutIsNullAndWithoutCast());
			Expect(session.Query<User>().Where(o => 3 != o.Short), shouldCast ? WithoutIsNullAndWithCast() : WithoutIsNullAndWithoutCast());
		}

		private IResolveConstraint WithIsNullAndWithoutCast()
		{
			return Does.Contain("is null").IgnoreCase.And.Not.Contain("cast").IgnoreCase;
		}

		private IResolveConstraint WithIsNullAndWithCast()
		{
			return Does.Contain("is null").IgnoreCase.And.Contain("cast").IgnoreCase;
		}

		private IResolveConstraint WithoutIsNullAndWithoutCast()
		{
			return Does.Not.Contain("is null").IgnoreCase.And.Not.Contain("cast").IgnoreCase;
		}

		private IResolveConstraint WithoutIsNullAndWithCast()
		{
			return Does.Not.Contain("is null").IgnoreCase.And.Contain("cast").IgnoreCase;
		}

		[Test]
		public void NullEqualityInverted()
		{
			string nullVariable = null;
			string nullVariable2 = null;
			string notNullVariable = "input";

			IQueryable<AnotherEntity> q;

			// Null literal against itself
			q = from x in session.Query<AnotherEntity>() where !(null == null) select x;
			ExpectNone(q);

			// Null against constants
			q = from x in session.Query<AnotherEntity>() where !(null == "value") select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where !("value" == null) select x;
			ExpectAll(q);

			// Null against variables
			q = from x in session.Query<AnotherEntity>() where !(null == nullVariable) select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where !(null == notNullVariable) select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where !(nullVariable == null) select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where !(notNullVariable == null) select x;
			ExpectAll(q);

			// Null against columns
			q = from x in session.Query<AnotherEntity>() where !(x.Input == null) select x;
			ExpectInputIsNotNull(q);
			q = from x in session.Query<AnotherEntity>() where !(null == x.Input) select x;
			ExpectInputIsNotNull(q);

			// All null pairings with two columns.
			q = from x in session.Query<AnotherEntity>() where !(x.Input == null && x.Output == null) select x;
			Expect(q, InputSet, OutputSet, BothSame, BothDifferent);
			q = from x in session.Query<AnotherEntity>() where !(x.Input != null && x.Output == null) select x;
			Expect(q, OutputSet, BothNull, BothSame, BothDifferent);
			q = from x in session.Query<AnotherEntity>() where !(x.Input == null && x.Output != null) select x;
			Expect(q, InputSet, BothSame, BothDifferent, BothNull);
			q = from x in session.Query<AnotherEntity>() where !(x.Input != null && x.Output != null) select x;
			Expect(q, InputSet, OutputSet, BothNull);

			// Variables against variables
			q = from x in session.Query<AnotherEntity>() where !(nullVariable == nullVariable2) select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where !(nullVariable == notNullVariable) select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where !(notNullVariable == nullVariable) select x;
			ExpectAll(q);

			// Variables against columns
			q = from x in session.Query<AnotherEntity>() where !(nullVariable == x.Input) select x;
			ExpectInputIsNotNull(q);
			q = from x in session.Query<AnotherEntity>() where !(notNullVariable == x.Input) select x;
			Expect(q, BothSame);
			q = from x in session.Query<AnotherEntity>() where !(x.Input == nullVariable) select x;
			ExpectInputIsNotNull(q);
			q = from x in session.Query<AnotherEntity>() where !(x.Input == notNullVariable) select x;
			Expect(q, BothSame);

			// Columns against columns
			q = from x in session.Query<AnotherEntity>() where !(x.Input == x.Output) select x;
			Expect(q, BothDifferent);
		}

		[Test]
		public void NullInequalityInverted()
		{
			string nullVariable = null;
			string nullVariable2 = null;
			string notNullVariable = "input";

			IQueryable<AnotherEntity> q;

			// Null literal against itself
			q = from x in session.Query<AnotherEntity>() where !(null != null) select x;
			ExpectAll(q);

			// Null against constants
			q = from x in session.Query<AnotherEntity>() where !(null != "value") select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where !("value" != null) select x;
			ExpectNone(q);

			// Null against variables
			q = from x in session.Query<AnotherEntity>() where !(null != nullVariable) select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where !(null != notNullVariable) select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where !(nullVariable != null) select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where !(notNullVariable != null) select x;
			ExpectNone(q);

			// Null against columns.
			q = from x in session.Query<AnotherEntity>() where !(x.Input != null) select x;
			ExpectInputIsNull(q);
			q = from x in session.Query<AnotherEntity>() where !(null != x.Input) select x;
			ExpectInputIsNull(q);

			// Variables against variables.
			q = from x in session.Query<AnotherEntity>() where !(nullVariable != nullVariable2) select x;
			ExpectAll(q);
			q = from x in session.Query<AnotherEntity>() where !(nullVariable != notNullVariable) select x;
			ExpectNone(q);
			q = from x in session.Query<AnotherEntity>() where !(notNullVariable != nullVariable) select x;
			ExpectNone(q);

			// Variables against columns.
			q = from x in session.Query<AnotherEntity>() where !(nullVariable != x.Input) select x;
			ExpectInputIsNull(q);
			q = from x in session.Query<AnotherEntity>() where !(notNullVariable != x.Input) select x;
			Expect(q, InputSet, BothDifferent);
			q = from x in session.Query<AnotherEntity>() where !(x.Input != nullVariable) select x;
			ExpectInputIsNull(q);
			q = from x in session.Query<AnotherEntity>() where !(x.Input != notNullVariable) select x;
			Expect(q, InputSet, BothDifferent);

			// Columns against columns
			q = from x in session.Query<AnotherEntity>() where !(x.Input != x.Output) select x;
			Expect(q, BothSame);
		}

		private void ExpectAll(IQueryable<AnotherEntity> q)
		{
			Expect(q, BothNull, BothSame, BothDifferent, InputSet, OutputSet);
		}

		private void ExpectNone(IQueryable<AnotherEntity> q)
		{
			Expect(q);
		}

		private void ExpectInputIsNull(IQueryable<AnotherEntity> q)
		{
			Expect(q, BothNull, OutputSet);
		}

		private void ExpectInputIsNotNull(IQueryable<AnotherEntity> q)
		{
			Expect(q, InputSet, BothSame, BothDifferent);
		}

		private void Expect(IQueryable<AnotherEntity> q, params AnotherEntity[] entities)
		{
			IList<AnotherEntity> results = q.ToList().OrderBy(l=> Key(l)).ToList();
			IList<AnotherEntity> check = entities.OrderBy(l => Key(l)).ToList();

			Assert.AreEqual(check.Count, results.Count);
			for(int i=0; i<check.Count; i++)
				Assert.AreEqual(Key(check[i]), Key(results[i]));
		}

		private string Key(AnotherEntity e)
		{
			return "Input=" + (e.Input ?? "NULL") + ", Output=" + (e.Output ?? "NULL");
		}

		private void ExpectAll(IQueryable<AnotherEntityRequired> q, IResolveConstraint sqlConstraint)
		{
			Expect(q, sqlConstraint, BothNull, BothSame, BothDifferent, InputSet, OutputSet);
		}

		private void Expect(IQueryable<AnotherEntityRequired> q, IResolveConstraint sqlConstraint, params AnotherEntity[] entities)
		{
			IList<AnotherEntityRequired> results;
			if (sqlConstraint == null)
			{
				results = GetResults(q);
			}
			else
			{
				using (var sqlLog = new SqlLogSpy())
				{
					results = GetResults(q);
					Assert.That(sqlLog.GetWholeLog(), sqlConstraint);
				}
			}

			IList<AnotherEntity> check = entities.OrderBy(Key).ToList();

			Assert.That(results.Count, Is.EqualTo(check.Count));
			for (var i = 0; i < check.Count; i++)
			{
				Assert.That(Key(results[i]), Is.EqualTo(Key(check[i])));
			}
		}

		private IList<AnotherEntityRequired> GetResults(IQueryable<AnotherEntityRequired> q)
		{
			return q.ToList().OrderBy(Key).ToList();
		}

		private static void Expect<T>(IQueryable<T> query, IResolveConstraint sqlConstraint)
		{
			using (var sqlLog = new SqlLogSpy())
			{
				var list = query.ToList();
				Assert.That(sqlLog.GetWholeLog(), sqlConstraint);
			}
		}

		private static string Key(AnotherEntityRequired e)
		{
			return "Input=" + (e.Input ?? "NULL") + ", Output=" + (e.Output ?? "NULL");
		}
	}
}
