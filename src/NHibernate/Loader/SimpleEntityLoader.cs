using System;
using System.Text;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader {
	/// <summary>
	/// Loads entity instances one instance per select (ie without outerjoin fetching)
	/// </summary>
	public class SimpleEntityLoader : Loader, IUniqueEntityLoader {
		private ILoadable[] persister;
		private IType[] idType;
		private string sql;
		private LockMode lockMode;
		private string[] NoSuffix = new string[] { StringHelper.EmptyString };

		public SimpleEntityLoader(ILoadable persister, string sql, LockMode lockMode) {
			this.persister = new ILoadable[] { persister };
			this.idType = new IType[] { persister.IdentifierType };
			this.sql = sql;
			this.lockMode = lockMode;
		}

		public override string SQLString {
			get { return sql; }
		}

		public override ILoadable[] Persisters {
			get { return persister; }
		}
		protected override CollectionPersister CollectionPersister {
			get { return null; }
		}
		public override string[] Suffixes {
			get { return NoSuffix; }
			set { throw new NotImplementedException(); }
		}
		protected override LockMode LockMode {
			get { return lockMode; }
		}

		public object Load(ISessionImplementor session, object id, object obj) {
			IList list = LoadEntity(session, new object[] { id }, idType, obj, id, false);
			if (list.Count==1) {
				return ( (object[]) list[0] )[0];
			} else if (list.Count==0) {
				return null;
			} else {
				throw new HibernateException("More than one row with the given identifier was found: " + id + ", for class: " + persister[0].ClassName );
			}
		}
	}
}
