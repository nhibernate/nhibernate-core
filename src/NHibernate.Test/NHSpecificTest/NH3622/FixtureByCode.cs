using System;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3622
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<TagMap>();
			mapper.AddMapping<EquipmentMap>();
			mapper.AddMapping<DisciplineMap>();
			mapper.AddMapping<DisciplineTypeMap>();
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void MissingJoinsInSubquery()
		{
			var id = Guid.NewGuid();
			using( var logSpy = new SqlLogSpy())
			using (var s = OpenSession())
			{
				var x = s.Query<Tag>()
						.Where(tag => tag.Equipment.Discipline.DisciplineType.Id == id)
						.Select(tag => tag.Id);

				var y = s.Query<Tag>()
						.Where(tag => x.Contains(tag.Id))
						.Fetch(tag => tag.Equipment)
						.ThenFetch(equipment => equipment.Discipline)
						.ThenFetch(discipline => discipline.DisciplineType)
						.ToList();

				var sql = logSpy.GetWholeLog();
				var findSubqyeryIndex = sql.IndexOf(" in (");
				var capturesCount = Regex.Matches(sql.Substring(findSubqyeryIndex), "inner join").Count;
				//Expected joins for tag.Equipment.Discipline in subquery
				Assert.That(capturesCount, Is.EqualTo(2), "Missing inner joins in subquery: " + sql);
			}
		}
	}
}
