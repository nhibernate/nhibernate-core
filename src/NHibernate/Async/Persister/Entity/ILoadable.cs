﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Type;
using NHibernate.Engine;
using System.Data.Common;

namespace NHibernate.Persister.Entity
{
	using System.Threading.Tasks;
	using System.Threading;

	public partial interface ILoadable : IEntityPersister
	{

		/// <summary>
		/// Retrieve property values from one row of a result set
		/// </summary>
		//Since v5.3
		[Obsolete("Use the extension method without the rootLoadable parameter instead")]
		Task<object[]> HydrateAsync(DbDataReader rs, object id, object obj, ILoadable rootLoadable, string[][] suffixedPropertyColumns,
						 bool allProperties, ISessionImplementor session, CancellationToken cancellationToken);
	}

	public static partial class LoadableExtensions
	{
		public static Task<object[]> HydrateAsync(
			this ILoadable loadable,
			DbDataReader rs,
			object id,
			object obj,
			string[][] suffixedPropertyColumns,
			bool allProperties,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object[]>(cancellationToken);
			}
			try
			{
				if (loadable is AbstractEntityPersister entityPersister)
				{
					return entityPersister.HydrateAsync(rs, id, obj, suffixedPropertyColumns, allProperties, session, cancellationToken);
				}

				var rootLoadable = loadable.RootEntityName == loadable.EntityName
				? loadable
				: (ILoadable) loadable.Factory.GetEntityPersister(loadable.RootEntityName);

#pragma warning disable 618
				return loadable.HydrateAsync(rs, id, obj, rootLoadable, suffixedPropertyColumns, allProperties, session, cancellationToken);
#pragma warning restore 618
			}
			catch (Exception ex)
			{
				return Task.FromException<object[]>(ex);
			}
		}
	}
}
