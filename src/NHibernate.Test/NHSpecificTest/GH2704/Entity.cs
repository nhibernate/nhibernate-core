using System;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH2704
{
	public class Entity1
	{
		public virtual string Id { get; set; }
		public virtual bool IsChiusa { get; set; }
	}

	class Entity1Map : ClassMapping<Entity1>
	{
		public Entity1Map()
		{
			Table("TA");

			Id(x => x.Id);
			Property(x => x.IsChiusa, m => m.Type<StringBoolToBoolUserType>());
		}
	}
}
