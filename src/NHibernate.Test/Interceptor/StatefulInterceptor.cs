using System.Collections;
using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Test.Interceptor
{
	public class StatefulInterceptor : EmptyInterceptor
	{
		private ISession session;
		private readonly IList<Log> list = new List<Log>();

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			if (!(entity is Log))
			{
				list.Add(new Log("insert", (string) id, entity.GetType().FullName));
			}
			return false;
		}

		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
		                                  string[] propertyNames, IType[] types)
		{
			if (!(entity is Log))
			{
				list.Add(new Log("update", (string) id, entity.GetType().FullName));
			}
			return false;
		}

		public override void PostFlush(ICollection entities)
		{
			if (list.Count > 0)
			{
				foreach (Log iter in list)
				{
					session.Persist(iter);
				}
				list.Clear();
				session.Flush();
			}
		}

		public override void SetSession(ISession sessionLocal)
		{
			session = sessionLocal;
		}

		public ISession Session
		{
			get { return session; }
		}
	}
}