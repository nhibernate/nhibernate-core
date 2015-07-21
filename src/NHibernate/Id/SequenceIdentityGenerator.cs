using NHibernate.Engine;
using NHibernate.Id.Insert;

namespace NHibernate.Id
{
	/// <summary>
	/// A generator which combines sequence generation with immediate retrieval
	/// by attaching a output parameter to the SQL command
	/// In this respect it works much like ANSI-SQL IDENTITY generation.
	/// </summary>
	public class SequenceIdentityGenerator : SequenceGenerator, IPostInsertIdentifierGenerator
	{
		#region IPostInsertIdentifierGenerator Members

		public override object Generate(ISessionImplementor session, object obj)
		{
			return IdentifierGeneratorFactory.PostInsertIndicator;
		}

		#endregion

		#region Implementation of IPostInsertIdentifierGenerator

		public IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(IPostInsertIdentityPersister persister,
		                                                                               ISessionFactoryImplementor factory,
		                                                                               bool isGetGeneratedKeysEnabled)
		{
			return new SequenceIdentityDelegate(persister, factory, SequenceName);
		}

		#endregion

		#region Nested type: SequenceIdentityDelegate

		public class SequenceIdentityDelegate : OutputParamReturningDelegate
		{
			private readonly string sequenceNextValFragment;

			public SequenceIdentityDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory,
			                                string sequenceName) : base(persister, factory)
			{
				sequenceNextValFragment = factory.Dialect.GetSelectSequenceNextValString(sequenceName);
			}

			public override IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert()
			{
				IdentifierGeneratingInsert insert = base.PrepareIdentifierGeneratingInsert();
				insert.AddColumn(Persister.RootTableKeyColumnNames[0], sequenceNextValFragment);
				return insert;
			}
		}

		#endregion
	}
}