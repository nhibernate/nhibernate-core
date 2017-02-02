using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

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
			Expect(q, BothSame);
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
			Expect(q, BothSame);
			q = from x in session.Query<AnotherEntity>() where x.Input != nullVariable select x;
			ExpectInputIsNotNull(q);
			q = from x in session.Query<AnotherEntity>() where x.Input != notNullVariable select x;
			Expect(q, BothSame);

			// Columns against columns
			q = from x in session.Query<AnotherEntity>() where x.Input != x.Output select x;
			Expect(q, BothDifferent);
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
	}
}
