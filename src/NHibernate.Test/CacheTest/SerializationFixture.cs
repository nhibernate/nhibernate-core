﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Properties;
using NHibernate.Type;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class SerializationFixture
	{
		private static readonly List<System.Type> KnownTypes = new List<System.Type>
		{
			typeof(AnyType.ObjectTypeCacheEntry),
			typeof(DateTimeOffset),
			typeof(TimeSpan),
			typeof(UnfetchedLazyProperty),
			typeof(UnknownBackrefProperty),
			typeof(object[]),
			typeof(CacheEntry),
			typeof(CacheLock),
			typeof(CachedItem),
			typeof(CollectionCacheEntry)
		};

		[Test]
		public void TestCacheEntrySerialization()
		{
			var item = CreateCacheEntry();
			var copy = TestDataContractSerializer(item);
			CheckCacheEntry(item, copy);

			copy = TestBinaryFormatter(item);
			CheckCacheEntry(item, copy);
		}

		[Test]
		public void TestCollectionCacheEntrySerialization()
		{
			var item = CreateCollectionCacheEntry();
			var copy = TestDataContractSerializer(item);
			CheckCollectionCacheEntry(item, copy);

			copy = TestBinaryFormatter(item);
			CheckCollectionCacheEntry(item, copy);
		}

		[Test]
		public void TestCachedItemSerialization()
		{
			// CacheEntry
			var item = CreateCachedItem(CreateCacheEntry());
			var copy = TestDataContractSerializer(item);
			CheckCachedItem(item, copy);

			copy = TestBinaryFormatter(item);
			CheckCachedItem(item, copy);

			// CollectionCacheEntry
			item = CreateCachedItem(CreateCollectionCacheEntry());
			copy = TestDataContractSerializer(item);
			CheckCachedItem(item, copy);

			copy = TestBinaryFormatter(item);
			CheckCachedItem(item, copy);
		}

		[Test]
		public void TestCacheLockSerialization()
		{
			var item = new CacheLock
			{
				Version = 3.5m,
				Id = 100,
				Timeout = 999,
				UnlockTimestamp = 444,
				Multiplicity = 5,
				WasLockedConcurrently = false
			};
			var copy = TestDataContractSerializer(item);
			CheckCacheLock(item, copy);

			copy = TestBinaryFormatter(item);
			CheckCacheLock(item, copy);
		}

		[Test]
		public void TestAnyTypeObjectTypeCacheEntrySerialization()
		{
			var item = CreateObjectTypeCacheEntry();
			var copy = TestDataContractSerializer(item);
			CheckObjectTypeCacheEntry(item, copy);

			copy = TestBinaryFormatter(item);
			CheckObjectTypeCacheEntry(item, copy);
		}


		[Serializable]
		public class MyEntity
		{
			public int Id { get; set; }
		}

		private CachedItem CreateCachedItem(object value)
		{
			return new CachedItem
			{
				Version = 55L,
				Value = value,
				FreshTimestamp = 500
			};
		}

		private CacheEntry CreateCacheEntry()
		{
			return new CacheEntry
			{
				DisassembledState = GetAllKnownTypeValues(),
				Version = 55,
				Subclass = "Test",
				AreLazyPropertiesUnfetched = true
			};
		}

		private CollectionCacheEntry CreateCollectionCacheEntry()
		{
			return new CollectionCacheEntry
			{
				State = GetAllKnownTypeValues()
			};
		}

		private object[] GetAllKnownTypeValues()
		{
			var entityName = nameof(MyEntity);
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<Root>XmlDoc</Root>");
			var types = new Dictionary<IType, object>
			{
				{NHibernateUtil.AnsiString, "test"},
				{NHibernateUtil.Binary, new byte[] {1, 2, 3, 4}},
				{NHibernateUtil.BinaryBlob, new byte[] {1, 2, 3, 4}},
				{NHibernateUtil.Boolean, true},
				{NHibernateUtil.Byte, (byte) 1},
				{NHibernateUtil.Character, 'a'},
				{NHibernateUtil.CultureInfo, CultureInfo.CurrentCulture},
				{NHibernateUtil.DateTime, DateTime.Now},
				{NHibernateUtil.DateTimeNoMs, DateTime.Now},
				{NHibernateUtil.LocalDateTime, DateTime.Now},
				{NHibernateUtil.UtcDateTime, DateTime.UtcNow},
				{NHibernateUtil.LocalDateTimeNoMs, DateTime.Now},
				{NHibernateUtil.UtcDateTimeNoMs, DateTime.UtcNow},
				{NHibernateUtil.DateTimeOffset, DateTimeOffset.Now},
				{NHibernateUtil.Date, DateTime.Today},
				{NHibernateUtil.Decimal, 2.5m},
				{NHibernateUtil.Double, 2.5d},
				{NHibernateUtil.Currency, 2.5m},
				{NHibernateUtil.Guid, Guid.NewGuid()},
				{NHibernateUtil.Int16, (short) 1},
				{NHibernateUtil.Int32, 3},
				{NHibernateUtil.Int64, 3L},
				{NHibernateUtil.SByte, (sbyte) 1},
				{NHibernateUtil.UInt16, (ushort) 1},
				{NHibernateUtil.UInt32, (uint) 1},
				{NHibernateUtil.UInt64, (ulong) 1},
				{NHibernateUtil.Single, 1.1f},
				{NHibernateUtil.String, "test"},
				{NHibernateUtil.StringClob, "test"},
				{NHibernateUtil.Time, DateTime.Now},
				{NHibernateUtil.Ticks, DateTime.Now},
				{NHibernateUtil.TimeAsTimeSpan, TimeSpan.FromMilliseconds(15)},
				{NHibernateUtil.TimeSpan, TimeSpan.FromMilliseconds(1234)},
				{NHibernateUtil.DbTimestamp, DateTime.Now},
				{NHibernateUtil.TrueFalse, false},
				{NHibernateUtil.YesNo, true},
				{NHibernateUtil.Class, typeof(IType)},
				{NHibernateUtil.ClassMetaType, entityName},
				{NHibernateUtil.Serializable, new MyEntity {Id = 1}},
				{NHibernateUtil.Object, new MyEntity {Id = 10}},
				{NHibernateUtil.AnsiChar, 'a'},
				{NHibernateUtil.XmlDoc, xmlDoc},
				{NHibernateUtil.XDoc, XDocument.Parse("<Root>XDoc</Root>")},
				{NHibernateUtil.Uri, new Uri("http://test.com")}
			};

			var sessionImpl = Substitute.For<ISessionImplementor>();
			sessionImpl.BestGuessEntityName(Arg.Any<object>()).Returns(o => o[0].GetType().Name);
			sessionImpl.GetContextEntityIdentifier(Arg.Is<object>(o => o is MyEntity)).Returns(o => ((MyEntity) o[0]).Id);
			return TypeHelper.Disassemble(
				                 types.Values.ToArray(),
				                 types.Keys.Cast<ICacheAssembler>().ToArray(),
				                 null,
				                 sessionImpl,
				                 null)
			                 .Concat(
				                 new[]
				                 {
					                 LazyPropertyInitializer.UnfetchedProperty,
					                 BackrefPropertyAccessor.Unknown
				                 })
			                 .ToArray();
		}

		private AnyType.ObjectTypeCacheEntry CreateObjectTypeCacheEntry()
		{
			return new AnyType.ObjectTypeCacheEntry
			{
				Id = 100,
				EntityName = "Test"
			};
		}

		private void CheckCacheEntry(CacheEntry original, CacheEntry copy)
		{
			Assert.That(copy.Version, Is.EqualTo(original.Version));
			Assert.That(copy.Version, Is.TypeOf(original.Version.GetType()));
			Assert.That(copy.Subclass, Is.EqualTo(original.Subclass));
			Assert.That(copy.AreLazyPropertiesUnfetched, Is.EqualTo(original.AreLazyPropertiesUnfetched));
			for (var i = 0; i < copy.DisassembledState.Length; i++)
			{
				Assert.That(copy.DisassembledState[i], Is.TypeOf(original.DisassembledState[i].GetType()));
				if (original.DisassembledState[i] is AnyType.ObjectTypeCacheEntry originalAnyType)
				{
					var copyAnyType = (AnyType.ObjectTypeCacheEntry) copy.DisassembledState[i];
					CheckObjectTypeCacheEntry(originalAnyType, copyAnyType);
				}
				else
				{
					Assert.That(copy.DisassembledState[i], Is.EqualTo(original.DisassembledState[i]));
				}
			}
		}

		private void CheckCachedItem(CachedItem original, CachedItem copy)
		{
			Assert.That(copy.Version, Is.EqualTo(original.Version));
			Assert.That(copy.Version, Is.TypeOf(original.Version.GetType()));
			Assert.That(copy.Value, Is.TypeOf(original.Value.GetType()));
			switch (original.Value)
			{
				case CacheEntry cacheEntry:
					CheckCacheEntry(cacheEntry, (CacheEntry) copy.Value);
					break;
				case CollectionCacheEntry colleectionCacheEntry:
					CheckCollectionCacheEntry(colleectionCacheEntry, (CollectionCacheEntry) copy.Value);
					break;
				default:
					Assert.That(copy.Value, Is.EqualTo(original.Value));
					break;
			}
			Assert.That(copy.FreshTimestamp, Is.EqualTo(original.FreshTimestamp));
		}

		private void CheckCollectionCacheEntry(CollectionCacheEntry original, CollectionCacheEntry copy)
		{
			Assert.That(copy.State, Is.TypeOf(original.State.GetType()));

			var originalArray = original.State;
			var copyArray = copy.State;

			for (var i = 0; i < copyArray.Length; i++)
			{
				Assert.That(copyArray[i], Is.TypeOf(originalArray[i].GetType()));
				if (originalArray[i] is AnyType.ObjectTypeCacheEntry originalAnyType)
				{
					var copyAnyType = (AnyType.ObjectTypeCacheEntry) copyArray[i];
					CheckObjectTypeCacheEntry(originalAnyType, copyAnyType);
				}
				else
				{
					Assert.That(copyArray[i], Is.EqualTo(originalArray[i]));
				}
			}
		}

		private void CheckCacheLock(CacheLock original, CacheLock copy)
		{
			Assert.That(copy.Version, Is.EqualTo(original.Version));
			Assert.That(copy.Version, Is.TypeOf(original.Version.GetType()));
			Assert.That(copy.Id, Is.EqualTo(original.Id));
			Assert.That(copy.Multiplicity, Is.EqualTo(original.Multiplicity));
			Assert.That(copy.Timeout, Is.EqualTo(original.Timeout));
			Assert.That(copy.UnlockTimestamp, Is.EqualTo(original.UnlockTimestamp));
			Assert.That(copy.WasLockedConcurrently, Is.EqualTo(original.WasLockedConcurrently));
		}

		private void CheckObjectTypeCacheEntry(AnyType.ObjectTypeCacheEntry original, AnyType.ObjectTypeCacheEntry copy)
		{
			Assert.That(copy.Id, Is.EqualTo(original.Id));
			Assert.That(copy.EntityName, Is.EqualTo(original.EntityName));
		}

		private static T TestDataContractSerializer<T>(T obj)
		{
			var xml = DataContractSerializerToXml(obj);
			obj = DataContractSerializerFromXml<T>(xml);
			Assert.That(xml, Is.EqualTo(DataContractSerializerToXml(obj)));
			return obj;
		}

		private static T TestBinaryFormatter<T>(T obj)
		{
			var bytes = BinaryFormatterToBinary(obj);
			obj = BinaryFormatterFromBinary<T>(bytes);
			Assert.That(bytes, Is.EqualTo(BinaryFormatterToBinary(obj)));
			return obj;
		}

		private static string DataContractSerializerToXml<T>(T obj)
		{
			using (var memoryStream = new MemoryStream())
			using (var reader = new StreamReader(memoryStream))
			{
				var serializer = new DataContractSerializer(typeof(T), KnownTypes);
				serializer.WriteObject(memoryStream, obj);
				memoryStream.Position = 0;
				return reader.ReadToEnd();
			}
		}

		private static T DataContractSerializerFromXml<T>(string xml)
		{
			using (var stream = new MemoryStream())
			{
				var data = Encoding.UTF8.GetBytes(xml);
				stream.Write(data, 0, data.Length);
				stream.Position = 0;
				var deserializer = new DataContractSerializer(typeof(T), KnownTypes);
				return (T) deserializer.ReadObject(stream);
			}
		}

		private static byte[] BinaryFormatterToBinary<T>(T obj)
		{
			var serializer = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				serializer.Serialize(stream, obj);
				return stream.ToArray();
			}
		}

		private static T BinaryFormatterFromBinary<T>(byte[] value)
		{
			var serializer = new BinaryFormatter();
			using (var stream = new MemoryStream(value))
			{
				return (T)serializer.Deserialize(stream);
			}
		}
	}
}
