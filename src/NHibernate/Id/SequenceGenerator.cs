using System;
using System.Data;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Dialect;


namespace NHibernate.Id {
	/// <summary>
	/// Generates <c>long</c> values using an oracle-style sequence. A higher performance
	/// algorithm is <c>SequenceHiLoGerator</c>
	/// </summary>
	public class SequenceGenerator : IPersistentIdentifierGenerator, IConfigurable {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SequenceGenerator));

		/// <summary>
		/// The sequence parameter
		/// </summary>
		public const string Sequence = "sequence";

		private string sequenceName;
		private System.Type returnClass;
		private string sql;

		public virtual void Configure(IType type, IDictionary parms, Dialect.Dialect dialect) {
			this.sequenceName = PropertiesHelper.GetString(Sequence, parms, "hibernate_sequence");
			returnClass = type.ReturnedClass;
			sql = dialect.GetSequenceNextValString(sequenceName);
		}

		public virtual object Generate(ISessionImplementor session, object obj) {
			IDbCommand st = session.Batcher.PrepareStatement(sql);
			try {
				IDataReader rs = st.ExecuteReader();
				object result = null;
				try {
					rs.Read();
					result = IdentifierGeneratorFactory.Get(rs, returnClass);
				} finally {
					rs.Close();
				}
				log.Debug("sequence ID generated: " + result);
				return result;
			} catch (Exception e) {
				throw e;
			} finally {
				session.Batcher.CloseStatement(st);
			}
		}

		public string[] SqlCreateStrings(Dialect.Dialect dialect) {
			return new string[] {
									dialect.GetCreateSequenceString(sequenceName)
								};
		}

		public string SqlDropString(Dialect.Dialect dialect) {
			return dialect.GetDropSequenceString(sequenceName);
		}

		public object GeneratorKey() {
			return sequenceName;
		}
	}
}
