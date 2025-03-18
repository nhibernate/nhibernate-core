﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cache;
#if !NETFX
using NHibernate.Util;
#endif

namespace NHibernate.Test.NHSpecificTest.NH2898
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class BinaryFormatterCache : CacheBase
	{

		public override Task PutAsync(object key, object value, CancellationToken cancellationToken)
		{
			try
			{
				var fmt = new BinaryFormatter
				{
#if !NETFX
					SurrogateSelector = new SerializationHelper.SurrogateSelector()	
#endif
				};
				using (var stream = new MemoryStream())
				{
					fmt.Serialize(stream, value);
					_hashtable[key] = stream.ToArray();
				}
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task RemoveAsync(object key, CancellationToken cancellationToken)
		{
			try
			{
				_hashtable.Remove(key);
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task ClearAsync(CancellationToken cancellationToken)
		{
			try
			{
				_hashtable.Clear();
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task<object> LockAsync(object key, CancellationToken cancellationToken)
		{
			// local cache, so we use synchronization
			return Task.FromResult<object>(null);
		}

		public override Task UnlockAsync(object key, object lockValue, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
			// local cache, so we use synchronization
		}
	}
}
