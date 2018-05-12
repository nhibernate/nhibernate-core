﻿using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class PropertyMethodMappingTests : LinqTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return Dialect.SupportsScalarSubSelects && base.AppliesTo(dialect);
		}

		[Test]
		public void CanExecuteCountInSelectClause()
		{
			var results = db.Timesheets
				.Select(t => t.Entries.Count).OrderBy(s => s).ToList();

			Assert.AreEqual(3, results.Count);
			Assert.AreEqual(0, results[0]);
			Assert.AreEqual(2, results[1]);
			Assert.AreEqual(4, results[2]);
		}

		[Test]
		public void CanExecuteCountInWhereClause()
		{
			var results = db.Timesheets
				.Where(t => t.Entries.Count >= 2).ToList();

			Assert.AreEqual(2, results.Count);
		}
	}
}
