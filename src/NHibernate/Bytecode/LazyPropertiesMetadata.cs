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
			return new LazyPropertiesMetadata(entityName, null);
		}

		public static LazyPropertiesMetadata From(
			string entityName,
			IEnumerable<LazyPropertyDescriptor> lazyPropertyDescriptors)
		{
			// TODO: port lazy fetch groups
			return new LazyPropertiesMetadata(
				entityName,
				lazyPropertyDescriptors.ToDictionary(o => o.Name));
		}

		private readonly IDictionary<string, LazyPropertyDescriptor> _lazyPropertyDescriptors;

		public LazyPropertiesMetadata(
			string entityName,
			IDictionary<string, LazyPropertyDescriptor> lazyPropertyDescriptors)
		{
			EntityName = entityName;
			_lazyPropertyDescriptors = lazyPropertyDescriptors;
			HasLazyProperties = _lazyPropertyDescriptors?.Count > 0;
			LazyPropertyNames = HasLazyProperties
				? new ReadOnlySet<string>(new HashSet<string>(_lazyPropertyDescriptors.Keys))
				: CollectionHelper.EmptySet<string>();
			// TODO: port lazy fetch groups
		}

		public string EntityName { get; }

		public bool HasLazyProperties { get; }

		public ISet<string> LazyPropertyNames { get; }

		public IEnumerable<LazyPropertyDescriptor> LazyPropertyDescriptors =>
			_lazyPropertyDescriptors?.Values ?? Enumerable.Empty<LazyPropertyDescriptor>();

		/// <summary>
		/// Get the descriptor for the lazy property.
		/// </summary>
		/// <param name="propertyName">The propery name.</param>
		/// <returns>The index of the property.</returns>
		public LazyPropertyDescriptor GetLazyPropertyDescriptor(string propertyName)
		{
			if (!_lazyPropertyDescriptors.TryGetValue(propertyName, out var descriptor))
			{
				throw new InvalidOperationException(
					$"Property {propertyName} is not mapped as lazy on entity {EntityName}");
			}

			return descriptor;
		}
	}
}
