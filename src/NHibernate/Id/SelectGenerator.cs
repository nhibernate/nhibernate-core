using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
		private string uniqueKeyPropertyNames;

		#region Overrides of AbstractPostInsertGenerator

		public override IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(
			IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory, bool isGetGeneratedKeysEnabled)
		{
			return new SelectGeneratorDelegate(persister, factory, uniqueKeyPropertyNames);
		}

		#endregion

		#region Implementation of IConfigurable

		public void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			parms.TryGetValue("key", out uniqueKeyPropertyNames);
		}

		#endregion

		private static string[] DetermineNameOfPropertiesToUse(IEntityPersister persister, string supplied)
		{
			if (supplied != null)
			{
				return supplied.Split(',').Select(p => p.Trim()).ToArray();
			}

			var naturalIdPropertyIndices = persister.NaturalIdentifierProperties;
			if (naturalIdPropertyIndices == null)
			{
				throw new IdentifierGenerationException(
					"no natural-id property defined; need to specify [key] in generator parameters");
			}

			foreach (var naturalIdPropertyIndex in naturalIdPropertyIndices)
			{
				var inclusion = persister.PropertyInsertGenerationInclusions[naturalIdPropertyIndex];
				if (inclusion != ValueInclusion.None)
				{
					throw new IdentifierGenerationException(
						"natural-id also defined as insert-generated; need to specify [key] in generator parameters");
				}
			}

			var result = new string[naturalIdPropertyIndices.Length];
			for (var i = 0; i < naturalIdPropertyIndices.Length; i++)
			{
				result[i] = persister.PropertyNames[naturalIdPropertyIndices[i]];
			}
			return result;
		}

		#region Nested type: SelectGeneratorDelegate

		/// <summary> The delegate for the select generation strategy.</summary>
		public partial class SelectGeneratorDelegate : AbstractSelectingDelegate
		{
			private readonly ISessionFactoryImplementor factory;
			private readonly SqlString idSelectString;
			private readonly IType idType;
			private readonly IPostInsertIdentityPersister persister;
			private readonly IEntityPersister entityPersister;

			private readonly string[] uniqueKeyPropertyNames;
			private readonly IType[] uniqueKeyTypes;

			internal SelectGeneratorDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory,
			                                 string suppliedUniqueKeyPropertyNames) : base(persister)
			{
				this.persister = persister;
				this.factory = factory;

				entityPersister = (IEntityPersister) persister;
				uniqueKeyPropertyNames = DetermineNameOfPropertiesToUse(entityPersister, suppliedUniqueKeyPropertyNames);

				uniqueKeyTypes = uniqueKeyPropertyNames.Select(p => entityPersister.GetPropertyType(p)).ToArray();

				idSelectString = persister.GetSelectByUniqueKeyString(uniqueKeyPropertyNames);
				idType = persister.IdentifierType;
			}

			protected internal override SqlString SelectSQL
			{
				get { return idSelectString; }
			}

			protected internal override SqlType[] ParametersTypes
			{
				get
				{
					return uniqueKeyTypes.SelectMany(t => t.SqlTypes(factory)).ToArray();
				}
			}

			public override IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert()
			{
				return new IdentifierGeneratingInsert(factory);
			}

			protected internal override void BindParameters(ISessionImplementor session, DbCommand ps, object entity)
			{
				for (var i = 0; i < uniqueKeyPropertyNames.Length; i++)
				{
					var uniqueKeyValue = entityPersister.GetPropertyValue(entity, uniqueKeyPropertyNames[i]);
					uniqueKeyTypes[i].NullSafeSet(ps, uniqueKeyValue, i, session);
				}
			}

			protected internal override object GetResult(ISessionImplementor session, DbDataReader rs, object entity)
			{
				if (!rs.Read())
				{
					throw new IdentifierGenerationException(
						$"The inserted row could not be located by the unique key: {string.Join(", ", uniqueKeyPropertyNames)}");
				}
				return idType.NullSafeGet(rs, persister.RootTableKeyColumnNames, session, entity);
			}
		}

		#endregion
	}
}
