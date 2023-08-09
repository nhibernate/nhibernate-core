using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Collection.Generic.SetHelpers;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class SetSnapShotFixture
	{
		[Test]
		public void TestNullValue()
		{
			var sn = new SetSnapShot<object>(1);
			Assert.That(sn, Has.Count.EqualTo(0));
			Assert.That(sn, Is.EquivalentTo(new object[0]));
			Assert.That(sn.Contains(null), Is.False);
			Assert.That(sn.TryGetValue(null, out _), Is.False);

			sn.Add(null);
			Assert.That(sn, Has.Count.EqualTo(1));
			Assert.That(sn, Is.EquivalentTo(new object[] {null}));

			Assert.That(sn.TryGetValue(null, out var value), Is.True);
			Assert.That(sn.Contains(null), Is.True);
			Assert.That(value, Is.Null);

			Assert.That(sn.Remove(null), Is.True);
			Assert.That(sn, Has.Count.EqualTo(0));
			Assert.That(sn, Is.EquivalentTo(new object[0]));

			sn.Add(null);
			Assert.That(sn, Has.Count.EqualTo(1));

			sn.Clear();
			Assert.That(sn, Has.Count.EqualTo(0));
			Assert.That(sn, Is.EquivalentTo(new object[0]));
		}

		[Test]
		public void TestInitialization()
		{
			var list = new List<string> {"test1", null, "test2"};
			var sn = new SetSnapShot<string>(list);
			Assert.That(sn, Has.Count.EqualTo(list.Count));
			Assert.That(sn, Is.EquivalentTo(list));
			Assert.That(sn.TryGetValue("test1", out _), Is.True);
			Assert.That(sn.TryGetValue(null, out _), Is.True);
		}

		[Test]
		public void TestDuplicates()
		{
			var list = new List<string> { "test1", "test1", "test2" };
			var sn = new SetSnapShot<string>(list);
			Assert.That(sn, Has.Count.EqualTo(2));
			Assert.That(sn.TryGetValue("test1", out _), Is.True);
			Assert.That(sn.TryGetValue("test2", out _), Is.True);
		}

		[Test]
		public void TestCopyTo()
		{
			var list = new List<string> {"test1", null, "test2"};
			var sn = new SetSnapShot<string>(list);

			var array = new string[3];
			sn.CopyTo(array, 0);
			Assert.That(list, Is.EquivalentTo(array));
		}

		[Test]
		public void TestCopyToObjectArray()
		{
			var list = new List<string> { "test1", null, "test2" };
			ICollection sn = new SetSnapShot<string>(list);

			var array = new object[3];
			sn.CopyTo(array, 0);
			Assert.That(list, Is.EquivalentTo(array));
		}

		[Test]
		public void WhenCopyToIsCalledWithIncompatibleArrayTypeThenThrowArgumentOrInvalidCastException()
		{
			var list = new List<string> { "test1", null, "test2" };
			ICollection sn = new SetSnapShot<string>(list);

			var array = new int[3];
			Assert.That(
				() => sn.CopyTo(array, 0),
				Throws.ArgumentException.Or.TypeOf<InvalidCastException>());
		}

		[Test]
		public void TestSerialization()
		{
			var list = new List<string> {"test1", null, "test2"};
			var sn = new SetSnapShot<string>(list);

			sn = Deserialize<SetSnapShot<string>>(Serialize(sn));
			Assert.That(sn, Has.Count.EqualTo(list.Count));
			Assert.That(sn, Is.EquivalentTo(list));
			Assert.That(sn.TryGetValue("test1", out var item1), Is.True);
			Assert.That(item1, Is.EqualTo("test1"));
			Assert.That(sn.TryGetValue(null, out var nullValue), Is.True);
			Assert.That(nullValue, Is.Null);
		}

		private static byte[] Serialize<T>(T obj)
		{
			var serializer = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				serializer.Serialize(stream, obj);
				return stream.ToArray();
			}
		}

		private static T Deserialize<T>(byte[] value)
		{
			var serializer = new BinaryFormatter();
			using (var stream = new MemoryStream(value))
			{
				return (T) serializer.Deserialize(stream);
			}
		}
	}
}
