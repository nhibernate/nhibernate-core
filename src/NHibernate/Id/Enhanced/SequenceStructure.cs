using System;
using System.Data;
using System.Data.Common;
using log4net;
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
		private static readonly ILog log = LogManager.GetLogger(typeof (SequenceStructure));
		private readonly int incrementSize;
		private readonly int initialValue;
		private readonly string sequenceName;
		private readonly SqlString sql;
		private int accessCounter;
		private bool applyIncrementSizeToSourceValues;

		public SequenceStructure(Dialect.Dialect dialect, string sequenceName, int initialValue, int incrementSize)
		{
			this.sequenceName = sequenceName;
			this.initialValue = initialValue;
			this.incrementSize = incrementSize;
			sql = new SqlString(dialect.GetSequenceNextValString(sequenceName));
		}

		#region IDatabaseStructure Members

		public string Name
		{
			get { return sequenceName; }
		}

		public int IncrementSize
		{
			get { return incrementSize; }
		}

		public IAccessCallback BuildCallback(ISessionImplementor session)
		{
			return new SequenceAccessCallback(session, this);
		}

		public void Prepare(IOptimizer optimizer)
		{
			applyIncrementSizeToSourceValues = optimizer.ApplyIncrementSizeToSourceValues;
		}

		public string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			int sourceIncrementSize = applyIncrementSizeToSourceValues ? incrementSize : 1;
			return dialect.GetCreateSequenceStrings(sequenceName, initialValue, sourceIncrementSize);
		}

		public string[] SqlDropStrings(Dialect.Dialect dialect)
		{
			return dialect.GetDropSequenceStrings(sequenceName);
		}

		public int TimesAccessed
		{
			get { return accessCounter; }
		}

		#endregion

		#region Nested type: SequenceAccessCallback

		private class SequenceAccessCallback : IAccessCallback
		{
			private readonly SequenceStructure owner;
			private readonly ISessionImplementor session;

			public SequenceAccessCallback(ISessionImplementor session, SequenceStructure owner)
			{
				this.session = session;
				this.owner = owner;
			}

			#region IAccessCallback Members

			public virtual long NextValue
			{
				get
				{
					owner.accessCounter++;
					try
					{
						IDbCommand st = session.Batcher.PrepareCommand(CommandType.Text, owner.sql, new SqlType[] {SqlTypeFactory.Int64});
						IDataReader rs = null;
						try
						{
							rs = session.Batcher.ExecuteReader(st);
							try
							{
								rs.Read();
								long result = rs.GetInt64(0);
								if (log.IsDebugEnabled)
								{
									log.Debug("Sequence identifier generated: " + result);
								}
								return result;
							}
							finally
							{
								try
								{
									rs.Close();
								}
								catch (Exception ignore)
								{
									// intentionally empty
								}
							}
						}
						finally
						{
							session.Batcher.CloseCommand(st, rs);
						}
					}
					catch (DbException sqle)
					{
						throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle, "could not get next sequence value",
						                                 owner.sql);
					}
				}
			}

			#endregion
		}

		#endregion
	}
}