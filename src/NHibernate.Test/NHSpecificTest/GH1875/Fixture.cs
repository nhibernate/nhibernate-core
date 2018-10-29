﻿using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1875
{
	public class BadlyMappedEntity
	{
		public virtual Guid Id { get; set; }
		public virtual long FirstValue { get; set; }
		public virtual long SecondValue { get; set; }
	}

	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<BadlyMappedEntity>(
				ca =>
				{
					ca.Abstract(true);
					ca.Id(
						x => x.Id,
						map =>
						{
							map.Column("BadlyMappedEntityId");
							map.Generator(Generators.GuidComb);
						});
					ca.Property(x => x.FirstValue, map => map.Column("SameColumn"));
					// SecondValue is mapped with same name as another column, this gives the AbstractEntityMapper
					// more entries in the fields array than there are in the includeColumns array; this causes the
					// index to fall out of bounds.
					ca.Property(x => x.SecondValue, map => map.Column("SameColumn"));
				});

			return mapper.CompileMappingFor(new[] { typeof(BadlyMappedEntity) });
		}

		[Test]
		public void ShouldThrowSoundErrorForBadlyMappedEntity()
		{
			var bad = new BadlyMappedEntity
			{
				FirstValue = 1,
				SecondValue = 2
			};

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(bad);
				Assert.That(
					transaction.Commit,
					Throws.TypeOf<PropertyValueException>().And.Message.Contains("(Duplicate column mapping?)"));
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
				transaction.Commit();
			}
		}
	}
}
