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
using NHibernate.AdoNet;
using NHibernate.Engine.Query;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class OracleDataClientDriverBase : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{

		public override Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken)
		{
			if (!SuppressDecimalInvalidCastException && _suppressDecimalInvalidCastExceptionSetter == null)
			{
				throw new NotSupportedException("OracleDataReader.SuppressGetDecimalInvalidCastException property is supported only in ODP.NET version 19.10 or newer");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<DbDataReader>(cancellationToken);
			}
			return InternalExecuteReaderAsync();
			async Task<DbDataReader> InternalExecuteReaderAsync()
			{

				var reader = await (command.ExecuteReaderAsync(cancellationToken)).ConfigureAwait(false);

				if (SuppressDecimalInvalidCastException)
				{
					_suppressDecimalInvalidCastExceptionSetter(reader, true);
				}

				string timestampFormat = GetDateFormat(command.Connection);

				return new OracleDbDataReader(reader, timestampFormat);
			}
		}
	}
}
