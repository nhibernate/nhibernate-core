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

namespace NHibernate.Id
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that returns a <c>Int64</c>, constructed by
	/// counting from the maximum primary key value at startup. Not safe for use in a
	/// cluster!
	/// </summary>
	/// <remarks>
	/// <para>
	/// java author Gavin King, .NET port Mark Holden
	/// </para>
	/// <para>
	/// Mapping parameters supported, but not usually needed: table, column.
	/// </para>
	/// </remarks>
	public partial class IncrementGenerator : IIdentifierGenerator, IConfigurable
	{
		private static readonly IInternalLogger Logger = LoggerProvider.LoggerFor(typeof(IncrementGenerator));

		private long _next;
		private SqlString _sql;
		private System.Type _returnClass;

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parms"></param>
		/// <param name="dialect"></param>
		public void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			string tableList;
			string column;
			string schema;
			string catalog;

			if (!parms.TryGetValue("tables", out tableList))
				parms.TryGetValue(PersistentIdGeneratorParmsNames.Tables, out tableList);
			string[] tables = tableList.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			if (!parms.TryGetValue("column", out column))
				parms.TryGetValue(PersistentIdGeneratorParmsNames.PK, out column);

			_returnClass = type.ReturnedClass;

			parms.TryGetValue(PersistentIdGeneratorParmsNames.Schema, out schema);
			parms.TryGetValue(PersistentIdGeneratorParmsNames.Catalog, out catalog);

			StringBuilder buf = new StringBuilder();
			for (int i = 0; i < tables.Length; i++)
			{
				if (tables.Length > 1)
				{
					buf.Append("select ").Append(column).Append(" from ");
				}
				buf.Append(dialect.Qualify(catalog, schema, tables[i]));
				if (i < tables.Length - 1)
					buf.Append(" union ");
			}
			if (tables.Length > 1)
			{
				buf.Insert(0, "( ").Append(" ) ids_");
				column = "ids_." + column;
			}

			var sqlTxt = string.Format("select max({0}) from {1}", column, buf);
			_sql = new SqlString(sqlTxt);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public object Generate(ISessionImplementor session, object obj)
		{
			if (_sql != null)
			{
				GetNext(session);
			}
			return IdentifierGeneratorFactory.CreateNumber(_next++, _returnClass);
		}

		private void GetNext(ISessionImplementor session)
		{
			Logger.Debug("fetching initial value: " + _sql);

			try
			{
				var cmd = session.Batcher.PrepareCommand(CommandType.Text, _sql, SqlTypeFactory.NoTypes);
				DbDataReader reader = null;
				try
				{
					reader = session.Batcher.ExecuteReader(cmd);
					try
					{
						if (reader.Read())
						{
							_next = !reader.IsDBNull(0) ? Convert.ToInt64(reader.GetValue(0)) + 1 : 1L;
						}
						else
						{
							_next = 1L;
						}
						_sql = null;
						Logger.Debug("first free id: " + _next);
					}
					finally
					{
						reader.Close();
					}
				}
				finally
				{
					session.Batcher.CloseCommand(cmd, reader);
				}
			}
			catch (DbException sqle)
			{
				Logger.Error("could not get increment value", sqle);
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
												 "could not fetch initial value for increment generator");
			}
		}
	}
}
