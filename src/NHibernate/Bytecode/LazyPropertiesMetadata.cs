using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iesi.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Information about all of the bytecode lazy properties for an entity
	/// 
	/// Author: Steve Ebersole
	/// </summary>
	[Serializable]
	public class LazyPropertiesMetadata
	{
		public static LazyPropertiesMetadata NonEnhanced(string entityName)
		{
			return new LazyPropertiesMetadata(entityName, null, null);
		}

		public static LazyPropertiesMetadata From(
			string entityName,
			IEnumerable<LazyPropertyDescriptor> lazyPropertyDescriptors,
			ISet<string> unwrapProxyPropertyNames)
		{
			// TODO: port lazy fetch groups
			return new LazyPropertiesMetadata(
				entityName,
				lazyPropertyDescriptors.ToDictionary(o => o.Name),
				unwrapProxyPropertyNames);
		}

		private readonly IDictionary<string, LazyPropertyDescriptor> _lazyPropertyDescriptors;

		public LazyPropertiesMetadata(
			string entityName,
			IDictionary<string, LazyPropertyDescriptor> lazyPropertyDescriptors,
			ISet<string> unwrapProxyPropertyNames)
		{
			EntityName = entityName;
			_lazyPropertyDescriptors = lazyPropertyDescriptors;
			HasLazyProperties = _lazyPropertyDescriptors?.Count > 0;
			LazyPropertyNames = HasLazyProperties
				? new ReadOnlySet<string>(new HashSet<string>(_lazyPropertyDescriptors.Keys))
				: CollectionHelper.EmptySet<string>();
			HasUnwrapProxyProperties = unwrapProxyPropertyNames?.Count > 0;
			UnwrapProxyPropertyNames = HasUnwrapProxyProperties
				? new ReadOnlySet<string>(unwrapProxyPropertyNames)
				: CollectionHelper.EmptySet<string>();
			// TODO: port lazy fetch groups
		}

		public string EntityName { get; }

		public bool HasLazyProperties { get; }

		public bool HasUnwrapProxyProperties { get; }

		public ISet<string> LazyPropertyNames { get; }

		public ISet<string> UnwrapProxyPropertyNames { get; }

		public IEnumerable<LazyPropertyDescriptor> LazyPropertyDescriptors =>
			_lazyPropertyDescriptors?.Values ?? Enumerable.Empty<LazyPropertyDescriptor>();
	}
}
