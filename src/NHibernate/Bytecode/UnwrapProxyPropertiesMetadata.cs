using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Information about all properties mapped as lazy="no-proxy" for an entity
	/// </summary>
	public class UnwrapProxyPropertiesMetadata
	{
		public static UnwrapProxyPropertiesMetadata NonEnhanced(string entityName)
		{
			return new UnwrapProxyPropertiesMetadata(entityName, null);
		}

		public static UnwrapProxyPropertiesMetadata From(
			string entityName,
			IEnumerable<UnwrapProxyPropertyDescriptor> unwrapProxyPropertyDescriptors)
		{
			return new UnwrapProxyPropertiesMetadata(
				entityName,
				unwrapProxyPropertyDescriptors.ToDictionary(o => o.Name));
		}

		private readonly IDictionary<string, UnwrapProxyPropertyDescriptor> _unwrapProxyPropertyDescriptors;

		public UnwrapProxyPropertiesMetadata(
			string entityName,
			IDictionary<string, UnwrapProxyPropertyDescriptor> unwrapProxyPropertyDescriptors)
		{
			EntityName = entityName;
			_unwrapProxyPropertyDescriptors = unwrapProxyPropertyDescriptors;
			HasUnwrapProxyProperties = unwrapProxyPropertyDescriptors?.Count > 0;
			UnwrapProxyPropertyNames = HasUnwrapProxyProperties
				? new ReadOnlySet<string>(new HashSet<string>(unwrapProxyPropertyDescriptors.Keys))
				: CollectionHelper.EmptySet<string>();
		}

		public string EntityName { get; }

		public bool HasUnwrapProxyProperties { get; }

		public ISet<string> UnwrapProxyPropertyNames { get; }

		public IEnumerable<UnwrapProxyPropertyDescriptor> UnwrapProxyPropertyDescriptors =>
			_unwrapProxyPropertyDescriptors?.Values ?? Enumerable.Empty<UnwrapProxyPropertyDescriptor>();

		public int GetUnwrapProxyPropertyIndex(string propertyName)
		{
			if (!_unwrapProxyPropertyDescriptors.TryGetValue(propertyName, out var descriptor))
			{
				throw new InvalidOperationException(
					$"Property {propertyName} is not mapped as lazy=\"no-proxy\" on entity {EntityName}");
			}

			return descriptor.PropertyIndex;
		}
	}
}
