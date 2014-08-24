using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Type;

namespace NHibernate.Engine.Query
{
	/// <summary> Encapsulates metadata about parameters encountered within a query. </summary>
	[Serializable]
	public class ParameterMetadata
	{
		private readonly Dictionary<string, NamedParameterDescriptor> namedDescriptorMap;
		private readonly OrdinalParameterDescriptor[] ordinalDescriptors;

		public ParameterMetadata(IEnumerable<OrdinalParameterDescriptor> ordinalDescriptors,
		                         IDictionary<string, NamedParameterDescriptor> namedDescriptorMap)
		{
			this.ordinalDescriptors = ordinalDescriptors == null ? Enumerable.Empty<OrdinalParameterDescriptor>().ToArray() : ordinalDescriptors.ToArray();

			this.namedDescriptorMap = namedDescriptorMap == null ? new Dictionary<string, NamedParameterDescriptor>(1) : new Dictionary<string, NamedParameterDescriptor>(namedDescriptorMap);
		}

		public int OrdinalParameterCount
		{
			get { return ordinalDescriptors.Length; }
		}

		public ICollection<string> NamedParameterNames
		{
			get { return namedDescriptorMap.Keys; }
		}

		public OrdinalParameterDescriptor GetOrdinalParameterDescriptor(int position)
		{
			if (position < 1 || position > ordinalDescriptors.Length)
			{
				throw new IndexOutOfRangeException("Remember that ordinal parameters are 1-based!");
			}
			return ordinalDescriptors[position - 1];
		}

		public IType GetOrdinalParameterExpectedType(int position)
		{
			return GetOrdinalParameterDescriptor(position).ExpectedType;
		}

		public NamedParameterDescriptor GetNamedParameterDescriptor(string name)
		{
			NamedParameterDescriptor meta;
			namedDescriptorMap.TryGetValue(name, out meta);
			if (meta == null)
			{
				throw new QueryParameterException("could not locate named parameter [" + name + "]");
			}

			return meta;
		}

		public IType GetNamedParameterExpectedType(string name)
		{
			return GetNamedParameterDescriptor(name).ExpectedType;
		}
	}
}