using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using log4net;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

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
		public void Configure(IType type, IDictionary parms, Dialect.Dialect d)
		{
			string tableList = (string)parms["tables"];
			if (tableList == null)
				tableList = (string)parms[PersistentIdGeneratorParmsNames.Tables];
			string[] tables = StringHelper.Split(", ", tableList);
			string column = (string)parms["column"];
			if (column == null)
				column = (string)parms[PersistentIdGeneratorParmsNames.PK];
			string schema = (string)parms[PersistentIdGeneratorParmsNames.Schema];
			string catalog = (string)parms[PersistentIdGeneratorParmsNames.Catalog];
			returnClass = type.ReturnedClass;

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
							next = !rs.IsDBNull(0) ? Convert.ToInt64(rs.GetValue(0)) + 1: 1L;
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
			catch (Exception sqle)
			{
				log.Error("could not get increment value", sqle);
				throw ADOExceptionHelper.Convert(sqle, "could not fetch initial value for increment generator");
			}
		}
	}
}