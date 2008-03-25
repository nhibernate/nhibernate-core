using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using log4net;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;
using System.Data.Common;

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
	public class IncrementGenerator : IIdentifierGenerator, IConfigurable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(IncrementGenerator));

		private long next;
		private string sql;
		private System.Type returnClass;

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parms"></param>
		/// <param name="d"></param>
		public void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect d)
		{
			string tableList;
			string column;
			string schema;
			string catalog;

			if (!parms.TryGetValue("tables", out tableList))
				parms.TryGetValue(PersistentIdGeneratorParmsNames.Tables, out tableList);
			string[] tables = StringHelper.Split(", ", tableList);
			if (!parms.TryGetValue("column", out column))
				parms.TryGetValue(PersistentIdGeneratorParmsNames.PK, out column);
			returnClass = type.ReturnedClass;
			parms.TryGetValue(PersistentIdGeneratorParmsNames.Schema, out schema);
			parms.TryGetValue(PersistentIdGeneratorParmsNames.Catalog, out catalog);

			StringBuilder buf = new StringBuilder();
			for (int i = 0; i < tables.Length; i++)
			{
				if (tables.Length > 1)
				{
					buf.Append("select ").Append(column).Append(" from ");
				}
				buf.Append(Table.Qualify(catalog, schema, tables[i]));
				if (i < tables.Length - 1)
					buf.Append(" union ");
			}
			if (tables.Length > 1)
			{
				buf.Insert(0, "( ").Append(" ) ids_");
				column = "ids_." + column;
			}

			sql = "select max(" + column + ") from " + buf;
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
			if (sql != null)
			{
				GetNext(session);
			}
			return IdentifierGeneratorFactory.CreateNumber(next++, returnClass);
		}

		private void GetNext(ISessionImplementor session)
		{
			log.Debug("fetching initial value: " + sql);

			try
			{
				IDbConnection conn = session.Factory.OpenConnection();
				IDbCommand qps = conn.CreateCommand();
				qps.CommandText = sql;
				qps.CommandType = CommandType.Text;
				try
				{
					IDataReader rs = qps.ExecuteReader();
					try
					{
						if (rs.Read())
						{
							next = !rs.IsDBNull(0) ? Convert.ToInt64(rs.GetValue(0)) + 1 : 1L;
						}
						else
						{
							next = 1L;
						}
						sql = null;
						log.Debug("first free id: " + next);
					}
					finally
					{
						rs.Close();
					}
				}
				finally
				{
					session.Factory.CloseConnection(conn);
				}
			}
			catch (DbException sqle)
			{
				log.Error("could not get increment value", sqle);
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
				                                 "could not fetch initial value for increment generator");
			}
		}
	}
}