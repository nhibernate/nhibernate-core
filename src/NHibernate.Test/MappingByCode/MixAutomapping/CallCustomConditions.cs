using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class CallCustomConditions
	{
		public enum ActivityType
		{
			Form = 1,
			Custom = 2,
			Email = 3
		}

		private class Activity : Entity
		{
			public virtual string Title { get; set; }
		}

		private interface IEntity<T>
		{
			T Id { get; }
		}

		private class Entity : IEntity<Guid>
		{
			#region IEntity<Guid> Members

			public virtual Guid Id { get; private set; }

			#endregion
		}

		private class FormActivity : Activity { }

		[Test]
		public void WhenCustomizeConditionsThenUseCustomConditionsToRecognizeRootEntities()
		{
			System.Type baseEntityType = typeof(Entity);
			var inspector = new SimpleModelInspector();
			inspector.IsEntity((t, declared) => baseEntityType.IsAssignableFrom(t) && baseEntityType != t && !t.IsInterface);
			inspector.IsRootEntity((t, declared) => baseEntityType == t.BaseType);

			var mapper = new ModelMapper(inspector);
			mapper.Class<Entity>(map => map.Id(x => x.Id,
																				 m =>
																				 {
																					 m.Generator(Generators.Guid);
																					 m.Column("ID");
																				 }));

			mapper.Class<Activity>(map =>
			{
				map.Discriminator(dm =>
				{
					dm.Column("DISCRIMINATOR_TYPE");
					dm.NotNullable(true);
				});
				map.DiscriminatorValue(0);
			});
			mapper.Subclass<FormActivity>(map => map.DiscriminatorValue(1));

			mapper.Executing(m=> m.CompileMappingFor(new[] { typeof(Activity), typeof(FormActivity) })).NotThrows();
		}
	}
}