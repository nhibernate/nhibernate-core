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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Id.Insert;
using NHibernate.Impl;
using NHibernate.Loader.Collection;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using Array = NHibernate.Mapping.Array;

namespace NHibernate.Persister.Collection
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class AbstractCollectionPersister : 
		ICollectionMetadata, 
		ISqlLoadableCollection,
		IPostInsertIdentityPersister, 
		ISupportSelectModeJoinable, 
		ICompositeKeyPostInsertIdentityPersister, 
		ISupportLazyPropsJoinable,
		IPersister
	{

		public Task InitializeAsync(object key, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return GetAppropriateInitializer(key, session).InitializeAsync(key, session, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// Reads the Element from the DbDataReader.  The DbDataReader will probably only contain
		/// the id of the Element.
		/// </summary>
		/// <remarks>See ReadElementIdentifier for an explanation of why this method will be depreciated.</remarks>
		public Task<object> ReadElementAsync(DbDataReader rs, object owner, string[] aliases, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return ElementType.NullSafeGetAsync(rs, aliases, session, owner, cancellationToken);
		}

		public async Task<object> ReadIndexAsync(DbDataReader rs, string[] aliases, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object index = await (IndexType.NullSafeGetAsync(rs, aliases, session, null, cancellationToken)).ConfigureAwait(false);
			if (index == null)
			{
				throw new HibernateException("null index column for collection: " + role);
			}
			index = DecrementIndexByBase(index);
			return index;
		}

		public async Task<object> ReadIdentifierAsync(DbDataReader rs, string alias, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object id = await (IdentifierType.NullSafeGetAsync(rs, alias, session, null, cancellationToken)).ConfigureAwait(false);
			if (id == null)
			{
				throw new HibernateException("null identifier column for collection: " + role);
			}

			return id;
		}

		public Task<object> ReadKeyAsync(DbDataReader dr, string[] aliases, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return KeyType.NullSafeGetAsync(dr, aliases, session, null, cancellationToken);
		}

		protected Task<int> WriteKeyAsync(DbCommand st, object id, int i, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id", "Null key for collection: " + role);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			return InternalWriteKeyAsync();
			async Task<int> InternalWriteKeyAsync()
			{

				await (KeyType.NullSafeSetAsync(st, id, i, session, cancellationToken)).ConfigureAwait(false);
				return i + keyColumnAliases.Length;
			}
		}

		protected async Task<int> WriteElementAsync(DbCommand st, object elt, int i, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await (ElementType.NullSafeSetAsync(st, elt, i, elementColumnIsSettable, session, cancellationToken)).ConfigureAwait(false);
			return i + ArrayHelper.CountTrue(elementColumnIsSettable);
		}

		protected async Task<int> WriteIndexAsync(DbCommand st, object idx, int i, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await (IndexType.NullSafeSetAsync(st, IncrementIndexByBase(idx), i, indexColumnIsSettable, session, cancellationToken)).ConfigureAwait(false);
			return i + ArrayHelper.CountTrue(indexColumnIsSettable);
		}

		protected Task<int> WriteElementToWhereAsync(DbCommand st, object elt, bool[] columnNullness, int i, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (elementIsPureFormula)
			{
				throw new AssertionFailure("cannot use a formula-based element in the where condition");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			return InternalWriteElementToWhereAsync();
			async Task<int> InternalWriteElementToWhereAsync()
			{

				var settable = Combine(elementColumnIsInPrimaryKey, columnNullness);

				await (ElementType.NullSafeSetAsync(st, elt, i, settable, session, cancellationToken)).ConfigureAwait(false);
				return i + settable.Count(s => s);
			}
		}

		// Since v5.2
		[Obsolete("Use overload with columnNullness instead")]
		protected Task<int> WriteElementToWhereAsync(DbCommand st, object elt, int i, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			return WriteElementToWhereAsync(st, elt, null, i, session, cancellationToken);
		}

		// No column nullness handling here: although a composite index could have null columns, the mapping
		// current implementation forbirds this by forcing not-null to true on all columns.
		protected Task<int> WriteIndexToWhereAsync(DbCommand st, object index, int i, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (indexContainsFormula)
			{
				throw new AssertionFailure("cannot use a formula-based index in the where condition");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			return InternalWriteIndexToWhereAsync();
			async Task<int> InternalWriteIndexToWhereAsync()
			{

				await (IndexType.NullSafeSetAsync(st, IncrementIndexByBase(index), i, session, cancellationToken)).ConfigureAwait(false);
				return i + indexColumnAliases.Length;
			}
		}

		protected async Task<int> WriteIdentifierAsync(DbCommand st, object idx, int i, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await (IdentifierType.NullSafeSetAsync(st, idx, i, session, cancellationToken)).ConfigureAwait(false);
			return i + 1;
		}

		public async Task RemoveAsync(object id, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!isInverse && RowDeleteEnabled)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Deleting collection: {0}", MessageHelper.CollectionInfoString(this, id, Factory));
				}

				// Remove all the old entries
				try
				{
					int offset = 0;
					IExpectation expectation = Expectations.AppropriateExpectation(DeleteAllCheckStyle);
					//bool callable = DeleteAllCallable;
					bool useBatch = expectation.CanBeBatched;
					var st = useBatch
						? await (session.Batcher.PrepareBatchCommandAsync(SqlDeleteString.CommandType, SqlDeleteString.Text, SqlDeleteString.ParameterTypes, cancellationToken)).ConfigureAwait(false)
						: await (session.Batcher.PrepareCommandAsync(SqlDeleteString.CommandType, SqlDeleteString.Text, SqlDeleteString.ParameterTypes, cancellationToken)).ConfigureAwait(false);

					try
					{
						//offset += expectation.Prepare(st, factory.ConnectionProvider.Driver);
						await (WriteKeyAsync(st, id, offset, session, cancellationToken)).ConfigureAwait(false);
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

					if (log.IsDebugEnabled())
					{
						log.Debug("done deleting collection");
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle,
													 "could not delete collection: " + MessageHelper.CollectionInfoString(this, id));
				}
			}
		}

		public async Task RecreateAsync(IPersistentCollection collection, object id, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!isInverse && RowInsertEnabled)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Inserting collection: {0}", MessageHelper.CollectionInfoString(this, collection, id, session));
				}

				try
				{
					IExpectation expectation = null;
					bool useBatch = false;
					int i = 0;
					int count = 0;

					// create all the new entries
					foreach (var entry in collection.Entries(this))
					{
						// Init, if we're on the first element.
						if (count == 0)
						{
							expectation = Expectations.AppropriateExpectation(insertCheckStyle);
							await (collection.PreInsertAsync(this, cancellationToken)).ConfigureAwait(false);
							//bool callable = InsertCallable;
							useBatch = expectation.CanBeBatched;
						}
						
						if (collection.EntryExists(entry, i))
						{
							object entryId;
							if (!IsIdentifierAssignedByInsert)
							{
								// NH Different implementation: write once
								entryId = await (PerformInsertAsync(id, collection, expectation, entry, i, useBatch, false, session, cancellationToken)).ConfigureAwait(false);
							}
							else
							{
								entryId = await (PerformInsertAsync(id, collection, entry, i, session, cancellationToken)).ConfigureAwait(false);
							}
							collection.AfterRowInsert(this, entry, i, entryId);
							count++;
						}
						i++;
					}

					if (log.IsDebugEnabled())
					{
						if (count > 0)
							log.Debug("done inserting collection: {0} rows inserted", count);
						else
							log.Debug("collection was empty");
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle,
													 "could not insert collection: " + MessageHelper.CollectionInfoString(this, collection, id, session));
				}
			}
		}

		public async Task DeleteRowsAsync(IPersistentCollection collection, object id, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!isInverse && RowDeleteEnabled)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Deleting rows of collection: {0}", MessageHelper.CollectionInfoString(this, collection, id, session));
				}

				bool deleteByIndex = !IsOneToMany && hasIndex && !indexContainsFormula;

				try
				{
					// delete all the deleted entries
					var offset = 0;
					var count = 0;

					foreach (var entry in await (collection.GetDeletesAsync(this, !deleteByIndex, cancellationToken)).ConfigureAwait(false))
					{
						DbCommand st;
						var expectation = Expectations.AppropriateExpectation(deleteCheckStyle);
						//var callable = DeleteCallable;
						var commandInfo = GetDeleteCommand(deleteByIndex, entry, out var columnNullness);

						var useBatch = expectation.CanBeBatched;
						if (useBatch)
						{
							st = await (session.Batcher.PrepareBatchCommandAsync(
								commandInfo.CommandType, commandInfo.Text, commandInfo.ParameterTypes, cancellationToken)).ConfigureAwait(false);
						}
						else
						{
							st = await (session.Batcher.PrepareCommandAsync(
								commandInfo.CommandType, commandInfo.Text, commandInfo.ParameterTypes, cancellationToken)).ConfigureAwait(false);
						}
						try
						{
							var loc = offset;
							if (hasIdentifier)
							{
								await (WriteIdentifierAsync(st, entry, loc, session, cancellationToken)).ConfigureAwait(false);
							}
							else
							{
								loc = await (WriteKeyAsync(st, id, loc, session, cancellationToken)).ConfigureAwait(false);

								if (deleteByIndex)
								{
									await (WriteIndexToWhereAsync(st, entry, loc, session, cancellationToken)).ConfigureAwait(false);
								}
								else
								{
									await (WriteElementToWhereAsync(st, entry, columnNullness, loc, session, cancellationToken)).ConfigureAwait(false);
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
							count++;
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
					}

					if (log.IsDebugEnabled())
					{
						if (count > 0)
							log.Debug("done deleting collection rows: {0} deleted", count);
						else
							log.Debug("no rows to delete");
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle,
						"could not delete collection rows: " + MessageHelper.CollectionInfoString(this, collection, id, session));
				}
			}
		}

		public async Task InsertRowsAsync(IPersistentCollection collection, object id, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!isInverse && RowInsertEnabled)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Inserting rows of collection: {0}", MessageHelper.CollectionInfoString(this, collection, id, session));
				}

				try
				{
					// insert all the new entries
					await (collection.PreInsertAsync(this, cancellationToken)).ConfigureAwait(false);
					IExpectation expectation = Expectations.AppropriateExpectation(insertCheckStyle);
					//bool callable = InsertCallable;
					bool useBatch = expectation.CanBeBatched;
					int i = 0;
					int count = 0;

					IEnumerable entries = collection.Entries(this);
					foreach (object entry in entries)
					{
						if (await (collection.NeedsInsertingAsync(entry, i, elementType, cancellationToken)).ConfigureAwait(false))
						{
							object entryId;
							if (!IsIdentifierAssignedByInsert)
							{
								// NH Different implementation: write once
								entryId = await (PerformInsertAsync(id, collection, expectation, entry, i, useBatch, false, session, cancellationToken)).ConfigureAwait(false);
							}
							else
							{
								entryId = await (PerformInsertAsync(id, collection, entry, i, session, cancellationToken)).ConfigureAwait(false);
							}
							collection.AfterRowInsert(this, entry, i, entryId);
							count++;
						}
						i++;
					}

					if (log.IsDebugEnabled())
					{
						log.Debug("done inserting rows: {0} inserted", count);
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle,
													 "could not insert collection rows: " + MessageHelper.CollectionInfoString(this, collection, id, session));
				}
			}
		}

		public async Task UpdateRowsAsync(IPersistentCollection collection, object id, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!isInverse && collection.RowUpdatePossible)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Updating rows of collection: {0}#{1}", role, id);
				}

				// update all the modified entries
				int count = await (DoUpdateRowsAsync(id, collection, session, cancellationToken)).ConfigureAwait(false);

				if (log.IsDebugEnabled())
				{
					log.Debug("done updating rows: {0} updated", count);
				}
			}
		}

		protected abstract Task<int> DoUpdateRowsAsync(object key, IPersistentCollection collection, ISessionImplementor session, CancellationToken cancellationToken);

		protected async Task<object> PerformInsertAsync(object ownerId, IPersistentCollection collection, IExpectation expectation,
									   object entry, int index, bool useBatch, bool callable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object entryId = null;
			int offset = 0;
			var st = useBatch
				? await (session.Batcher.PrepareBatchCommandAsync(SqlInsertRowString.CommandType, SqlInsertRowString.Text, SqlInsertRowString.ParameterTypes, cancellationToken)).ConfigureAwait(false)
				: await (session.Batcher.PrepareCommandAsync(SqlInsertRowString.CommandType, SqlInsertRowString.Text, SqlInsertRowString.ParameterTypes, cancellationToken)).ConfigureAwait(false);
			try
			{
				//offset += expectation.Prepare(st, factory.ConnectionProvider.Driver);
				offset = await (WriteKeyAsync(st, ownerId, offset, session, cancellationToken)).ConfigureAwait(false);
				if (hasIdentifier)
				{
					entryId = collection.GetIdentifier(entry, index);
					offset = await (WriteIdentifierAsync(st, entryId, offset, session, cancellationToken)).ConfigureAwait(false);
				}
				if (hasIndex)
				{
					offset = await (WriteIndexAsync(st, collection.GetIndex(entry, index, this), offset, session, cancellationToken)).ConfigureAwait(false);
				}
				await (WriteElementAsync(st, collection.GetElement(entry), offset, session, cancellationToken)).ConfigureAwait(false);
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
			return entryId;
		}

		#region NH specific

		#region IPostInsertIdentityPersister Members

		public Task BindSelectByUniqueKeyAsync(
			ISessionImplementor session,
			DbCommand selectCommand,
			IBinder binder,
			string[] suppliedPropertyNames, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return binder.BindValuesAsync(selectCommand, cancellationToken);
		}
		#endregion

		/// <summary>
		/// Perform an SQL INSERT, and then retrieve a generated identifier.
		/// </summary>
		/// <returns> the id of the collection entry </returns>
		/// <remarks>
		/// This form is used for PostInsertIdentifierGenerator-style ids (IDENTITY, select, etc).
		/// </remarks>
		protected Task<object> PerformInsertAsync(object ownerId, IPersistentCollection collection, object entry, int index,
									   ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			IBinder binder = new GeneratedIdentifierBinder(ownerId, collection, entry, index, session, this);
			return identityDelegate.PerformInsertAsync(SqlInsertRowString, session, binder, cancellationToken);
		}

		protected partial class GeneratedIdentifierBinder : IBinder
		{

			public async Task BindValuesAsync(DbCommand cm, CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int offset = 0;
				offset = await (persister.WriteKeyAsync(cm, ownerId, offset, session, cancellationToken)).ConfigureAwait(false);
				if (persister.HasIndex)
				{
					offset = await (persister.WriteIndexAsync(cm, collection.GetIndex(entry, index, persister), offset, session, cancellationToken)).ConfigureAwait(false);
				}
				await (persister.WriteElementAsync(cm, collection.GetElement(entry), offset, session, cancellationToken)).ConfigureAwait(false);
			}
		}

		#endregion
	}
}
