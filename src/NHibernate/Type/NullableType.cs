using System;

using log4net;

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

		private static readonly ILog log = LogManager.GetLogger(StringHelper.Qualifier((typeof(IType)).ToString()));  //Is it correct?

		//Needs PreparedStatement implementation
		//
		//public abstract void Set(PreparedStatement st, object value, int index);

		//Needs to solve ResultSet problem
		//public abstract object Get(ResultSet rs, string name);  ????

		public abstract Types SqlType { get; }
		public abstract string ToXML(object val);

		//Needs SessionFactoryImplementor
		/*
		public sealed string ToXML(object val, SessionFactoryImplementor pc) {
			return (value==null) ? null : ToXML(val);
		}
		*/

		/*  Needs PreparedStatement implementation
		 * 
		public sealed void NullSafeSet(PreparedStatement st, object value, int index, ISessionImplementor session) {
			NullSafeSet(st, value, index);
		}
		


		public sealed void NullSafeSet(PreparedStatement st, object val, int index) {
			if (val==null) {
				if ( log.IsDebugEnabled )
					LogManager.GetLogger(GetType()).Debug("binding null to parameter: " + index.ToString());
				
				st.SetNull( index, SqlType() );
			}
			else {
				if ( log.IsDebugEnabled )
					LogManager.GetLogger(GetType()).Debug("binding '" + ToXML(val) + "' to parameter: " + index);
				
				Set(st, val, index);
			}
		}
		*/


		/* Needs to solve ResultSet problem
		 * 
		public sealed object NullSafeGet(ResultSet rs, string[] names, ISessionImplementor session, object owner) {
			return NullSafeGet(rs, names[0]);
		}

		public sealed object NullSafeGet(ResultSet rs, string[] names) {
			return NullSafeGet(rs, names[0]);
		}

		public sealed object NullSafeGet(ResultSet rs, string name) {
			object val = Get(rs, name);
			if ( val==null || rs.wasNull() ) { //rs!!!!
				if ( log.IsDebugEnabled )
					LogManager.GetLogger(GetType()).Debug("returning null as column: " + name);
				return null;
			}
			else {
				if ( log.isTraceEnabled() )
					LogManager.GetLogger(GetType()).Debug("returning '" + ToXML(val) + "' as column: " + name);
				return val;
			}
		}

		public sealed object NullSafeGet(ResultSet rs, string name, ISessionImplementor session, object owner) {
			return NullSafeGet(rs, name);
		}
		*/
	
		public override int GetColumnSpan(IMapping session) {
			return 1;
		}
	
		public override Types[] SqlTypes(IMapping session) {
			return new Types[] { SqlType };
		}
	
		public abstract object DeepCopyNotNull(object val);
	
		public override object DeepCopy(object val) {
			return (val==null) ? null : DeepCopyNotNull(val);
		}
	}
}