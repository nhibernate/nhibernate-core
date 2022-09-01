using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3134
{
	public class AMap : ClassMapping<A>
	{
		public AMap()
		{
			this.Table("A");
			this.Id(x => x.Id, mapper => { mapper.Column("IDA"); mapper.Generator(Generators.Guid); });
			Property(x => x.NameA);
			this.Set(
				x => x.Bs,
				collectionMapping =>
				{
					collectionMapping.Table("AB");
					collectionMapping.Key(k => { k.Column("A"); k.ForeignKey("none"); });
					collectionMapping.Cascade(Mapping.ByCode.Cascade.All);
				},
				map => map.ManyToMany(m => { m.Column("B"); }));
		}
	}
}
