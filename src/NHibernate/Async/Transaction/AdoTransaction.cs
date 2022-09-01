﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Impl;

namespace NHibernate.Transaction
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class AdoTransaction : ITransaction
	{

		public Task BeginAsync()
		{
			return BeginAsync(IsolationLevel.Unspecified);
		}

		/// <summary>
		/// Begins the <see cref="DbTransaction"/> on the <see cref="DbConnection"/>
		/// used by the <see cref="ISession"/>.
		/// </summary>
		/// <exception cref="TransactionException">
		/// Thrown if there is any problems encountered while trying to create
		/// the <see cref="DbTransaction"/>.
		/// </exception>
		public async Task BeginAsync(IsolationLevel isolationLevel)
		{
			using (session.BeginProcess())
			{
				if (begun)
				{
					return;
				}

				if (commitFailed)
				{
					throw new TransactionException("Cannot restart transaction after failed commit");
				}

				if (isolationLevel == IsolationLevel.Unspecified)
				{
					isolationLevel = session.Factory.Settings.IsolationLevel;
				}

				log.Debug("Begin ({0})", isolationLevel);

				try
				{
					trans = await (session.Factory.ConnectionProvider.Driver.BeginTransactionAsync(isolationLevel, session.Connection)).ConfigureAwait(false);
				}
				catch (HibernateException)
				{
					// Don't wrap HibernateExceptions
					throw;
				}
				catch (Exception e)
				{
					log.Error(e, "Begin transaction failed");
					throw new TransactionException("Begin failed with SQL exception", e);
				}

				begun = true;
				committed = false;
				rolledBack = false;

				session.AfterTransactionBegin(this);
				foreach (var dependentSession in session.ConnectionManager.DependentSessions)
					dependentSession.AfterTransactionBegin(this);
			}
		}

		private async Task AfterTransactionCompletionAsync(bool successful, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			session.ConnectionManager.AfterTransaction();
			await (session.AfterTransactionCompletionAsync(successful, this, cancellationToken)).ConfigureAwait(false);
			await (NotifyLocalSynchsAfterTransactionCompletionAsync(successful, cancellationToken)).ConfigureAwait(false);
			foreach (var dependentSession in session.ConnectionManager.DependentSessions)
				await (dependentSession.AfterTransactionCompletionAsync(successful, this, cancellationToken)).ConfigureAwait(false);
	
			session = null;
			begun = false;
		}

		/// <summary>
		/// Commits the <see cref="ITransaction"/> by flushing asynchronously the <see cref="ISession"/>
		/// then committing synchronously the <see cref="DbTransaction"/>.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <exception cref="TransactionException">
		/// Thrown if there is any exception while trying to call <c>Commit()</c> on 
		/// the underlying <see cref="DbTransaction"/>.
		/// </exception>
		public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (session.BeginProcess())
			{
				CheckNotDisposed();
				CheckBegun();
				CheckNotZombied();

				log.Debug("Start Commit");

				await (session.BeforeTransactionCompletionAsync(this, cancellationToken)).ConfigureAwait(false);
				await (NotifyLocalSynchsBeforeTransactionCompletionAsync(cancellationToken)).ConfigureAwait(false);
				foreach (var dependentSession in session.ConnectionManager.DependentSessions)
					await (dependentSession.BeforeTransactionCompletionAsync(this, cancellationToken)).ConfigureAwait(false);

				try
				{
					trans.Commit();
					log.Debug("DbTransaction Committed");

					committed = true;
					await (AfterTransactionCompletionAsync(true, cancellationToken)).ConfigureAwait(false);
					Dispose();
				}
				catch (OperationCanceledException) { throw; }
				catch (HibernateException e)
				{
					log.Error(e, "Commit failed");
					await (AfterTransactionCompletionAsync(false, cancellationToken)).ConfigureAwait(false);
					commitFailed = true;
					// Don't wrap HibernateExceptions
					throw;
				}
				catch (Exception e)
				{
					log.Error(e, "Commit failed");
					await (AfterTransactionCompletionAsync(false, cancellationToken)).ConfigureAwait(false);
					commitFailed = true;
					throw new TransactionException("Commit failed with SQL exception", e);
				}
				finally
				{
					CloseIfRequired();
				}
			}
		}

		/// <summary>
		/// Rolls back the <see cref="ITransaction"/> by calling the method <c>Rollback</c> 
		/// on the underlying <see cref="DbTransaction"/>.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <exception cref="TransactionException">
		/// Thrown if there is any exception while trying to call <c>Rollback()</c> on 
		/// the underlying <see cref="DbTransaction"/>.
		/// </exception>
		public async Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (SessionIdLoggingContext.CreateOrNull(sessionId))
			{
				CheckNotDisposed();
				CheckBegun();
				CheckNotZombied();

				log.Debug("Rollback");

				if (!commitFailed)
				{
					try
					{
						trans.Rollback();
						log.Debug("DbTransaction RolledBack");
						rolledBack = true;
						Dispose();
					}
					catch (HibernateException e)
					{
						log.Error(e, "Rollback failed");
						// Don't wrap HibernateExceptions
						throw;
					}
					catch (Exception e)
					{
						log.Error(e, "Rollback failed");
						throw new TransactionException("Rollback failed with SQL Exception", e);
					}
					finally
					{
						await (AfterTransactionCompletionAsync(false, cancellationToken)).ConfigureAwait(false);
						CloseIfRequired();
					}
				}
			}
		}

		#region System.IDisposable Members

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this AdoTransaction is being Disposed of or Finalized.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <remarks>
		/// If this AdoTransaction is being Finalized (<c>isDisposing==false</c>) then make sure not
		/// to call any methods that could potentially bring this AdoTransaction back to life.
		/// </remarks>
		protected virtual async Task DisposeAsync(bool isDisposing, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (SessionIdLoggingContext.CreateOrNull(sessionId))
			{
				if (_isAlreadyDisposed)
				{
					// don't dispose of multiple times.
					return;
				}
				_isAlreadyDisposed = true;

				// free managed resources that are being managed by the AdoTransaction if we
				// know this call came through Dispose()
				if (isDisposing)
				{
					try
					{
						if (trans != null)
						{
							trans.Dispose();
							trans = null;
							log.Debug("DbTransaction disposed.");
						}

						if (IsActive)
						{
							// Assume we are rolled back
							rolledBack = true;
							if (session != null)
								await (AfterTransactionCompletionAsync(false, cancellationToken)).ConfigureAwait(false);
						}
						// nothing for Finalizer to do - so tell the GC to ignore it
						GC.SuppressFinalize(this);
					}
					finally
					{
						// Do not leave the object in an inconsistent state in case of disposal failure: we should assume
						// the DbTransaction is either no more ongoing or unrecoverable.
						begun = false;
					}
				}

				// free unmanaged resources here
			}
		}

		#endregion

		private async Task NotifyLocalSynchsBeforeTransactionCompletionAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
