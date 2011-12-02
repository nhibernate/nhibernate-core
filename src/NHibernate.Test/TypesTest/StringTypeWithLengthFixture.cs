using System;
using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for StringTypeWithLengthFixture.
	/// </summary>
	[TestFixture]
	public class StringTypeWithLengthFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "String"; }
		}


		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						String.Format("TypesTest.{0}ClassWithLength.hbm.xml", TypeName)
					};
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// this test only works where the driver has set an explicit length on the IDbDataParameter
			return dialect is MsSql2008Dialect;
		}

		[Test]
		public void ThrowsOnTooLong()
		{
			PropertyValueException ex = Assert.Throws<PropertyValueException>(() =>
				{
					using (ISession s = OpenSession())
					{
						StringClass b = new StringClass();
						b.StringValue = "0123456789a";
						s.Save(b);
						s.Flush();
					}
				});

			Assert.That(ex.Message, Iz.EqualTo("Error dehydrating property value for NHibernate.Test.TypesTest.StringClass.StringValue"));
			Assert.That(ex.InnerException, Iz.TypeOf<HibernateException>());
			Assert.That(ex.InnerException.Message, Iz.EqualTo("The length of the string value exceeds the length configured in the mapping."));
		}
	}
}
