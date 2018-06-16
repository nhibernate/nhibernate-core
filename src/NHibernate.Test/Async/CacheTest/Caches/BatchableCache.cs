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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cache;

namespace NHibernate.Test.CacheTest.Caches
{
	public partial class BatchableCache : ICache, IBatchableCache
	{

		public Task PutManyAsync(object[] keys, object[] values, CancellationToken cancellationToken)
		{
			try
			{
				PutMultipleCalls.Add(keys);
				for (int i = 0; i < keys.Length; i++)
				{
					_hashtable[keys[i]] = values[i];
				}
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public Task<object> LockManyAsync(object[] keys, CancellationToken cancellationToken)
		{
			try
			{
				LockMultipleCalls.Add(keys);
				return Task.FromResult<object>(null);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public Task UnlockManyAsync(object[] keys, object lockValue, CancellationToken cancellationToken)
		{
			try
			{
				UnlockMultipleCalls.Add(keys);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		#region ICache Members

		/// <summary></summary>
		public Task<object> GetAsync(object key, CancellationToken cancellationToken)
		{
			try
			{
				GetCalls.Add(key);
				return Task.FromResult<object>(_hashtable[key]);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public Task<object[]> GetManyAsync(object[] keys, CancellationToken cancellationToken)
		{
			try
			{
				GetMultipleCalls.Add(keys);
				var result = new object[keys.Length];
				for (var i = 0; i < keys.Length; i++)
				{
					result[i] = _hashtable[keys[i]];
				}
				return Task.FromResult<object[]>(result);
			}
			catch (Exception ex)
			{
				return Task.FromException<object[]>(ex);
			}
		}

		/// <summary></summary>
		public Task PutAsync(object key, object value, CancellationToken cancellationToken)
		{
			try
			{
				PutCalls.Add(key);
				_hashtable[key] = value;
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary></summary>
		public Task RemoveAsync(object key, CancellationToken cancellationToken)
		{
			try
			{
				_hashtable.Remove(key);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary></summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public Task ClearAsync(CancellationToken cancellationToken)
		{
			try
			{
				_hashtable.Clear();
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary></summary>
		public Task LockAsync(object key, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
			// local cache, so we use synchronization
		}

		/// <summary></summary>
		public Task UnlockAsync(object key, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
			// local cache, so we use synchronization
		}

		#endregion
	}
}
