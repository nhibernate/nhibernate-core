using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Engine.Query
{
	/// <summary> Encapsulates metadata about parameters encountered within a query. </summary>
	[Serializable]
	public class ParameterMetadata
	{
		private static readonly OrdinalParameterDescriptor[] EmptyOrdinals = new OrdinalParameterDescriptor[0];
		private readonly OrdinalParameterDescriptor[] ordinalDescriptors;
		private readonly Dictionary<string, NamedParameterDescriptor> namedDescriptorMap;

		public ParameterMetadata(OrdinalParameterDescriptor[] ordinalDescriptors,
			IDictionary<string, NamedParameterDescriptor> namedDescriptorMap)
		{
			if (ordinalDescriptors == null)
			{
				this.ordinalDescriptors = EmptyOrdinals;
			}
			else
			{
				OrdinalParameterDescriptor[] copy = new OrdinalParameterDescriptor[ordinalDescriptors.Length];
				Array.Copy(ordinalDescriptors, 0, copy, 0, ordinalDescriptors.Length);
				this.ordinalDescriptors = copy;
			}

			if (namedDescriptorMap == null)
				this.namedDescriptorMap = new Dictionary<string, NamedParameterDescriptor>();
			else
				this.namedDescriptorMap = new Dictionary<string, NamedParameterDescriptor>(namedDescriptorMap);
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

		public int GetOrdinalParameterSourceLocation(int position)
		{
			return GetOrdinalParameterDescriptor(position).SourceLocation;
		}

		public NamedParameterDescriptor GetNamedParameterDescriptor(string name)
		{
			NamedParameterDescriptor meta;
			namedDescriptorMap.TryGetValue(name, out meta);
			if (meta == null)
				throw new QueryParameterException("could not locate named parameter [" + name + "]");

			return meta;
		}

		public IType GetNamedParameterExpectedType(string name)
		{
			return GetNamedParameterDescriptor(name).ExpectedType;
		}

		public int[] GetNamedParameterSourceLocations(string name)
		{
			return GetNamedParameterDescriptor(name).SourceLocations;
		}

	}
}
