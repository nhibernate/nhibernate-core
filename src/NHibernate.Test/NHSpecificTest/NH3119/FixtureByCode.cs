using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3119
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// This fixture is identical to <see cref="Fixture" /> except the <see cref="Entity" /> mapping is performed 
	/// by code in the GetMappings method, and does not require the <c>Mappings.hbm.xml</c> file. Use this approach
	/// if you prefer.
	/// </remarks>
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Component(x => x.Component, c =>
				{
					c.Property(x => x.Value, pmapper => pmapper.Column("`Value`"));
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			if (!Cfg.Environment.UseReflectionOptimizer)
			{
				Assert.Ignore("Test only works with reflection optimization enabled");
			}

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Name", Component = new Component { Value = "Value" } };
				session.Save(e1);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void PocoComponentTuplizer_Instantiate_UsesReflectonOptimizer()
		{
			using (ISession freshSession = OpenSession())
			using (freshSession.BeginTransaction())
			{
				Entity entity = freshSession.Query<Entity>().Single();

				string stackTrace = entity.Component.LastCtorStackTrace;

				StringAssert.Contains("NHibernate.Bytecode.Lightweight.ReflectionOptimizer.CreateInstance", stackTrace);
			}
		}

		[Test]
		public void PocoComponentTuplizerOfDeserializedConfiguration_Instantiate_UsesReflectonOptimizer()
		{
			MemoryStream configMemoryStream = new MemoryStream();
			BinaryFormatter writer = new BinaryFormatter();
			writer.Serialize(configMemoryStream, cfg);

			configMemoryStream.Seek(0, SeekOrigin.Begin);
			BinaryFormatter reader = new BinaryFormatter();
			Configuration deserializedConfig = (Configuration)reader.Deserialize(configMemoryStream);
			ISessionFactory factoryFromDeserializedConfig = deserializedConfig.BuildSessionFactory();

			using (ISession deserializedSession = factoryFromDeserializedConfig.OpenSession())
			using (deserializedSession.BeginTransaction())
			{
				Entity entity = deserializedSession.Query<Entity>().Single();

				string stackTrace = entity.Component.LastCtorStackTrace;

				StringAssert.Contains("NHibernate.Bytecode.Lightweight.ReflectionOptimizer.CreateInstance", stackTrace);
			}
		}
	}
}