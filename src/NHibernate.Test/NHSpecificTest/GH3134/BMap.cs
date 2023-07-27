using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3134
{
	public class BMap : ClassMapping<B>
	{
		public BMap()
		{
			this.Table("B");
			this.Id(x => x.Id, mapper => { mapper.Column("IDB"); mapper.Generator(Generators.Guid); });
			Property(x => x.NameB);
			this.Set(
				x => x.As,
				collectionMapping =>
				{
					collectionMapping.Table("AB");
					collectionMapping.Key(k => { k.Column("B"); k.ForeignKey("none"); });
					collectionMapping.Cascade(Mapping.ByCode.Cascade.All);
				},
				map => map.ManyToMany(m => { m.Column("A"); }));
		}
	}
}
