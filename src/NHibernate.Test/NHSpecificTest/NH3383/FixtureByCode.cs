using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3383
{
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}


		[Test]
		public void DeserializedCascadeStyleRefersToSameObject()
		{
			CascadeStyle deserializedCascadeStyle;

			using (var configMemoryStream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(configMemoryStream, CascadeStyle.Evict);
				configMemoryStream.Position = 0;
				deserializedCascadeStyle = (CascadeStyle) formatter.Deserialize(configMemoryStream);
			}

			Assert.That(deserializedCascadeStyle, Is.SameAs(CascadeStyle.Evict));
		}


		[Test]
		public void CanRoundTripSerializedMultipleCascadeStyle()
		{
			CascadeStyle startingCascadeStyle =
				new CascadeStyle.MultipleCascadeStyle(new[] {CascadeStyle.Delete, CascadeStyle.Lock});
			CascadeStyle deserializedCascadeStyle;

			using (var configMemoryStream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(configMemoryStream, startingCascadeStyle);
				configMemoryStream.Position = 0;
				deserializedCascadeStyle = (CascadeStyle)formatter.Deserialize(configMemoryStream);
			}

			Assert.That(deserializedCascadeStyle, Is.TypeOf<CascadeStyle.MultipleCascadeStyle>());
			Assert.That(deserializedCascadeStyle.ToString(),
			            Is.EqualTo(
				            "[NHibernate.Engine.CascadeStyle+DeleteCascadeStyle,NHibernate.Engine.CascadeStyle+LockCascadeStyle]"));
		}


		[Test]
		public void DeserializedPropertyMapping_RefersToSameCascadeStyle()
		{
			var classMapping = CreateMappingClasses();

			RootClass deserializedClassMapping;

			using (MemoryStream configMemoryStream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(configMemoryStream, classMapping);
				configMemoryStream.Position = 0;
				deserializedClassMapping = (RootClass)formatter.Deserialize(configMemoryStream);
			}

			AssertDeserializedMappingClasses(deserializedClassMapping);
		}

		// This test uses a seperate AppDomain to simulate the loading of a Configuration that was
		// serialized to the disk and is later deserialized in a new process.
		[Test]
		public void DeserializedPropertyMapping_CascadeStyleNotYetInitializedOnDeserialization_RefersToSameCascadeStyle()
		{
			var classMapping = CreateMappingClasses();

			using (MemoryStream configMemoryStream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(configMemoryStream, classMapping);
				configMemoryStream.Position = 0;

				var secondAppDomain = AppDomain.CreateDomain(
					"SecondAppDomain",
					null,
					AppDomain.CurrentDomain.SetupInformation);

				try
				{
					var helper = (AppDomainHelper)secondAppDomain.CreateInstanceAndUnwrap(
						Assembly.GetExecutingAssembly().FullName,
						typeof(AppDomainHelper).FullName);

					helper.DeserializeAndAssert(configMemoryStream);
				}
				finally
				{
					AppDomain.Unload(secondAppDomain);
				}
			}
		}

		private static RootClass CreateMappingClasses()
		{
			var classMapping = new RootClass();
			var componentMapping = new NHibernate.Mapping.Component(classMapping);

			var componentPropertyMapping = new Property(componentMapping);
			componentPropertyMapping.Name = "ComponentPropertyInClass";
			classMapping.AddProperty(componentPropertyMapping);

			var stringValue = new SimpleValue();
			stringValue.TypeName = typeof(string).FullName;

			var stringPropertyInComponentMapping = new Property(stringValue);
			stringPropertyInComponentMapping.Name = "StringPropertyInComponent";
			componentMapping.AddProperty(stringPropertyInComponentMapping);

			var componentType = (IAbstractComponentType)componentMapping.Type;

			Assume.That(CascadeStyle.None == stringPropertyInComponentMapping.CascadeStyle);
			Assume.That(CascadeStyle.None == componentType.GetCascadeStyle(0));
			Assume.That(CascadeStyle.None == componentPropertyMapping.CascadeStyle);

			return classMapping;
		}

		private static void AssertDeserializedMappingClasses(RootClass deserializedClassMapping)
		{
			var deserializedComponentPropertyMapping = deserializedClassMapping.GetProperty("ComponentPropertyInClass");
			var deserializedComponentMapping = (NHibernate.Mapping.Component)deserializedComponentPropertyMapping.Value;
			var deserializedComponentType = (IAbstractComponentType)deserializedComponentMapping.Type;
			var deserializedStringPropertyInComponentMapping = deserializedComponentMapping.GetProperty("StringPropertyInComponent");

			// Must be all the same objects since CascadeStyles are singletons and are
			// compared with "==" and "!=" operators.
			Assert.AreSame(CascadeStyle.None, deserializedStringPropertyInComponentMapping.CascadeStyle);
			Assert.AreSame(CascadeStyle.None, deserializedComponentType.GetCascadeStyle(0));
			Assert.AreSame(CascadeStyle.None, deserializedComponentPropertyMapping.CascadeStyle);
		}

		private sealed class AppDomainHelper : MarshalByRefObject
		{
			public void DeserializeAndAssert(MemoryStream configMemoryStream)
			{
				BinaryFormatter formatter = new BinaryFormatter();
				var deserializedClassMapping = (RootClass)formatter.Deserialize(configMemoryStream);

				AssertDeserializedMappingClasses(deserializedClassMapping);
			}
		}
	}
}