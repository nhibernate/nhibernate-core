using System.Collections;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.EngineTest
{
	[TestFixture]
	public class TypedValueFixture
	{
		[Test]
		public void EqualsCollection()
		{
			ArrayList value1 = new ArrayList();
			value1.Add(10);
			value1.Add(20);

			ArrayList value2 = (ArrayList) value1.Clone();

			TypedValue t1 = new TypedValue(NHibernateUtil.Int32, value1, EntityMode.Poco);
			TypedValue t2 = new TypedValue(NHibernateUtil.Int32, value2, EntityMode.Poco);

			Assert.IsTrue(t1.Equals(t2));
		}

		[Test]
		public void ToStringWithNullValue()
		{
			Assert.AreEqual("null", new TypedValue(NHibernateUtil.Int32, null, EntityMode.Poco).ToString());
		}

		[Test]
		public void WhenTheTypeIsAnArray_ChoseTheDefaultComparer()
		{
			byte[] value = new byte[]{1,2,3};


			var tv = new TypedValue(NHibernateUtil.BinaryBlob, value, EntityMode.Poco);

			Assert.That(tv.Comparer, Is.TypeOf<TypedValue.DefaultComparer>());
		}
	}
}