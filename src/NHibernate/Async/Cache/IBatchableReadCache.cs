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
using System.Text;

namespace NHibernate.Cache
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface IBatchableReadCache
	{
		/// <summary>
		/// Get multiple objects from the cache.
		/// </summary>
		/// <param name="keys">The keys to be retrieved from the cache.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns></returns>
		Task<object[]> GetManyAsync(object[] keys, CancellationToken cancellationToken);
	}
}
