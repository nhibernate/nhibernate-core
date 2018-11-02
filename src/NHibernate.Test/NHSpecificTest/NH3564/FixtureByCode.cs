﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH3564
{
	public partial class MyDummyCache : CacheBase
	{
		private IDictionary hashtable = new Hashtable();
		private readonly string regionName;

		public MyDummyCache(string regionName)
		{
			this.regionName = regionName;
		}

		public override bool PreferMultipleGet => false;

		public override object Get(object key)
		{
			return hashtable[KeyAsString(key)];
		}

		public override void Put(object key, object value)
		{
			hashtable[KeyAsString(key)] = value;
		}

		public override void Remove(object key)
		{
			hashtable.Remove(KeyAsString(key));
		}

		public override void Clear()
		{
			hashtable.Clear();
		}

		public override void Destroy()
		{
		}

		public override object Lock(object key)
		{
			// local cache, so we use synchronization
			return null;
		}

		public override void Unlock(object key, object lockValue)
		{
			// local cache, so we use synchronization
		}

		public override long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public override int Timeout
		{
			get { return Timestamper.OneMs*60000; }
		}

		public override string RegionName
		{
			get { return regionName; }
		}

		private string KeyAsString(object key)
		{
			//This is how MemCached provider uses key.
			return string.Format("{0}@{1}", RegionName, (key == null ? string.Empty : key.ToString()));
		}
	}

	public class MyDummyCacheProvider : ICacheProvider
	{
		// Since 5.2
		[Obsolete]
		ICache ICacheProvider.BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return BuildCache(regionName, properties);
		}

		public CacheBase BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return new MyDummyCache(regionName);
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public void Start(IDictionary<string, string> properties)
		{
		}

		public void Stop()
		{
		}
	}

	class Person
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime DateOfBirth { get; set; }
	}

	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.DateOfBirth, pm =>
				{
					pm.Type(NHibernateUtil.DateTime);
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.CacheProvider, typeof (MyDummyCacheProvider).AssemblyQualifiedName);
			configuration.SetProperty(Environment.UseQueryCache, "true");
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Person {Name = "Bob", DateOfBirth = new DateTime(2015, 4, 22)});
				session.Save(new Person {Name = "Sally", DateOfBirth = new DateTime(2014, 4, 22)});

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void ShouldUseDifferentCache()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var bob = session.Query<Person>()
					.WithOptions(o => o.SetCacheable(true))
					.Where(e => e.DateOfBirth == new DateTime(2015, 4, 22))
					.ToList();
				var sally = session.Query<Person>()
					.WithOptions(o => o.SetCacheable(true))
					.Where(e => e.DateOfBirth == new DateTime(2014, 4, 22))
					.ToList();

				Assert.That(bob, Has.Count.EqualTo(1));
				Assert.That(bob[0].Name, Is.EqualTo("Bob"));

				Assert.That(sally, Has.Count.EqualTo(1));
				Assert.That(sally[0].Name, Is.EqualTo("Sally"));
			}
		}
	}
}
