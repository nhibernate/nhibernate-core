using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	internal static class ServiceProviderExtensions
	{
		public static object GetInstance(this IServiceProvider serviceProvider, System.Type serviceType)
		{
			// 6.0 TODO throw a meaningful exception instead of using the Activator
			return serviceProvider.GetService(serviceType) ?? Activator.CreateInstance(serviceType);
		}
	}
}
