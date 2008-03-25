using System;
using System.Collections;
using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// TestFixture for <c>type="Serializable"</c> in use by classes.  It test a Property
	/// that is mapped specifically by <c>type="Serializable"</c> and another Property
	/// whose type is a class that is serializable.
	/// </summary>
	[TestFixture]
	public class BasicSerializableFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"NHSpecific.BasicSerializable.hbm.xml"}; }
		}

		/// <summary>
		/// This contains portions of FumTest.CompositeIDs that deal with <c>type="Serializable"</c>
		/// and replacements Foo.NullBlob, and Foo.Blob.
		/// </summary>
		[Test]
		public void TestCRUD()
		{
			ISession s = OpenSession();
			BasicSerializable ser = new BasicSerializable();
			SerializableClass serClass = ser.SerializableProperty;
			s.Save(ser);
			s.Flush();
			s.Close();

			s = OpenSession();
			ser = (BasicSerializable) s.Load(typeof(BasicSerializable), ser.Id);
			Assert.IsNull(ser.Serial, "should have saved as null");

			ser.Serial = ser.SerializableProperty;
			s.Flush();
			s.Close();

			s = OpenSession();
			ser = (BasicSerializable) s.Load(typeof(BasicSerializable), ser.Id);
			Assert.IsTrue(ser.Serial is SerializableClass, "should have been a SerializableClass");
			Assert.AreEqual(ser.SerializableProperty, ser.Serial,
			                "SerializablePorperty and Serial should both be 5 and 'serialize me'");

			IDictionary props = new Hashtable();
			props["foo"] = "bar";
			props["bar"] = "foo";
			ser.Serial = props;
			s.Flush();

			props["x"] = "y";
			s.Flush();
			s.Close();

			s = OpenSession();
			ser = (BasicSerializable) s.Load(typeof(BasicSerializable), ser.Id);

			props = (IDictionary) ser.Serial;
			Assert.AreEqual("bar", props["foo"]);
			Assert.AreEqual("y", props["x"]);
			Assert.AreEqual(serClass, ser.SerializableProperty);

			ser.SerializableProperty._classString = "modify me";
			s.Flush();
			s.Close();

			s = OpenSession();
			ser = (BasicSerializable) s.Load(typeof(BasicSerializable), ser.Id);
			Assert.AreEqual("modify me", ser.SerializableProperty._classString);
			Assert.AreEqual("bar", props["foo"]);
			Assert.AreEqual("y", props["x"]);

			s.Delete(ser);
			s.Flush();
			s.Close();
		}
	}
}