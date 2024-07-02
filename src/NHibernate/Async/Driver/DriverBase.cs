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
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Driver
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class DriverBase : IDriver, ISqlParameterFormatter
	{

		#if NETFX
		#else

		#endif

		/// <inheritdoc />
		public virtual Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<DbDataReader>(cancellationToken);
			}
			return command.ExecuteReaderAsync(cancellationToken);
		}
	}
}
