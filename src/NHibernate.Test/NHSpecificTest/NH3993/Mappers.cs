﻿using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.NH3993
{
	public class BaseEntityMapper: ClassMapping<BaseEntity>
	{
		public BaseEntityMapper()
		{
			Component(c => c.Component,
				m =>
				{
					m.Parent("_parent",
						p => p.Access(Accessor.NoSetter));
					m.Map<string, Element>("_elements",
						map => map.Table("Elements"),
						key => key.Element(element => element.Formula("Key")),
						rel => rel.Component(e =>
						{
							e.Property("_name",
								p => p.Column("Name"));
							e.Parent("_parent",
								p => p.Access(Accessor.NoSetter));
							e.ManyToOne<string>("_description", r => r.Column("Desc"));
						}));
				});
		}
	}

	public class InvalidPropertyMapper : ClassMapping<BaseEntity>
	{
		public InvalidPropertyMapper()
		{
			Component(c => c.Component,
				m =>
				{
					m.Map<string, Element>("_elements",
						map => map.Table("Elements"),
						key => key.Element(element => element.Formula("Key")),
						rel => rel.Component(e =>
						{
							e.Property("Non existent field",
								p => p.Column("Name"));
						}));
				});
		}
	}

	public class InvalidRelationshipMapper : ClassMapping<BaseEntity>
	{
		public InvalidRelationshipMapper()
		{
			Component(c => c.Component,
				m =>
				{
					m.Map<string, Element>("_elements",
						map => map.Table("Elements"),
						key => key.Element(element => element.Formula("Key")),
						rel => rel.Component(e =>
						{
							e.ManyToOne<Element>("_description", r => r.Column("Desc"));
						}));
				});
		}
	}
}
