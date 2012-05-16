using System.Collections.Generic;
using System.Data;
using NHibernate.Engine;
using NHibernate.Id.Insert;
using NHibernate.Persister.Collection;
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
	public class SelectGenerator : AbstractPostInsertGenerator, IConfigurable
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

		private static string[] DetermineNameOfPropertiesToUse(IEntityPersister persister, string supplied)
		{
			if (supplied != null)
			{
				return new string[] { supplied };
			}
			int[] naturalIdPropertyIndices = persister.NaturalIdentifierProperties;

			if (naturalIdPropertyIndices == null)
			{
				throw new IdentifierGenerationException("no natural-id property defined; need to specify [key] in "
				                                        + "generator parameters");
			}
			foreach (var naturalIdPropertyIndex in naturalIdPropertyIndices)
			{
				ValueInclusion inclusion = persister.PropertyInsertGenerationInclusions[naturalIdPropertyIndex];
				if (inclusion != ValueInclusion.None)
				{
					throw new IdentifierGenerationException("natural-id also defined as insert-generated; need to specify [key] "
															+ "in generator parameters");
				}
			}

			string[] result = new string[naturalIdPropertyIndices.Length];
			for (int i = 0; i < naturalIdPropertyIndices.Length; i++)
			{
				result[i] = persister.PropertyNames[naturalIdPropertyIndices[i]];
			}
			return result;
		}

		#region Nested type: SelectGeneratorDelegate

		/// <summary> The delegate for the select generation strategy.</summary>
		public class SelectGeneratorDelegate : AbstractSelectingDelegate
		{
			private readonly ISessionFactoryImplementor factory;
			private readonly SqlString idSelectString;
			private readonly IType idType;
			private readonly IPostInsertIdentityPersister persister;

			private readonly string[] uniqueKeyPropertyNames;
			private readonly IType[] uniqueKeyTypes;

			internal SelectGeneratorDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory,
			                                 string suppliedUniqueKeyPropertyName) : base(persister)
			{
				this.persister = persister;
				this.factory = factory;

				if (persister is IEntityPersister)
				{
					uniqueKeyPropertyNames = DetermineNameOfPropertiesToUse((IEntityPersister)persister, suppliedUniqueKeyPropertyName);					

					uniqueKeyTypes = new IType[uniqueKeyPropertyNames.Length];
					for (int i = 0; i < uniqueKeyPropertyNames.Length; i++)
					{
						uniqueKeyTypes[i] = ((IEntityPersister)persister).GetPropertyType(uniqueKeyPropertyNames[i]);
					}

				}
				else if (persister is AbstractCollectionPersister)
				{
					var collectionPersister = (AbstractCollectionPersister)persister;

					uniqueKeyPropertyNames = new string[2];
					uniqueKeyTypes = new IType[2];

					uniqueKeyPropertyNames[0] = collectionPersister.KeyColumnNames[0];
					uniqueKeyTypes[0] = collectionPersister.KeyType;

					uniqueKeyPropertyNames[1] = collectionPersister.ElementColumnNames[0];
					uniqueKeyTypes[1] = collectionPersister.ElementType;					
				}
				else 
				{
					throw new IdentifierGenerationException(string.Format("Persister if type {0} is not supported by Select Generator", persister.GetType()));
				}

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
					var sqlTypes = new List<SqlType>();
					foreach (var uniqueKeyType in uniqueKeyTypes)
						sqlTypes.AddRange(uniqueKeyType.SqlTypes(factory));

					return sqlTypes.ToArray();
				}
			}

			public override IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert()
			{
				return new IdentifierGeneratingInsert(factory);
			}

			protected internal override void BindParameters(ISessionImplementor session, IDbCommand ps, IBinder binder)
			{
				for (int i = 0; i < uniqueKeyPropertyNames.Length; i++)
				{
					if (persister is IEntityPersister)
					{
						object uniqueKeyValue = ((IEntityPersister)persister).GetPropertyValue(binder.Entity, uniqueKeyPropertyNames[i],
																								session.EntityMode);
						uniqueKeyTypes[i].NullSafeSet(ps, uniqueKeyValue, i, session);
					}
					else if (persister is AbstractCollectionPersister)
					{
						binder.BindValues(ps);
					}
				}
			}

			protected internal override object GetResult(ISessionImplementor session, IDataReader rs, object entity)
			{
				if (!rs.Read())
				{
					throw new IdentifierGenerationException("the inserted row could not be located by the unique key: "
															+ uniqueKeyPropertyNames);
				}
				return idType.NullSafeGet(rs, persister.RootTableKeyColumnNames, session, entity);
			}
		}

		#endregion
	}
}