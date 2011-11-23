using System.Data;
using System.Data.Common;

using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Id.Enhanced
{
	/// <summary>
	/// Describes a sequence.
	/// </summary>
	public class SequenceStructure : IDatabaseStructure
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(SequenceStructure));

		private readonly int _incrementSize;
		private readonly int _initialValue;
		private readonly string _sequenceName;
		private readonly SqlString _sql;
		private int _accessCounter;
		private bool _applyIncrementSizeToSourceValues;

		public SequenceStructure(Dialect.Dialect dialect, string sequenceName, int initialValue, int incrementSize)
		{
			_sequenceName = sequenceName;
			_initialValue = initialValue;
			_incrementSize = incrementSize;
			_sql = new SqlString(dialect.GetSequenceNextValString(sequenceName));
		}

		#region IDatabaseStructure Members

		public string Name
		{
			get { return _sequenceName; }
		}

		public int IncrementSize
		{
			get { return _incrementSize; }
		}

		public IAccessCallback BuildCallback(ISessionImplementor session)
		{
			return new SequenceAccessCallback(session, this);
		}

		public void Prepare(IOptimizer optimizer)
		{
			_applyIncrementSizeToSourceValues = optimizer.ApplyIncrementSizeToSourceValues;
		}

		public string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			int sourceIncrementSize = _applyIncrementSizeToSourceValues ? _incrementSize : 1;
			return dialect.GetCreateSequenceStrings(_sequenceName, _initialValue, sourceIncrementSize);
		}

		public string[] SqlDropStrings(Dialect.Dialect dialect)
		{
			return dialect.GetDropSequenceStrings(_sequenceName);
		}

		public int TimesAccessed
		{
			get { return _accessCounter; }
		}

		#endregion

		#region Nested type: SequenceAccessCallback

		private class SequenceAccessCallback : IAccessCallback
		{
			private readonly SequenceStructure _owner;
			private readonly ISessionImplementor _session;

			public SequenceAccessCallback(ISessionImplementor session, SequenceStructure owner)
			{
				_session = session;
				_owner = owner;
			}

			#region IAccessCallback Members

			public virtual long GetNextValue()
			{
				_owner._accessCounter++;
				try
				{
					IDbCommand st = _session.Batcher.PrepareCommand(CommandType.Text, _owner._sql, new SqlType[] { SqlTypeFactory.Int64 });
					IDataReader rs = null;
					try
					{
						rs = _session.Batcher.ExecuteReader(st);
						try
						{
							rs.Read();
							long result = rs.GetInt64(0);
							if (Log.IsDebugEnabled)
							{
								Log.Debug("Sequence identifier generated: " + result);
							}
							return result;
						}
						finally
						{
							try
							{
								rs.Close();
							}
							catch
							{
								// intentionally empty
							}
						}
					}
					finally
					{
						_session.Batcher.CloseCommand(st, rs);
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(_session.Factory.SQLExceptionConverter, sqle, "could not get next sequence value",
													 _owner._sql);
				}
			}

			#endregion
		}

		#endregion
	}
}