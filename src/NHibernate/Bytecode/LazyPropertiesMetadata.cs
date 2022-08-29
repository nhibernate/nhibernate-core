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
			IEnumerable<LazyPropertyDescriptor> lazyPropertyDescriptors)
		{
			var lazyProperties = new Dictionary<string, LazyPropertyDescriptor>();
			var fetchGroups = new Dictionary<string, ISet<string>>();
			var mutableFetchGroups = new Dictionary<string, ISet<string>>();

			foreach (var propertyDescriptor in lazyPropertyDescriptors)
			{
				lazyProperties.Add(propertyDescriptor.Name, propertyDescriptor);
				if (!mutableFetchGroups.TryGetValue(propertyDescriptor.FetchGroupName, out var fetchGroup))
				{
					fetchGroup = new HashSet<string>();
					mutableFetchGroups.Add(propertyDescriptor.FetchGroupName, fetchGroup);
					fetchGroups.Add(propertyDescriptor.FetchGroupName, new ReadOnlySet<string>(fetchGroup));
				}

				fetchGroup.Add(propertyDescriptor.Name);
			}

			return new LazyPropertiesMetadata(
				entityName,
				lazyProperties,
				fetchGroups);
		}

		private readonly IDictionary<string, LazyPropertyDescriptor> _lazyPropertyDescriptors;
		private readonly IDictionary<string, ISet<string>> _fetchGroups;

		public LazyPropertiesMetadata(
			string entityName,
			IDictionary<string, LazyPropertyDescriptor> lazyPropertyDescriptors,
			IDictionary<string, ISet<string>> fetchGroups)
		{
			EntityName = entityName;
			_lazyPropertyDescriptors = lazyPropertyDescriptors;
			HasLazyProperties = _lazyPropertyDescriptors?.Count > 0;
			LazyPropertyNames = HasLazyProperties
				? new ReadOnlySet<string>(new HashSet<string>(_lazyPropertyDescriptors.Keys))
				: CollectionHelper.EmptySet<string>();

			_fetchGroups = fetchGroups;
			FetchGroupNames = _fetchGroups?.Count > 0
				? new ReadOnlySet<string>(new HashSet<string>(_fetchGroups.Keys))
				: CollectionHelper.EmptySet<string>();
		}

		public string EntityName { get; }

		public bool HasLazyProperties { get; }

		public ISet<string> LazyPropertyNames { get; }

		public ISet<string> FetchGroupNames { get; }

		public IEnumerable<LazyPropertyDescriptor> LazyPropertyDescriptors =>
			_lazyPropertyDescriptors?.Values ?? Enumerable.Empty<LazyPropertyDescriptor>();

		/// <summary>
		/// Get the descriptor for the lazy property.
		/// </summary>
		/// <param name="propertyName">The propery name.</param>
		/// <returns>The lazy property descriptor.</returns>
		public LazyPropertyDescriptor GetLazyPropertyDescriptor(string propertyName)
		{
			if (!_lazyPropertyDescriptors.TryGetValue(propertyName, out var descriptor))
			{
				throw new InvalidOperationException(
					$"Property {propertyName} is not mapped as lazy on entity {EntityName}");
			}

			return descriptor;
		}

		public string GetFetchGroupName(string propertyName)
		{
			return GetLazyPropertyDescriptor(propertyName).FetchGroupName;
		}

		public ISet<string> GetPropertiesInFetchGroup(string groupName)
		{
			if (!_fetchGroups.TryGetValue(groupName, out var properties))
			{
				throw new InvalidOperationException(
					$"Fetch group {groupName} does not exist for entity {EntityName}");
			}

			return properties;
		}

		public IEnumerable<LazyPropertyDescriptor> GetFetchGroupPropertyDescriptors(string groupName)
		{
			foreach (var propertyName in GetPropertiesInFetchGroup(groupName))
			{
				yield return GetLazyPropertyDescriptor(propertyName);
			}
		}
	}
}
