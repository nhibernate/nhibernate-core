﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Collection
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class AbstractBatchingCollectionInitializer : ICollectionInitializer
	{

		public abstract Task InitializeAsync(object id, ISessionImplementor session, CancellationToken cancellationToken);
	}
}
