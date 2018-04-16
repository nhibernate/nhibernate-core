using System;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Id.Insert
{
	/// <summary> 
	/// <see cref="IInsertGeneratedIdentifierDelegate"/> implementation where the
	/// underlying strategy causes the generated identifier to be returned, as an
	/// effect of performing the insert statement, in a Output parameter.
	/// Thus, there is no need for an additional sql statement to determine the generated identifier. 
	/// </summary>
	public partial class OutputParamReturningDelegate : AbstractReturningDelegate
	{
		private const string ReturnParameterName = "nhIdOutParam";
		private readonly ISessionFactoryImplementor factory;
		private readonly string idColumnName;
		private readonly SqlType paramType;
		private string driveGeneratedParamName = ReturnParameterName;

		public OutputParamReturningDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory)
			: base(persister)
		{
			if (Persister.RootTableKeyColumnNames.Length > 1)
			{
				throw new HibernateException("identity-style generator cannot be used with multi-column keys");
			}
			paramType = Persister.IdentifierType.SqlTypes(factory)[0];
			idColumnName = Persister.RootTableKeyColumnNames[0];
			this.factory = factory;
		}

		#region Overrides of AbstractReturningDelegate

		public override IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert()
		{
			return new ReturningIdentifierInsert(factory, idColumnName, ReturnParameterName);
		}

		protected internal override DbCommand Prepare(SqlCommandInfo insertSQL, ISessionImplementor session)
		{
			var command = session.Batcher.PrepareCommand(CommandType.Text, insertSQL.Text, insertSQL.ParameterTypes);
			//Add the output parameter
			var idParameter = factory.ConnectionProvider.Driver.GenerateParameter(command, ReturnParameterName,
			                                                                                         paramType);
			driveGeneratedParamName = idParameter.ParameterName;

            if (factory.Dialect.InsertGeneratedIdentifierRetrievalMethod == InsertGeneratedIdentifierRetrievalMethod.OutputParameter)
                idParameter.Direction = ParameterDirection.Output;
            else if (factory.Dialect.InsertGeneratedIdentifierRetrievalMethod == InsertGeneratedIdentifierRetrievalMethod.ReturnValueParameter)
                idParameter.Direction = ParameterDirection.ReturnValue;
            else
                throw new System.NotImplementedException("Unsupported InsertGeneratedIdentifierRetrievalMethod: " + factory.Dialect.InsertGeneratedIdentifierRetrievalMethod);

			command.Parameters.Add(idParameter);
			return command;
		}

		public override object ExecuteAndExtract(DbCommand insert, ISessionImplementor session)
		{
			session.Batcher.ExecuteNonQuery(insert);
			return Convert.ChangeType(insert.Parameters[driveGeneratedParamName].Value, Persister.IdentifierType.ReturnedClass);
		}

		#endregion
	}
}
