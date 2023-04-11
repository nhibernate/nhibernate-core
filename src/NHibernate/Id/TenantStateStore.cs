using System;
using System.Collections.Concurrent;

namespace NHibernate.Id
{
	internal class TenantStateStore<TState> where TState : new()
	{
		private TState _noTenantState;
		private Action<TState> _initializer;
		private readonly Lazy<ConcurrentDictionary<string, TState>> _tenantSpecificState = new Lazy<ConcurrentDictionary<string, TState>>(() => new ConcurrentDictionary<string, TState>());

		public TenantStateStore(Action<TState> initializer)
		{
			_initializer = initializer;
		}

		public TenantStateStore()
		{
		}

		internal TState LocateGenerationState(string tenantIdentifier)
		{
			if (tenantIdentifier == null)
			{
				if (_noTenantState == null)
				{
					_noTenantState = CreateNewState();
				}
				return _noTenantState;
			}
			else
			{
				return _tenantSpecificState.Value.GetOrAdd(tenantIdentifier, _ => CreateNewState());
			}
		}

		internal TState NoTenantGenerationState => _noTenantState ??
			throw new HibernateException("Could not locate previous generation state for no-tenant");


		private TState CreateNewState()
		{
			var state = new TState();
			_initializer?.Invoke(state);
			return state;
		}
	}
}
