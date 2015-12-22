using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Engine.Query;

namespace NHibernate.Impl
{
	public abstract partial class AbstractQueryImpl2 : AbstractQueryImpl
	{
		private readonly Dictionary<string, LockMode> _lockModes = new Dictionary<string, LockMode>(2);

		protected internal override IDictionary<string, LockMode> LockModes
		{
			get { return _lockModes; }
		}

		protected AbstractQueryImpl2(string queryString, FlushMode flushMode, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: base(queryString, flushMode, session, parameterMetadata)
		{
		}

		public override IQuery SetLockMode(string alias, LockMode lockMode)
		{
			_lockModes[alias] = lockMode;
			return this;
		}

		public override int ExecuteUpdate()
		{
			//FIX TO NH3079
			VerifyParameters(componentsParametersWillBeFlattened: false);
			var namedParams = NamedParams;
			Before();
			try
			{
				return Session.ExecuteUpdate(ExpandParameters(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

		public override IEnumerable Enumerable()
		{
			//FIX TO NH3079
			VerifyParameters(componentsParametersWillBeFlattened: false);
			var namedParams = NamedParams;
			Before();
			try
			{
				return Session.Enumerable(ExpandParameters(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

		public override IEnumerable<T> Enumerable<T>()
		{
			//FIX TO NH3079
			VerifyParameters(componentsParametersWillBeFlattened: false);
			var namedParams = NamedParams;
			Before();
			try
			{
				return Session.Enumerable<T>(ExpandParameters(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

		public override IList List()
		{
			//FIX TO NH3079
			VerifyParameters(componentsParametersWillBeFlattened: false);
			var namedParams = NamedParams;
			Before();
			try
			{
				return Session.List(ExpandParameters(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

		public override void List(IList results)
		{
			//FIX TO NH3079
			VerifyParameters(componentsParametersWillBeFlattened: false);
			var namedParams = NamedParams;
			Before();
			try
			{
				Session.List(ExpandParameters(namedParams), GetQueryParameters(namedParams), results);
			}
			finally
			{
				After();
			}
		}

		public override IList<T> List<T>()
		{
			//FIX TO NH3079
			VerifyParameters(componentsParametersWillBeFlattened: false);
			var namedParams = NamedParams;
			Before();
			try
			{
				return Session.List<T>(ExpandParameters(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

		/// <summary> 
		/// Warning: adds new parameters to the argument by side-effect, as well as mutating the query expression tree!
		/// </summary>
		protected abstract IQueryExpression ExpandParameters(IDictionary<string, TypedValue> namedParamsCopy);

		protected internal override IEnumerable<ITranslator> GetTranslators(ISessionImplementor sessionImplementor, QueryParameters queryParameters)
		{
			// NOTE: updates queryParameters.NamedParameters as (desired) side effect
			var queryExpression = ExpandParameters(queryParameters.NamedParameters);

			var plan = sessionImplementor.Factory.QueryPlanCache.GetHQLQueryPlan(queryExpression, false, sessionImplementor.EnabledFilters);
			return plan.Translators.Select(t => new HqlTranslatorWrapper(t));
		}
	}
}
