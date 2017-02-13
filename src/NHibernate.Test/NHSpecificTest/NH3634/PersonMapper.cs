using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.NH3634
{
	class PersonMapper : ClassMapping<Person>
	{
		public PersonMapper()
		{
			Id(p => p.Id, m => m.Generator(Generators.Identity));
			Lazy(false);
			Table("people");
			Property(p => p.Name);
			Component(
				p => p.Connection,
				m =>
				{
					m.Class<Connection>();
					m.Property(c => c.ConnectionType, mapper => mapper.NotNullable(true)); 
					m.Property(c => c.Address, mapper => mapper.NotNullable(false));
					m.Property(c => c.PortName, mapper => mapper.NotNullable(false));
				});
		}
	}
}