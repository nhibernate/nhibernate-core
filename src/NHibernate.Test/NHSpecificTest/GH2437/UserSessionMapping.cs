using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH2437
{
	public class UserSessionMapping : ClassMapping<UserSession>
	{
		public UserSessionMapping()
		{
			Table("UserSessions");
			Id(o => o.Guid, m => m.Generator(Generators.Native));
			Property(o => o.UserCode, m => m.NotNullable(true));
			Property(o => o.OpenDate, m =>
			{
				m.Type<SqlNumberDate>();
				m.Precision(8);
			});
			Property(o => o.ExpireDateTime, m =>
			{
				m.Type<SqlNumberDateTime>();
				m.Precision(17);
			});
			Property(o => o.IsOpen, m =>
			{
				m.Type<SqlBoolean>();
				m.Precision(8);
			});
			ManyToOne(
				o => o.User,
				m =>
				{
					m.Column("UserCode");
					m.Insert(false);
					m.Update(false);
				});
		}
	}
}
