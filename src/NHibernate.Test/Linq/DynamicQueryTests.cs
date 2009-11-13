using System;
using System.Linq;
using System.Linq.Dynamic;
using NHibernate.Test.Linq.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class DynamicQueryTests : LinqTestCase
	{
		[Test]
		public void CanQueryWithDynamicOrderBy()
		{
			var query = from user in db.Users
						select user;

			//dynamic orderby clause
			query = query.OrderBy("RegisteredAt");

			var list = query.ToList();

			//assert list was returned in order
			DateTime previousDate = DateTime.MinValue;
			list.Each(delegate(User user)
			{
				Assert.IsTrue(previousDate <= user.RegisteredAt);
				previousDate = user.RegisteredAt;
			});
		}
	}
}
