using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// </summary>
	[Serializable]
	public class FilterImpl : IFilter
	{
		public static readonly string MARKER = "$FILTER_PLACEHOLDER$";

		[NonSerialized]
		private FilterDefinition definition;

		private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();
		private readonly Dictionary<string, int> _parameterSpans = new Dictionary<string, int>();

		public void AfterDeserialize(FilterDefinition factoryDefinition)
		{
			definition = factoryDefinition;
		}

		public FilterImpl(FilterDefinition configuration)
		{
			definition = configuration;
		}

		public FilterDefinition FilterDefinition
		{
			get { return definition; }
		}

		/// <summary>
		/// Get the name of this filter.
		/// </summary>
		public string Name
		{
			get { return definition.FilterName; }
		}

		public IDictionary<string, object> Parameters
		{
			get { return parameters; }
		}

		/// <summary>
		/// Set the named parameter's value for this filter.
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <param name="value">The value to be applied.</param>
		/// <returns>This FilterImpl instance (for method chaining).</returns>
		public IFilter SetParameter(string name, object value)
		{
			// Make sure this is a defined parameter and check the incoming value type
			// TODO: what should be the actual exception type here?
			IType type = definition.GetParameterType(name);
			if (type == null)
			{
				throw new ArgumentException(name, "Undefined filter parameter [" + name + "]");
			}
			if (value != null && !type.ReturnedClass.IsInstanceOfType(value))
			{
				throw new ArgumentException(name, "Incorrect type for parameter [" + name + "]");
			}
			parameters[name] = value;
			return this;
		}

		/// <summary>
		/// Set the named parameter's value list for this filter.  Used
		/// in conjunction with IN-style filter criteria.
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <param name="values">The values to be expanded into an SQL IN list.</param>
		/// <returns>This FilterImpl instance (for method chaining).</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="values"/> are <see langword="null" />.</exception>
		public IFilter SetParameterList<T>(string name, ICollection<T> values)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values), "Collection must be not null!");

			var type = definition.GetParameterType(name);
			if (type == null)
				throw new HibernateException("Undefined filter parameter [" + name + "]");

			if (!type.ReturnedClass.IsAssignableFrom(typeof(T)))
				throw new HibernateException("Incorrect type for parameter [" + name + "]");

			_parameterSpans[name] = values.Count;
			parameters[name] = values;
			return this;
		}

		public object GetParameter(string name)
		{
			object result;
			parameters.TryGetValue(name, out result);
			return result;
		}

		/// <summary>
		/// Get the span of a value list parameter by name. <see langword="null" /> if the parameter is not a value list
		/// or if there is no such parameter.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <returns>The parameter span, or <see langword="null" /> if the parameter is not a value list or
		/// if there is no such parameter.</returns>
		public int? GetParameterSpan(string name)
		{
			return _parameterSpans.TryGetValue(name, out var result) ? result : default(int?);
		}

		/// <summary>
		/// Perform validation of the filter state.  This is used to verify the
		/// state of the filter after its enablement and before its use.
		/// </summary>
		public void Validate()
		{
			foreach (string parameterName in definition.ParameterNames)
			{
				if (!parameters.ContainsKey(parameterName))
					throw new HibernateException(string.Format("Filter [{0}] parameter [{1}] value not set", Name, parameterName));
			}
		}
	}
}
