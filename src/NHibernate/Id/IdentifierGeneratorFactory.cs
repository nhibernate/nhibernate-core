using System;
using System.Data;
using System.Collections;
using NHibernate.Type;


namespace NHibernate.Id {
	
	public sealed class IdentifierGeneratorFactory {
		public static object Get(IDataReader rs, System.Type clazz) {
			if (clazz==typeof(long)) {
				return rs.GetInt64(0);
			} else if (clazz==typeof(int)) {
				return rs.GetInt32(0);
			} else if (clazz==typeof(short)) {
				return rs.GetInt16(0);
			} else {
				throw new IdentifierGenerationException("this id generator generates long, integer, short");
			}
		}

		private static readonly Hashtable idgenerators = new Hashtable();
		public static readonly string ShortCircuitIndicator = string.Empty;

		static IdentifierGeneratorFactory() {
			idgenerators.Add("uuid.string", typeof(UUIDStringGenerator));
			idgenerators.Add("hilo", typeof(TableHiLoGenerator));
			idgenerators.Add("assigned", typeof(Assigned));
			idgenerators.Add("identity", typeof(IdentityGenerator));
			idgenerators.Add("sequence", typeof(SequenceGenerator));
			idgenerators.Add("seqhilo", typeof(SequenceHiLoGenerator));
			idgenerators.Add("vm", typeof(CounterGenerator));
			idgenerators.Add("foreign", typeof(ForeignGenerator));
		}

		private IdentifierGeneratorFactory() {} //cannot be instantiated

		public static IIdentifierGenerator Create(string strategy, IType type, IDictionary parms, Dialect.Dialect dialect) {
			try {
				System.Type clazz = (System.Type) idgenerators[strategy];
				if ( "native".Equals(strategy) ) {
					if ( dialect.SupportsIdentityColumns ) {
						clazz = typeof(IdentityGenerator);
					}// else if ( dialect.SupportsSequences ) {
					//	clazz = typeof(SequenceGenerator);
					//} else {
					//	clazz = typeof(TableHiLoGenerator);
					//}
				}
				if (clazz==null) clazz = System.Type.GetType(strategy);
				IIdentifierGenerator idgen = (IIdentifierGenerator) Activator.CreateInstance(clazz);
				if (idgen is IConfigurable) ((IConfigurable) idgen).Configure(type, parms, dialect);
				return idgen;
			} catch (Exception e) {
				throw new MappingException("could not instantiate id generator", e);
			}
		}

		internal static object CreateNumber(long value, System.Type type) {
			if (type==typeof(long)) {
				return value;
			} else if ( type==typeof(int)) {
				return (int) value;
			} else if ( type==typeof(short)) {
				return (short) value;
			} else {
				throw new IdentifierGenerationException("this id generator generates long, integer, short");
			}
		}
	}
}
