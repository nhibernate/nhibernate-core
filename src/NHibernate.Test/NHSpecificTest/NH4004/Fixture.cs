using System.Data;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH4004
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2012Dialect;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(
						x => x.Id,
						m => m.Generator(
							Generators.Sequence,
							gm => gm.Params(
								new
								{
									sequence = "`entity_seq`"
								}
							)));
					rc.Property(x => x.Name);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override bool CheckDatabaseWasCleaned()
		{
			//NO OP
			return true;
		}

		[Test]
		public void SequenceShallBeDropped()
		{
			DropSchema();

			using (var connection = Sfi.ConnectionProvider.GetConnection())
			{
				var command = connection.CreateCommand();
				command.CommandText = "SELECT COUNT(*) FROM sys.sequences WHERE NAME = 'entity_seq'";
				command.CommandType = CommandType.Text;
				var count = (int) command.ExecuteScalar();

				Assert.That(count, Is.EqualTo(0));
			}
		}
	}
}
