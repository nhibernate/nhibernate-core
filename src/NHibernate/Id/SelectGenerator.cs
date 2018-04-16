using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Id.Insert;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Id
{
	/// <summary> 
	/// A generator that selects the just inserted row to determine the identifier
	/// value assigned by the database. The correct row is located using a unique key.
	/// </summary>
	/// <remarks>One mapping parameter is required: key (unless a natural-id is defined in the mapping).</remarks>
	public partial class SelectGenerator : AbstractPostInsertGenerator, IConfigurable
	{
		private string uniqueKeyPropertyName;

		#region Overrides of AbstractPostInsertGenerator

		public override IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(
			IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory, bool isGetGeneratedKeysEnabled)
		{
			return new SelectGeneratorDelegate(persister, factory, uniqueKeyPropertyName);
		}

		#endregion

		#region Implementation of IConfigurable

		public void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			parms.TryGetValue("key", out uniqueKeyPropertyName);
		}

		#endregion

		private static string DetermineNameOfPropertyToUse(IEntityPersister persister, string supplied)
		{
			if (supplied != null)
			{
				return supplied;
			}
			int[] naturalIdPropertyIndices = persister.NaturalIdentifierProperties;
			if (naturalIdPropertyIndices == null)
			{
				throw new IdentifierGenerationException("no natural-id property defined; need to specify [key] in "
				                                        + "generator parameters");
			}
			if (naturalIdPropertyIndices.Length > 1)
			{
				throw new IdentifierGenerationException("select generator does not currently support composite "
				                                        + "natural-id properties; need to specify [key] in generator parameters");
			}
			ValueInclusion inclusion = persister.PropertyInsertGenerationInclusions[naturalIdPropertyIndices[0]];
			if (inclusion != ValueInclusion.None)
			{
				throw new IdentifierGenerationException("natural-id also defined as insert-generated; need to specify [key] "
				                                        + "in generator parameters");
			}
			return persister.PropertyNames[naturalIdPropertyIndices[0]];
		}

		#region Nested type: SelectGeneratorDelegate

		/// <summary> The delegate for the select generation strategy.</summary>
		public partial class SelectGeneratorDelegate : AbstractSelectingDelegate
		{
			private readonly ISessionFactoryImplementor factory;
			private readonly SqlString idSelectString;
			private readonly IType idType;
			private readonly IPostInsertIdentityPersister persister;

			private readonly string uniqueKeyPropertyName;
			private readonly IType uniqueKeyType;

			internal SelectGeneratorDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory,
			                                 string suppliedUniqueKeyPropertyName) : base(persister)
			{
				this.persister = persister;
				this.factory = factory;
				uniqueKeyPropertyName = DetermineNameOfPropertyToUse((IEntityPersister) persister, suppliedUniqueKeyPropertyName);

				idSelectString = persister.GetSelectByUniqueKeyString(uniqueKeyPropertyName);
				uniqueKeyType = ((IEntityPersister) persister).GetPropertyType(uniqueKeyPropertyName);
				idType = persister.IdentifierType;
			}

			protected internal override SqlString SelectSQL
			{
				get { return idSelectString; }
			}

			protected internal override SqlType[] ParametersTypes
			{
				get { return uniqueKeyType.SqlTypes(factory); }
			}

			public override IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert()
			{
				return new IdentifierGeneratingInsert(factory);
			}

			protected internal override void BindParameters(ISessionImplementor session, DbCommand ps, object entity)
			{
				object uniqueKeyValue = ((IEntityPersister) persister).GetPropertyValue(entity, uniqueKeyPropertyName);
				uniqueKeyType.NullSafeSet(ps, uniqueKeyValue, 0, session);
			}

			protected internal override object GetResult(ISessionImplementor session, DbDataReader rs, object entity)
			{
				if (!rs.Read())
				{
					throw new IdentifierGenerationException("the inserted row could not be located by the unique key: "
					                                        + uniqueKeyPropertyName);
				}
				return idType.NullSafeGet(rs, persister.RootTableKeyColumnNames, session, entity);
			}
		}

		#endregion
	}
}