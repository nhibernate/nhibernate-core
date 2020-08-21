using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.NH3622
{
	public class Tag
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Equipment Equipment { get; set; }
	}

	public class Equipment
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Discipline Discipline { get; set; }
	}

	public class Discipline
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DisciplineType DisciplineType { get; set; }
	}

	public class DisciplineType
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}


	public class TagMap : ClassMapping<Tag>
	{
		public TagMap()
		{
			this.Id(x => x.Id, mapper => mapper.Generator(Generators.Assigned));
			this.Property(x => x.Name, mapper => mapper.NotNullable(true));
			this.ManyToOne(
				x => x.Equipment,
				mapper =>
				{
					mapper.NotNullable(true);
					mapper.Column("EquipmentId");
				});
		}
	}

	public class EquipmentMap : ClassMapping<Equipment>
	{
		public EquipmentMap()
		{
			this.Id(x => x.Id, mapper => mapper.Generator(Generators.Assigned));
			this.Property(x => x.Name, mapper => mapper.NotNullable(true));
			this.ManyToOne(
				x => x.Discipline,
				mapper =>
				{
					mapper.Column("DisciplineId");
					mapper.NotNullable(false);
				});
		}
	}

	public class DisciplineMap : ClassMapping<Discipline>
	{
		public DisciplineMap()
		{
			this.Id(x => x.Id, mapper => mapper.Generator(Generators.Assigned));
			this.Property(x => x.Name, mapper => mapper.NotNullable(true));
			this.ManyToOne(
				x => x.DisciplineType,
				mapper =>
				{
					mapper.Column("DisciplineTypeId");
					mapper.NotNullable(false);
				});
		}
	}

	public class DisciplineTypeMap : ClassMapping<DisciplineType>
	{
		public DisciplineTypeMap()
		{
			this.Id(x => x.Id, mapper => mapper.Generator(Generators.Assigned));
			this.Property(x => x.Name, mapper => mapper.NotNullable(true));
		}
	}
}
