﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.Loader.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using System.Collections.Generic;
using NHibernate.SqlTypes;

namespace NHibernate.Persister.Collection
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class BasicCollectionPersister : AbstractCollectionPersister
	{

		protected override async Task<int> DoUpdateRowsAsync(object id, IPersistentCollection collection, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (ArrayHelper.IsAllFalse(elementColumnIsSettable)) return 0;

			try
			{
				DbCommand st = null;
				IExpectation expectation = Expectations.AppropriateExpectation(UpdateCheckStyle);
				//bool callable = UpdateCallable;
				bool useBatch = expectation.CanBeBatched;
				IEnumerable entries = collection.Entries(this);
				int i = 0;
				int count = 0;
				foreach (object entry in entries)
				{
					if (await (collection.NeedsUpdatingAsync(entry, i, ElementType, cancellationToken)).ConfigureAwait(false))
					{
						int offset = 0;
						if (useBatch)
						{
							if (st == null)
							{
								st =
									await (session.Batcher.PrepareBatchCommandAsync(SqlUpdateRowString.CommandType, SqlUpdateRowString.Text,
																		SqlUpdateRowString.ParameterTypes, cancellationToken)).ConfigureAwait(false);
							}
						}
						else
						{
							st =
								await (session.Batcher.PrepareCommandAsync(SqlUpdateRowString.CommandType, SqlUpdateRowString.Text,
															   SqlUpdateRowString.ParameterTypes, cancellationToken)).ConfigureAwait(false);
						}

						try
						{
							//offset += expectation.Prepare(st, Factory.ConnectionProvider.Driver);

							int loc = await (WriteElementAsync(st, collection.GetElement(entry), offset, session, cancellationToken)).ConfigureAwait(false);
							if (hasIdentifier)
							{
								await (WriteIdentifierAsync(st, collection.GetIdentifier(entry, i), loc, session, cancellationToken)).ConfigureAwait(false);
							}
							else
							{
								loc = await (WriteKeyAsync(st, id, loc, session, cancellationToken)).ConfigureAwait(false);
								if (HasIndex && !indexContainsFormula)
								{
									await (WriteIndexToWhereAsync(st, collection.GetIndex(entry, i, this), loc, session, cancellationToken)).ConfigureAwait(false);
								}
								else
								{
									// No nullness handled on update: updates does not occurs with sets or bags, and
									// indexed collections allowing formula (maps) force their element columns to
									// not-nullable.
									await (WriteElementToWhereAsync(st, collection.GetSnapshotElement(entry, i), null, loc, session, cancellationToken)).ConfigureAwait(false);
								}
							}

							if (useBatch)
							{
								await (session.Batcher.AddToBatchAsync(expectation, cancellationToken)).ConfigureAwait(false);
							}
							else
							{
								expectation.VerifyOutcomeNonBatched(await (session.Batcher.ExecuteNonQueryAsync(st, cancellationToken)).ConfigureAwait(false), st);
							}
						}
						catch (OperationCanceledException) { throw; }
						catch (Exception e)
						{
							if (useBatch)
							{
								session.Batcher.AbortBatch(e);
							}
							throw;
						}
						finally
						{
							if (!useBatch)
							{
								session.Batcher.CloseCommand(st, null);
							}
						}
						count++;
					}
					i++;
				}
				return count;
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(SQLExceptionConverter, sqle,
												 "could not update collection rows: " + MessageHelper.CollectionInfoString(this, collection, id, session),
												 SqlUpdateRowString.Text);
			}
		}
	}
}
