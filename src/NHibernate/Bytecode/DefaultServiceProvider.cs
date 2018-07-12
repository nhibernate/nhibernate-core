using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// The default NHibernate service provider that uses <see cref="Activator.CreateInstance(System.Type)"/> to instantiate
	/// services by default.
	/// </summary>
	public class DefaultServiceProvider : IServiceProvider
	{
		private readonly ConcurrentDictionary<System.Type, Func<object>> _registeredTypeProviders =
			new ConcurrentDictionary<System.Type, Func<object>>();

		/// <inheritdoc />
		public object GetService(System.Type serviceType)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException(nameof(serviceType));
			}

			if (_registeredTypeProviders.TryGetValue(serviceType, out var serviceProvider))
			{
				return serviceProvider();
			}

			if (serviceType.IsInterface || serviceType.IsAbstract)
			{
				return null;
			}

			return Activator.CreateInstance(serviceType);
		}

		/// <summary>
		/// Register the specified delegate <paramref name="instanceCreator"/> that will be called when the
		/// type <typeparamref name="TService"/> is requested.
		/// </summary>
		/// <typeparam name="TService">The type to register.</typeparam>
		/// <param name="instanceCreator">The delegate taht creates the <typeparamref name="TService"/> instance.</param>
		public void Register<TService>(Func<TService> instanceCreator)
		{
			Register(typeof(TService), () => instanceCreator());
		}

		/// <summary>
		/// Register the specified delegate <paramref name="instanceCreator"/> that will be called when the
		/// type <paramref name="serviceType"/> is requested.
		/// </summary>
		/// <param name="serviceType">The type to register.</param>
		/// <param name="instanceCreator">The delegate that creates the <paramref name="serviceType"/> instance.</param>
		public void Register(System.Type serviceType, Func<object> instanceCreator)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException(nameof(serviceType));
			}

			if (instanceCreator == null)
			{
				throw new ArgumentNullException(nameof(instanceCreator));
			}

			if (!_registeredTypeProviders.TryAdd(serviceType, instanceCreator))
			{
				throw new InvalidOperationException($"Service type {serviceType} is already registered.");
			}
		}

		/// <summary>
		/// Register that an instance of <typeparamref name="TImplementation"/> will be returned when an
		/// instance of type <typeparamref name="TService"/> is requested.
		/// </summary>
		/// <typeparam name="TService">The type to register.</typeparam>
		/// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
		public void Register<TService, TImplementation>() where TImplementation : class
		{
			Register(typeof(TService), typeof(TImplementation));
		}

		/// <summary>
		/// Register that an instance of <paramref name="implementationType"/> will be returned when an
		/// instance of type <paramref name="serviceType"/> is requested.
		/// </summary>
		public void Register(System.Type serviceType, System.Type implementationType)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException(nameof(serviceType));
			}
			if (implementationType == null)
			{
				throw new ArgumentNullException(nameof(implementationType));
			}
			if (!serviceType.IsAssignableFrom(implementationType))
			{
				throw new InvalidOperationException(
					$"Implementation type {implementationType} is not assignable to service type {serviceType}.");
			}

			if (implementationType.IsAbstract || implementationType.IsInterface)
			{
				throw new InvalidOperationException($"Implementation type {implementationType} is not a concrete type.");
			}

			if (implementationType.GetConstructors().All(o => o.GetParameters().Length > 0))
			{
				throw new InvalidOperationException($"Implementation type {implementationType} does not have a parameterless constructor.");
			}

			Register(serviceType, () => Activator.CreateInstance(implementationType));
		}
	}
}
