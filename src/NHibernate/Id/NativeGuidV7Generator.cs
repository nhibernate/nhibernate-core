using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Id
{
	/// <summary>
	/// Generates Guid values using a server-side UUIDv7 function exposed as <c>new_uuid_v7</c> by the dialect.
	/// </summary>
	public partial class NativeGuidV7Generator : IIdentifierGenerator
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(NativeGuidV7Generator));
		private readonly IType identifierType = new GuidType();

		private static SqlString GetSelectGuidV7String(Dialect.Dialect dialect)
		{
			if (!dialect.Functions.TryGetValue("new_uuid_v7", out var function))
			{
				throw new IdentifierGenerationException(
					"The configured dialect does not provide new_uuid_v7.");
			}

			var functionCall = function.Render(new List<object>(), null).ToString();
			return new SqlString("select ", functionCall);
		}

		public object Generate(ISessionImplementor session, object obj)
		{
			var sql = GetSelectGuidV7String(session.Factory.Dialect);
			try
			{
				var st = session.Batcher.PrepareCommand(CommandType.Text, sql, SqlTypeFactory.NoTypes);
				DbDataReader reader = null;
				try
				{
					reader = session.Batcher.ExecuteReader(st);
					object result;
					try
					{
						reader.Read();
						result = IdentifierGeneratorFactory.Get(reader, identifierType, session);
					}
					finally
					{
						reader.Close();
					}
					log.Debug("GUID v7 identifier generated: {0}", result);
					return result;
				}
				finally
				{
					session.Batcher.CloseCommand(st, reader);
				}
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle, "could not retrieve GUID v7", sql);
			}
		}
	}
}
