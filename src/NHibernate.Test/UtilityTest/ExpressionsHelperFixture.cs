using NHibernate.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.UtilityTest
{
	public class TestingClass
	{
		public int IntProp
		{
			get { return 0; }
		}

		public bool BoolProp
		{
			get { return false; }
		}

		public IEnumerable<string> CollectionProp
		{
			get { return new string[0]; }
		}
	}

	[TestFixture]
	public class ExpressionsHelperFixture
	{
		[Test]
		public void DecodeMemberAccessExpression()
		{
			Assert.That(ExpressionsHelper.DecodeMemberAccessExpression<TestingClass, int>(x => x.IntProp),
									Is.EqualTo(typeof(TestingClass).GetMember("IntProp")[0]));
			Assert.That(ExpressionsHelper.DecodeMemberAccessExpression<TestingClass, bool>(x => x.BoolProp),
									Is.EqualTo(typeof(TestingClass).GetMember("BoolProp")[0]));
			Assert.That(ExpressionsHelper.DecodeMemberAccessExpression<TestingClass, IEnumerable<string>>(x => x.CollectionProp),
									Is.EqualTo(typeof(TestingClass).GetMember("CollectionProp")[0]));
		}
	}
}