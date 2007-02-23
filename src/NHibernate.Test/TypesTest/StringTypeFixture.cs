using System;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for StringTypeFixture.
	/// </summary>
	[TestFixture]
	public class StringTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "String"; }
		}

		[Test]
		public void InsertNullValue()
		{
			using (ISession s = OpenSession())
			{
				StringClass b = new StringClass();
				b.StringValue = null;
				s.Save(b);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				StringClass b = (StringClass) s.CreateCriteria(
				                              	typeof(StringClass)).UniqueResult();
				Assert.IsNull(b.StringValue);
				s.Delete(b);
				s.Flush();
			}
		}
	}
}