﻿using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Async.NHSpecificTest.MultipleFieldsMappedToOneColumn
{
	public class BadlyMappedEntity
	{
		public virtual long Id { get; set; }
		public virtual long FirstValue { get; set; }
		public virtual long SecondValue {get; set; }
	}

	public class CorrectlyMappedEntity
	{
		public virtual long Id { get; set; }
		public virtual long FirstValue { get; set; }
		public virtual long SecondValue {get; set; }
	}

	[TestFixture]
	public class FixtureAsync : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<BadlyMappedEntity>(ca =>
			{
				ca.Abstract(true);
				ca.Id(x => x.Id, map =>
				{
					map.Column("BadlyMappedEntityId");
					map.Generator(Generators.Native);
				});
				ca.Property(x => x.FirstValue, map => map.Column("SameColumn"));
				ca.Property(x => x.SecondValue, map => map.Column("SameColumn"));
				// SecondValue is mapped badly, this gives the AbstractEntityMapper more entries in the fields array
				// than there are in the includeColumns array; this causes the index to fall out of bounds.
			});

			mapper.Class<CorrectlyMappedEntity>(ca =>
			{
				ca.Abstract(true);
				ca.Id(x => x.Id, map =>
				{
					map.Column("BadlyMappedEntityId");
					map.Generator(Generators.Native);
				});
				ca.Property(x => x.FirstValue, map => map.Column("SameColumn"));
				ca.Property(x => x.SecondValue, map => map.Column("OtherColumn"));
				// SecondValue is mapped correctly, should not break
			});
			
			return mapper.CompileMappingFor(new[] { typeof(BadlyMappedEntity), typeof(CorrectlyMappedEntity) });
		}

		[Test]
		public void ShouldThrowAREXForBadlyMappedEntity()
		{
			// arrange
			ArgumentOutOfRangeException result = null;
			var e = new BadlyMappedEntity
			{
				FirstValue = 1,
				SecondValue = 2
			};

			// act
			try
			{
				using (var session = OpenSession())
				using (var transaction = session.BeginTransaction())
				{
					
					session.Save(e);
					transaction.Commit();
				}
			}
			catch (ArgumentOutOfRangeException arex)
			{
				result = arex;
			}

			// assert
			Assert.That(result != null);
			Assert.That(result.Message.Contains("(Duplicate column mapping?)"));
		}

		[Test]
		public void ShouldNotThrow()
		{
			// arrange
			ArgumentOutOfRangeException result = null;
			var e = new BadlyMappedEntity
			{
				FirstValue = 1,
				SecondValue = 2
			};

			// act
			try
			{
				using (var session = OpenSession())
				using (var transaction = session.BeginTransaction())
				{
					
					session.Save(e);
					transaction.Commit();
				}
			}
			catch (ArgumentOutOfRangeException arex)
			{
				result = arex;
			}

			// assert
			Assert.That(result == null);
			Assert.That(e.Id > 0);

			// cleanup after Test
			OpenSession().Delete(e);
		}
	}
}
