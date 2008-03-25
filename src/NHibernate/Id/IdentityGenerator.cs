using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Id.Insert;
using NHibernate.SqlCommand;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that indicates to the <see cref="ISession"/> that identity
	/// (ie. identity/autoincrement column) key generation should be used.
	/// </summary>
	/// <remarks>
	/// <p>
	///	This id generation strategy is specified in the mapping file as 
	///	<code>&lt;generator class="identity" /&gt;</code> 
	///	or if the database natively supports identity columns 
	///	<code>&lt;generator class="native" /&gt;</code>
	/// </p>
	/// <p>
	/// This indicates to NHibernate that the database generates the id when
	/// the entity is inserted.
	/// </p>
	/// </remarks>
	public class IdentityGenerator : AbstractPostInsertGenerator
	{
		public override IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(
			IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory, bool isGetGeneratedKeysEnabled)
		{
			if (isGetGeneratedKeysEnabled)
			{
				throw new NotSupportedException();
			}
			else if (factory.Dialect.SupportsInsertSelectIdentity)
			{
				return new InsertSelectDelegate(persister, factory);
			}
			else
			{
				return new BasicDelegate(persister, factory);
			}
		}

		/// <summary> 
		/// Delegate for dealing with IDENTITY columns where the dialect supports returning
		/// the generated IDENTITY value directly from the insert statement.
		/// </summary>
		public class InsertSelectDelegate : AbstractReturningDelegate, IInsertGeneratedIdentifierDelegate
		{
			private readonly IPostInsertIdentityPersister persister;
			private readonly ISessionFactoryImplementor factory;

			public InsertSelectDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory)
				: base(persister)
			{
				this.persister = persister;
				this.factory = factory;
			}

			public override IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert()
			{
				InsertSelectIdentityInsert insert = new InsertSelectIdentityInsert(factory);
				insert.AddIdentityColumn(persister.RootTableKeyColumnNames[0]);
				return insert;
			}

			protected internal override IDbCommand Prepare(SqlCommandInfo insertSQL, ISessionImplementor session)
			{
				return session.Batcher.PrepareCommand(CommandType.Text, insertSQL.Text, insertSQL.ParameterTypes);
			}

			public override object ExecuteAndExtract(IDbCommand insert, ISessionImplementor session)
			{
				IDataReader rs = session.Batcher.ExecuteReader(insert);
				try
				{
					return IdentifierGeneratorFactory.GetGeneratedIdentity(rs, persister.IdentifierType, session);
				}
				finally
				{
					session.Batcher.CloseReader(rs);
				}
			}

			public object DetermineGeneratedIdentifier(ISessionImplementor session, object entity)
			{
				throw new AssertionFailure("insert statement returns generated value");
			}
		}

		/// <summary> 
		/// Delegate for dealing with IDENTITY columns where the dialect requires an
		/// additional command execution to retrieve the generated IDENTITY value
		/// </summary>
		public class BasicDelegate : AbstractSelectingDelegate, IInsertGeneratedIdentifierDelegate
		{

			private readonly IPostInsertIdentityPersister persister;
			private readonly ISessionFactoryImplementor factory;

			public BasicDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory)
				: base(persister)
			{
				this.persister = persister;
				this.factory = factory;
			}

			protected internal override SqlString SelectSQL
			{
				get { return new SqlString(persister.IdentitySelectString); }
			}

			public override IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert()
			{
				IdentifierGeneratingInsert insert = new IdentifierGeneratingInsert(factory);
				insert.AddIdentityColumn(persister.RootTableKeyColumnNames[0]);
				return insert;
			}

			protected internal override object GetResult(ISessionImplementor session, IDataReader rs, object obj)
			{
				return IdentifierGeneratorFactory.GetGeneratedIdentity(rs, persister.IdentifierType, session);
			}
		}
	}
}