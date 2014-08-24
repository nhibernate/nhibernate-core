using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Id.Insert
{
	/// <summary> 
	/// Specialized IdentifierGeneratingInsert which appends the database
	/// specific clause which signifies to return generated identifier values
	/// to the end of the insert statement. 
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ReturningIdentifierInsert : NoCommentsInsert
	{
		private readonly string identifierColumnName;
		private readonly string returnParameterName;

		public ReturningIdentifierInsert(ISessionFactoryImplementor factory, string identifierColumnName,
		                                 string returnParameterName) : base(factory)
		{
			this.returnParameterName = returnParameterName;
			this.identifierColumnName = identifierColumnName;
		}

		public override SqlString ToSqlString()
		{
			return Dialect.AddIdentifierOutParameterToInsert(base.ToSqlString(), identifierColumnName, returnParameterName);
		}
	}
}