using System;
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

		#region Nested type: SelectGeneratorDelegate

		/// <summary> The delegate for the select generation strategy.</summary>
		public partial class SelectGeneratorDelegate : AbstractSelectingDelegate
		{
			private readonly ISessionFactoryImplementor factory;
			private readonly SqlString idSelectString;
			private readonly IType idType;
			private readonly IPostInsertIdentityPersister persister;

			private readonly string[] uniqueKeySuppliedPropertyNames;
			private readonly IType[] uniqueKeyTypes;

			internal SelectGeneratorDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory,
			                                 string suppliedUniqueKeyPropertyNames) : base(persister)
			{
				this.persister = persister;
				this.factory = factory;

				uniqueKeySuppliedPropertyNames = suppliedUniqueKeyPropertyNames?.Split(',').Select(p => p.Trim()).ToArray();

				idSelectString = persister.GetSelectByUniqueKeyString(uniqueKeySuppliedPropertyNames, out uniqueKeyTypes);
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

			// Since 5.2
			[Obsolete("Use or override BindParameters(ISessionImplementor session, DbCommand ps, IBinder binder) instead.")]
			protected internal override void BindParameters(ISessionImplementor session, DbCommand ps, object entity)
			{
				var entityPersister = (IEntityPersister) persister;
				var uniqueKeyPropertyNames = uniqueKeySuppliedPropertyNames ??
					PostInsertIdentityPersisterExtension.DetermineNameOfPropertiesToUse(entityPersister);
				for (var i = 0; i < uniqueKeyPropertyNames.Length; i++)
				{
					var uniqueKeyValue = entityPersister.GetPropertyValue(entity, uniqueKeyPropertyNames[i]);
					uniqueKeyTypes[i].NullSafeSet(ps, uniqueKeyValue, i, session);
				}
			}

			protected internal override void BindParameters(ISessionImplementor session, DbCommand ps, IBinder binder)
			{
				// 6.0 TODO: remove the "if" block and do the other TODO of this method
				if (persister is IEntityPersister)
				{
#pragma warning disable 618
					BindParameters(session, ps, binder.Entity);
#pragma warning restore 618
					return;
				}

				if (persister is ICompositeKeyPostInsertIdentityPersister compositeKeyPersister)
				{
					compositeKeyPersister.BindSelectByUniqueKey(session, ps, binder, uniqueKeySuppliedPropertyNames);
					return;
				}

				// 6.0 TODO: remove by merging ICompositeKeyPostInsertIdentityPersister in IPostInsertIdentityPersister
				binder.BindValues(ps);
			}

			protected internal override object GetResult(ISessionImplementor session, DbDataReader rs, object entity)
			{
				if (!rs.Read())
				{
					var message = "The inserted row could not be located by the unique key";
					if (uniqueKeySuppliedPropertyNames != null)
						message = $"{message} (supplied unique key: {string.Join(", ", uniqueKeySuppliedPropertyNames)})";
					throw new IdentifierGenerationException(message);
				}
				return idType.NullSafeGet(rs, persister.RootTableKeyColumnNames, session, entity);
			}
		}

		#endregion
	}
}
