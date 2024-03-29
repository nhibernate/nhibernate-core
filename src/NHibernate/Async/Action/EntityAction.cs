﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.IO;
using System.Runtime.Serialization;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Util;
using NHibernate.Impl;
using NHibernate.Persister;

namespace NHibernate.Action
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class EntityAction : 
		IAsyncExecutable,
		IBeforeTransactionCompletionProcess,
		IAfterTransactionCompletionProcess,
		IComparable<EntityAction>, 
		IDeserializationCallback,
		ICacheableExecutable
	{

		#region IExecutable Members

		public Task BeforeExecutionsAsync(CancellationToken cancellationToken)
		{
			throw new AssertionFailure("BeforeExecutions() called for non-collection action");
		}

		public abstract Task ExecuteAsync(CancellationToken cancellationToken);

		protected virtual Task BeforeTransactionCompletionProcessImplAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				BeforeTransactionCompletionProcessImpl();
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
		
		protected virtual Task AfterTransactionCompletionProcessImplAsync(bool success, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				AfterTransactionCompletionProcessImpl(success);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		#endregion

		public Task ExecuteBeforeTransactionCompletionAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return BeforeTransactionCompletionProcessImplAsync(cancellationToken);
		}

		public Task ExecuteAfterTransactionCompletionAsync(bool success, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return AfterTransactionCompletionProcessImplAsync(success, cancellationToken);
		}
	}
}
