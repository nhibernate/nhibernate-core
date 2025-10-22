using System;
using System.Collections.Concurrent;

namespace NHibernate.Id
{
	internal class TenantStateStore<TState> where TState : new()
	{
		private Lazy<TState> _noTenantState;
		private Action<TState> _initializer;
		private ConcurrentDictionary<string, TState> _tenantSpecificState = new ConcurrentDictionary<string, TState>();

		public TenantStateStore(Action<TState> initializer) : this()
		{
			_initializer = initializer;
		}

		public TenantStateStore()
		{
			_noTenantState = new Lazy<TState>(() => CreateNewState());
		}

		internal TState LocateGenerationState(string tenantIdentifier)
		{
			if (tenantIdentifier == null)
			{
				return _noTenantState.Value;
			}
			else
			{
				return _tenantSpecificState.GetOrAdd(tenantIdentifier, _ => CreateNewState());
			}
		}

		internal TState NoTenantGenerationState => _noTenantState.IsValueCreated ?
			_noTenantState.Value :
			throw new HibernateException("Could not locate previous generation state for no-tenant");


		private TState CreateNewState()
		{
			var state = new TState();
			_initializer?.Invoke(state);
			return state;
		}
	}
}
