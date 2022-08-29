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
using System.Runtime.CompilerServices;
using System.Text;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class IncrementGenerator : IIdentifierGenerator, IConfigurable
	{

		/// <summary>
		///
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns></returns>
		public async Task<object> GenerateAsync(ISessionImplementor session, object obj, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (await (_asyncLock.LockAsync()).ConfigureAwait(false))
			{
				if (_sql != null)
				{
					await (GetNextAsync(session, cancellationToken)).ConfigureAwait(false);
				}

				return IdentifierGeneratorFactory.CreateNumber(_next++, _returnClass);
			}
		}

		private async Task GetNextAsync(ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Logger.Debug("fetching initial value: {0}", _sql);

			try
			{
				var cmd = await (session.Batcher.PrepareCommandAsync(CommandType.Text, _sql, SqlTypeFactory.NoTypes, cancellationToken)).ConfigureAwait(false);
				DbDataReader reader = null;
				try
				{
					reader = await (session.Batcher.ExecuteReaderAsync(cmd, cancellationToken)).ConfigureAwait(false);
					try
					{
						if (await (reader.ReadAsync(cancellationToken)).ConfigureAwait(false))
						{
							_next = !reader.IsDBNull(0) ? Convert.ToInt64(reader.GetValue(0)) + 1 : 1L;
						}
						else
						{
							_next = 1L;
						}
						_sql = null;
						Logger.Debug("first free id: {0}", _next);
					}
					finally
					{
						cancellationToken.ThrowIfCancellationRequested();
						await (reader.CloseAsync()).ConfigureAwait(false);
					}
				}
				finally
				{
					session.Batcher.CloseCommand(cmd, reader);
				}
			}
			catch (DbException sqle)
			{
				Logger.Error(sqle, "could not get increment value");
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
												 "could not fetch initial value for increment generator");
			}
		}
	}
}
