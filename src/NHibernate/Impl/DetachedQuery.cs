using System;
using NHibernate;

namespace NHibernate.Impl
{
	/// <summary>
	/// Query in "detached mode" where the NHibernate session is not available.
	/// </summary>
	/// <seealso cref="AbstractDetachedQuery"/>
	/// <seealso cref="IDetachedQuery"/>
	/// <seealso cref="IQuery"/>
	[Serializable]
	public class DetachedQuery: AbstractDetachedQuery
	{
		private readonly string hql;
		/// <summary>
		/// Create a new instance of <see cref="DetachedQuery"/> for the given query string.
		/// </summary>
		/// <param name="hql">A hibernate query string</param>
		public DetachedQuery(string hql)
		{
			this.hql = hql;
		}

		/// <summary>
		/// Get the HQL string.
		/// </summary>
		public string Hql
		{
			get { return hql; }
		}

		public override IQuery GetExecutableQuery(ISession session)
		{
			IQuery result = session.CreateQuery(hql);
			SetQueryProperties(result);
			return result;
		}
	}
}