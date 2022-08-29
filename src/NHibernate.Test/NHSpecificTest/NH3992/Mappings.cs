using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Test.NHSpecificTest.NH3992.Longchain1;

namespace NHibernate.Test.NHSpecificTest.NH3992
{
	public class BaseEntityMapping : ClassMapping<BaseEntity>
	{
		public BaseEntityMapping()
		{
			Id(i => i.Id, m => m.Generator(Generators.GuidComb));
			Property(p => p.BaseField,
					 map => { map.Column("BaseField"); });
		}
	}

	public class MappedEntityJoinedSubclassMapping : JoinedSubclassMapping<MappedEntity>
	{
		public MappedEntityJoinedSubclassMapping()
		{
			Extends(typeof(BaseEntity));
			Property(p => p.ExtendedField,
					 map => { map.Column("ExtendedField"); });
			Property(p => p.TopLevelField,
					 map => { map.Column("TopLevelField"); });
		}
	}

	public class MappedEntityUnionSubclassMapping : UnionSubclassMapping<MappedEntity>
	{
		public MappedEntityUnionSubclassMapping()
		{
			Extends(typeof(BaseEntity));
			Property(p => p.ExtendedField,
					 map => { map.Column("ExtendedField"); });
			Property(p => p.TopLevelField,
					 map => { map.Column("TopLevelField"); });
		}
	}

	public class MappedEntitySubclassMapping : SubclassMapping<MappedEntity>
	{
		public MappedEntitySubclassMapping()
		{
			Extends(typeof(BaseEntity));
			Property(p => p.ExtendedField,
					 map => { map.Column("ExtendedField"); });
			Property(p => p.TopLevelField,
					 map => { map.Column("TopLevelField"); });
		}
	}

	public class BaseInterfaceMapping : ClassMapping<IBaseInterface>
	{
		public BaseInterfaceMapping()
		{
			Id(i => i.Id, m => m.Generator(Generators.GuidComb));
			Property(p => p.BaseField,
					 map => { map.Column("BaseField"); });
		}
	}

	public class MappedEntityFromInterfaceJoinedSubclassMapping : JoinedSubclassMapping<MappedEntityFromInterface>
	{
		public MappedEntityFromInterfaceJoinedSubclassMapping()
		{
			Extends(typeof(IBaseInterface));
			Property(p => p.ExtendedField,
					 map => { map.Column("ExtendedField"); });
			Property(p => p.TopLevelField,
					 map => { map.Column("TopLevelField"); });
		}
	}

	public class MappedEntityFromInterfaceUnionSubclassMapping : UnionSubclassMapping<MappedEntityFromInterface>
	{
		public MappedEntityFromInterfaceUnionSubclassMapping()
		{
			Extends(typeof(IBaseInterface));
			Property(p => p.ExtendedField,
					 map => { map.Column("ExtendedField"); });
			Property(p => p.TopLevelField,
					 map => { map.Column("TopLevelField"); });
		}
	}

	public class MappedEntityFromInterfaceSubclassMapping : SubclassMapping<MappedEntityFromInterface>
	{
		public MappedEntityFromInterfaceSubclassMapping()
		{
			Extends(typeof(IBaseInterface));
			Property(p => p.ExtendedField,
					 map => { map.Column("ExtendedField"); });
			Property(p => p.TopLevelField,
					 map => { map.Column("TopLevelField"); });
		}
	}

	// Mapped -> Unmapped -> Mapped -> TopLevel
	namespace Longchain1
	{
		public class MappedRootMapping : ClassMapping<MappedRoot>
		{
			public MappedRootMapping()
			{
				Id(i => i.Id, m => m.Generator(Generators.Identity));
				Property(p => p.BaseField,
						 map => { map.Column("BaseField"); });
			}
		}

		public class MappedExtensionMapping : SubclassMapping<MappedExtension>
		{
			public MappedExtensionMapping()
			{
				Extends(typeof(MappedRoot));

				Property(p => p.MappedExtensionField,
						 map => { map.Column("MappedExtensionField"); });
			}
		}

		public class TopLevelMapping : SubclassMapping<TopLevel>
		{
			public TopLevelMapping()
			{
				Extends(typeof(MappedExtension));
				Property(p => p.UnmappedExtensionField,
						 map => { map.Column("UnmappedExtensionField"); });
				Property(p => p.TopLevelExtensionField,
						 map => { map.Column("TopLevelExtensionField"); });
			}
		}
	}

	// Mapped -> Mapped -> Unmapped -> TopLevel
	namespace Longchain2
	{
		public class MappedRootMapping : ClassMapping<MappedRoot>
		{
			public MappedRootMapping()
			{
				Id(i => i.Id, m => m.Generator(Generators.Identity));
				Property(p => p.BaseField,
						 map => { map.Column("BaseField"); });
			}
		}

		public class MappedExtensionMapping : SubclassMapping<MappedExtension>
		{
			public MappedExtensionMapping()
			{
				Extends(typeof(MappedRoot));
				Property(p => p.UnmappedExtensionField,
						 map => { map.Column("UnmappedExtensionField"); });
				Property(p => p.MappedExtensionField,
						 map => { map.Column("MappedExtensionField"); });
			}
		}

		public class TopLevelMapping : SubclassMapping<TopLevel>
		{
			public TopLevelMapping()
			{
				Extends(typeof(MappedExtension));
				Property(p => p.TopLevelExtensionField,
						 map => { map.Column("TopLevelExtensionField"); });
			}
		}
	}

	// Mapped -> Unmapped -> Mapped -> Unmapped -> TopLevel
	namespace Longchain3
	{
		public class MappedRootMapping : ClassMapping<MappedRoot>
		{
			public MappedRootMapping()
			{
				Id(i => i.Id, m => m.Generator(Generators.Identity));
				Property(
					p => p.BaseField,
					map => { map.Column("BaseField"); });
			}
		}

		public class MappedExtensionMapping : SubclassMapping<MappedExtension>
		{
			public MappedExtensionMapping()
			{
				Extends(typeof(MappedRoot));
				Property(
					p => p.FirstUnmappedExtensionField,
					map => { map.Column("FirstUnmappedExtensionField"); });
				Property(
					p => p.MappedExtensionField,
					map => { map.Column("MappedExtensionField"); });
			}
		}

		public class TopLevelMapping : SubclassMapping<TopLevel>
		{
			public TopLevelMapping()
			{
				Extends(typeof(MappedExtension));
				Property(
					p => p.SecondUnmappedExtensionField,
					map => { map.Column("SecondUnmappedExtensionField"); });
				Property(
					p => p.TopLevelExtensionField,
					map => { map.Column("TopLevelExtensionField"); });
			}
		}
	}
}
