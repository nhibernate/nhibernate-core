using System;
using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// A FilterDefinition defines the global attributes of a dynamic filter.  This
	/// information includes its name as well as its defined parameters (name and type).
	/// </summary>
	[Serializable]
	public class FilterDefinition
	{
		private readonly string filterName;
		private readonly string defaultFilterCondition;
		private readonly IDictionary<string, IType> parameterTypes= new Dictionary<string, IType>();

		/// <summary>
		/// Set the named parameter's value list for this filter. 
		/// </summary>
		/// <param name="name">The name of the filter for which this configuration is in effect.</param>
		/// <param name="defaultCondition">The default filter condition.</param>
		/// <param name="parameterTypes">A dictionary storing the NHibernate <see cref="IType" /> type
		/// of each parameter under its name.</param>
		public FilterDefinition(string name, string defaultCondition, IDictionary<string, IType> parameterTypes)
		{
			filterName = name;
			defaultFilterCondition = defaultCondition;
			this.parameterTypes = parameterTypes;
		}

		/// <summary>
		/// Get the name of the filter this configuration defines.
		/// </summary>
		/// <returns>The filter name for this configuration.</returns>
		public string FilterName
		{
			get { return filterName; }
		}

		/// <summary>
		/// Get a set of the parameters defined by this configuration.
		/// </summary>
		/// <returns>The parameters named by this configuration.</returns>
		public ICollection<string> ParameterNames
		{
			get { return parameterTypes.Keys; }
		}

		/// <summary>
		/// Retreive the type of the named parameter defined for this filter.
		/// </summary>
		/// <param name="parameterName">The name of the filter parameter for which to return the type.</param>
		/// <returns>The type of the named parameter.</returns>
		public IType GetParameterType(string parameterName)
		{
			IType result;
			parameterTypes.TryGetValue(parameterName, out result);
			return result;
		}

		public string DefaultFilterCondition
		{
			get { return defaultFilterCondition; }
		}

		public IDictionary<string, IType> ParameterTypes
		{
			get { return parameterTypes; }
		}
	}
}
