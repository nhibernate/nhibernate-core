using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Id.Insert
{
	/// <summary> 
	/// Specialized IdentifierGeneratingInsert which appends the database
	/// specific clause which signifies to return generated IDENTITY values
	/// to the end of the insert statement. 
	/// </summary>
	public class InsertSelectIdentityInsert : IdentifierGeneratingInsert
	{
		public InsertSelectIdentityInsert(ISessionFactoryImplementor factory)
			: base(factory)
		{
		}

		public override SqlString ToSqlString()
		{
			return Dialect.AppendIdentitySelectToInsert(base.ToSqlString());
		}
	}
}
