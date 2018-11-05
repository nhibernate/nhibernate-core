using System;
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
		private readonly string _identifierColumnName;

		//Since v5.2
		[Obsolete("Please use constructor accepting identifierColumnName parameter.")]
		public InsertSelectIdentityInsert(ISessionFactoryImplementor factory)
			: base(factory)
		{
		}

		public InsertSelectIdentityInsert(ISessionFactoryImplementor factory, string identifierColumnName)
			: base(factory)
		{
			_identifierColumnName = identifierColumnName;
		}

		public override SqlString ToSqlString()
		{
			return Dialect.AppendIdentitySelectToInsert(base.ToSqlString(), _identifierColumnName);
		}
	}
}
