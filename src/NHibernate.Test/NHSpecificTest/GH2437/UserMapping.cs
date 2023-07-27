using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH2437
{
	public class UserMapping : ClassMapping<User>
	{
		public UserMapping()
		{
			Table("Users");
			Id(o => o.UserCode, m => m.Generator(Generators.Assigned));
			Property(
				o => o.IsOpen,
				m =>
				{
					m.Type<SqlBoolean>();
					m.Precision(8);
				});
		}
	}
}