#pragma warning disable 612
			if (synchronizations != null)
			{
				foreach (var sync in synchronizations)
#pragma warning restore 612
				{
					try
					{
						sync.BeforeCompletion();
					}
					catch (Exception e)
					{
						log.Error(e, "exception calling user Synchronization");
						throw;
					}
				}
			}

			if (_completionSynchronizations == null)
				return;

			foreach (var sync in _completionSynchronizations)
			{
				await (sync.ExecuteBeforeTransactionCompletionAsync(cancellationToken)).ConfigureAwait(false);
			}
		}

		private async Task NotifyLocalSynchsAfterTransactionCompletionAsync(bool success, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			begun = false;

#pragma warning disable 612
			if (synchronizations != null)
			{
				foreach (var sync in synchronizations)
#pragma warning restore 612
				{
					try
					{
						sync.AfterCompletion(success);
					}
					catch (Exception e)
					{
						log.Error(e, "exception calling user Synchronization");
					}
				}
			}

			if (_completionSynchronizations == null)
				return;

			foreach (var sync in _completionSynchronizations)
			{
				try
				{
					await (sync.ExecuteAfterTransactionCompletionAsync(success, cancellationToken)).ConfigureAwait(false);
				}
				catch (OperationCanceledException) { throw; }
				catch (Exception e)
				{
					log.Error(e, "exception calling user Synchronization");
				}
			}
		}
	}
}
