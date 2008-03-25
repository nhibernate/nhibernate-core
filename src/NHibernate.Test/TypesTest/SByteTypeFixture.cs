using NHibernate.Dialect;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for SByteTypeFixture.
	/// </summary>
	[TestFixture]
	public class SByteTypeFixture
	{
		[Test]
		public void Equals()
		{
			SByteType type = (SByteType) NHibernateUtil.SByte;

			Assert.IsTrue(type.IsEqual((sbyte) -1, (sbyte) -1));
			Assert.IsFalse(type.IsEqual((sbyte) -2, (sbyte) -1));
		}

		[Test]
		public void ObjectToSQLString()
		{
			SByteType type = (SByteType) NHibernateUtil.SByte;
			Assert.AreEqual("-1", type.ObjectToSQLString((sbyte) -1, new MsSql2000Dialect()));
		}

		[Test]
		public void StringToObject()
		{
			SByteType type = (SByteType) NHibernateUtil.SByte;
			Assert.AreEqual((sbyte) -1, type.StringToObject("-1"));
		}
	}
}