using System;
using System.Data;

using NHibernate.Sql;
using NHibernate.Engine;
using NHibernate.Ps;
using NHibernate.Util;


namespace NHibernate.Type
{
	/// <summary>
	/// Superclass of single-column nullable types.
	/// </summary>
	public abstract class NullableType : AbstractType {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(NullableType));

		public abstract void Set(IDbCommand cmd, object value, int index);

		public abstract object Get(IDataReader rs, string name);

		public abstract Types SqlType { get; }

		public abstract string ToXML(object val);

		public override sealed string ToXML(object value, ISessionFactoryImplementor pc) {
			return (value==null) ? null : ToXML(value);
		}
		
		public override sealed void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) {
			NullSafeSet(cmd, value, index);
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index) {
			if (value==null) {
				if ( log.IsDebugEnabled )
					log.Debug("binding null to parameter: " + index.ToString());
				
				//Do we check IsNullable?
				( (IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
			}
			else {
				if ( log.IsDebugEnabled )
					log.Debug("binding '" + ToXML(value) + "' to parameter: " + index);
				
				Set(cmd, value, index);
			}
		}
		
		public override sealed object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner) {
			return NullSafeGet(rs, names[0]);
		}

		public object NullSafeGet(IDataReader rs, string[] names) {
			return NullSafeGet(rs, names[0]);
		}

		public object NullSafeGet(IDataReader rs, string name) {
			object val = rs[name];
			if ( val==null || val==DBNull.Value ) {
				if ( log.IsDebugEnabled )
					log.Debug("returning null as column: " + name);
				return null;
			}
			else {
				if ( log.IsDebugEnabled )
					log.Debug("returning '" + ToXML(val) + "' as column: " + name);
				return val;
			}
		}

		public override sealed object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner) {
			return NullSafeGet(rs, name);
		}
	
		public override sealed int GetColumnSpan(IMapping session) {
			return 1;
		}
	
		public override sealed Types[] SqlTypes(IMapping session) {
			return new Types[] { SqlType };
		}
	
		public abstract object DeepCopyNotNull(object val);
	
		public override sealed object DeepCopy(object val) {
			return (val==null) ? null : DeepCopyNotNull(val);
		}
	}
}