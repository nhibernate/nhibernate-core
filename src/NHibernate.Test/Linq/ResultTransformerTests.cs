using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate.Linq;
using NHibernate.Transform;
using System.Collections;

namespace NHibernate.Test.Linq
{
	public class ResultTransformerTests : LinqTestCase
	{
		class DummyResultTransformer : IResultTransformer
		{
			public bool[] Called { get; set; }

			public object TransformTuple(object[] tuple, string[] aliases)
			{
				this.Called[1] = true;
				return null;
			}

			public IList TransformList(IList collection)
			{
				this.Called[0] = true;
				return collection;
			}
		}

		[Test]
		public void CanSetResultTransformerOnList()
		{
			//NH-3702
			var called = new bool[2];

			var result = (from e in db.Customers select e).SetResultTransformer(new DummyResultTransformer() { Called = called }).ToList();

			Assert.IsTrue(called[0]);
			Assert.IsTrue(called[1]);
		}

		[Test]
		public void CanSetResultTransformerOnProjection()
		{
			//NH-3702
			var called = new bool[2];

			var result = (from e in db.Customers select e.ContactName).SetResultTransformer(new DummyResultTransformer() { Called = called }).ToList();

			Assert.IsTrue(called[0]);
			Assert.IsTrue(called[1]);
		}
	}
}
