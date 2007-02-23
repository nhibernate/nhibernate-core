using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;
#if NET_2_0
#endif

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

		private string filterName;
		private IDictionary parameters = new Hashtable();

		public void AfterDeserialize(FilterDefinition factoryDefinition)
		{
			definition = factoryDefinition;
		}

		public FilterImpl(FilterDefinition configuration)
		{
			this.definition = configuration;
			filterName = definition.FilterName;
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

		public IDictionary Parameters
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
			if (value != null && !type.ReturnedClass.IsAssignableFrom(value.GetType()))
			{
				throw new ArgumentException(name, "Incorrect type for parameter [" + name + "]");
			}
			parameters.Add(name, value);
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
				throw new ArgumentException("values", "Collection must be not null!");
			}
			IType type = definition.GetParameterType(name);
			if (type == null)
			{
				throw new HibernateException("Undefined filter parameter [" + name + "]");
			}
			if (values.Count > 0)
			{
				IEnumerator e = values.GetEnumerator();
				e.MoveNext();
				if (!type.ReturnedClass.IsAssignableFrom(e.Current.GetType()))
				{
					throw new HibernateException("Incorrect type for parameter [" + name + "]");
				}
			}
			parameters.Add(name, values);
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
			return SetParameterList(name, new ArrayList(values));
		}

		public object GetParameter(string name)
		{
			return parameters[name];
		}

		/// <summary>
		/// Perform validation of the filter state.  This is used to verify the
		/// state of the filter after its enablement and before its use.
		/// </summary>
		/// <returns></returns>
		public void Validate()
		{
			foreach (string parameterName in  definition.ParameterNames)
			{
				if (parameters[parameterName] == null)
				{
					throw new HibernateException(
						"Filter [" + Name + "] parameter [" + parameterName + "] value not set"
						);
				}
			}
		}
	}
}