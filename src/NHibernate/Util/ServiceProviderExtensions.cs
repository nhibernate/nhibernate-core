using System;
using NHibernate.Bytecode;

namespace NHibernate.Util
{
	internal static class ServiceProviderExtensions
	{
		/// <summary>
		/// Get a service, throwing if it cannot be resolved.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		/// <param name="serviceType">The service interface, base class or concrete implementation class.</param>
		/// <returns>The service instance.</returns>
		/// <exception cref="ArgumentNullException">thrown if <paramref name="serviceType"/> is <see langword="null" />.</exception>
		/// <exception cref="HibernateServiceProviderException">thrown if the service cannot be resolved.</exception>
		public static object GetInstance(this IServiceProvider serviceProvider, System.Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));
			var service = serviceProvider.GetService(serviceType);

			if (service == null)
				throw new HibernateServiceProviderException(
					$"Unable to resolve an instance for {serviceType.AssemblyQualifiedName}");
			return service;
		}
	}
}
