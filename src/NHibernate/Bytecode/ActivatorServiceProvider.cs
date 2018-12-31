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
	/// services.
	/// </summary>
	public class ActivatorServiceProvider : IServiceProvider
	{
		/// <inheritdoc />
		public object GetService(System.Type serviceType)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException(nameof(serviceType));
			}

			if (serviceType.IsInterface || serviceType.IsAbstract)
			{
				return null;
			}

			return Activator.CreateInstance(serviceType);
		}
	}
}
