using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Parameters
{
	public class DynamicFilterParameterSpecification : IParameterSpecification
	{
		private readonly string _filterName;
		private readonly string _parameterName;
		private readonly IType _definedParameterType;

		/// <summary>
		/// Constructs a parameter specification for a particular filter parameter.
		/// </summary>
		/// <param name="filterName">The name of the filter</param>
		/// <param name="parameterName">The name of the parameter</param>
		/// <param name="definedParameterType">The paremeter type specified on the filter metadata</param>
		public DynamicFilterParameterSpecification(
				string filterName,
				string parameterName,
				IType definedParameterType)
		{
			_filterName = filterName;
			_parameterName = parameterName;
			_definedParameterType = definedParameterType;
		}

		public int Bind(IDbCommand statement, QueryParameters qp, ISessionImplementor session, int start)
		{
			int columnSpan = _definedParameterType.GetColumnSpan(session.Factory);

			object value = session.GetFilterParameterValue(_filterName + '.' + _parameterName);

			if (value is ICollection && !value.GetType().IsArray)
			{
				int positions = 0;

				foreach (var entry in (ICollection)value)
				{
					_definedParameterType.NullSafeSet(statement, entry, start + positions, session);
					positions += columnSpan;
				}

				return positions;
			}
			else
			{
				_definedParameterType.NullSafeSet(statement, value, start, session);
				return columnSpan;
			}
		}

		public IType ExpectedType
		{
			get { return _definedParameterType; }
			set { throw new InvalidOperationException(); }
		}

		public string RenderDisplayInfo()
		{
			return "dynamic-filter={filterName=" + _filterName + ",paramName=" + _parameterName + "}";
		}
	}
}
