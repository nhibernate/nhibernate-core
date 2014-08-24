using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;
using System.Collections.Generic;

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

		private readonly IDictionary<string, object> parameters = new Dictionary<string, object>();

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
		public IFilter SetParameterList(string name, ICollection values)
		{
			// Make sure this is a defined parameter and check the incoming value type
			if (values == null)
			{
				throw new ArgumentException("Collection must be not null!", "values");
			}

			var type = definition.GetParameterType(name);
			if (type == null)
			{
				throw new HibernateException("Undefined filter parameter [" + name + "]");
			}

			if (values.Count > 0)
			{
				var e = values.GetEnumerator();
				e.MoveNext();
				if (!type.ReturnedClass.IsInstanceOfType(e.Current))
				{
					throw new HibernateException("Incorrect type for parameter [" + name + "]");
				}
			}
			parameters[name] = values;
			return this;
		}

		/// <summary>
		/// Set the named parameter's value list for this filter.  Used
		/// in conjunction with IN-style filter criteria.        
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <param name="values">The values to be expanded into an SQL IN list.</param>
		/// <returns>This FilterImpl instance (for method chaining).</returns>
		public IFilter SetParameterList(string name, object[] values)
		{
			return SetParameterList(name, new List<object>(values));
		}

		public object GetParameter(string name)
		{
			object result;
			parameters.TryGetValue(name, out result);
			return result;
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
