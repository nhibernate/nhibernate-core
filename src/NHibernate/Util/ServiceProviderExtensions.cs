using System;
using System.Reflection;
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
		public static object GetMandatoryService(this IServiceProvider serviceProvider, System.Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));
			var service = serviceProvider.GetService(serviceType);
			if (service != null)
			{
				return service;
			}

			// Some IoC containers require explicit registration for concete types. In order to avoid registering all NHibernate types
			// the Activator.CreateInstance is used for them.
			Exception innerException = null;
			if (serviceType.IsClass && !serviceType.IsAbstract)
			{
				try
				{
					return Activator.CreateInstance(serviceType);
				}
				catch (Exception e)
				{
					innerException = e;
				}
			}

			throw new HibernateServiceProviderException(
				$"Unable to resolve an instance for {serviceType.AssemblyQualifiedName}, " +
				"make sure that the service is registered and a non-null value is returned for it.", innerException);
		}
	}
}
