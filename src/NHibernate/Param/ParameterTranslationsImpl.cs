using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Param
{
	/// <summary>
	/// Defines the information available for parameters encountered during
	/// query translation through the antlr-based parser.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public class ParameterTranslationsImpl : IParameterTranslations
	{
		private readonly Dictionary<string, ParameterInfo> _namedParameters;
		private readonly ParameterInfo[] _ordinalParameters;

		public ParameterTranslationsImpl(IEnumerable<IParameterSpecification> parameterSpecifications)
		{
			List<ParameterInfo> ordinalParameterList = new List<ParameterInfo>();
			NullableDictionary<string, NamedParamTempHolder> namedParameterMap = new NullableDictionary<string, NamedParamTempHolder>();

			int i = 0;
			foreach (IParameterSpecification spec in parameterSpecifications)
			{
				if ( spec is PositionalParameterSpecification) 
				{
					PositionalParameterSpecification ordinalSpec = ( PositionalParameterSpecification ) spec;
					ordinalParameterList.Add( new ParameterInfo( i, ordinalSpec.ExpectedType) );
				}
				else if ( spec is NamedParameterSpecification ) 
				{
					NamedParameterSpecification namedSpec = ( NamedParameterSpecification ) spec;
					NamedParamTempHolder paramHolder = namedParameterMap[namedSpec.Name];
					if ( paramHolder == null ) {
						paramHolder = new NamedParamTempHolder();
						paramHolder.name = namedSpec.Name;
						paramHolder.type = namedSpec.ExpectedType;
						namedParameterMap.Add( namedSpec.Name, paramHolder );
					}
					paramHolder.positions.Add( i );
				}
				else {
					// don't care about other param types here, just those explicitly user-defined...

					// Steve Strong Note:  The original Java does not do this decrement; it increments i for
					// every parameter type.  However, within the Loader.GetParameterTypes() method, this introduces
					// nulls into the paramTypeList array, which in turn causes Loader.ConvertITypesToSqlTypes() to crash
					// with a null dereference.  An alternative fix is to change the Loader to handle the null.  I'm
					// not sure which fix is the most appropriate.
					// Legacy.FumTest.CompositeIDQuery() shows the bug if you remove the decrement below...
					i--;
				}

				i++;
			}

			_ordinalParameters = ordinalParameterList.ToArray();
			_namedParameters = new Dictionary<string, ParameterInfo>();

			foreach (NamedParamTempHolder holder in namedParameterMap.Values)
			{
				_namedParameters.Add(holder.name, new ParameterInfo( ArrayHelper.ToIntArray( holder.positions ), holder.type ));
			}
		}

		public void AdjustNamedParameterLocationsForQueryParameters(QueryParameters parameters)
		{
			// NH Different behaviour NH-1776
			// Analyze all named parameters declared after filters 
			// in general all named parameters but depend on the complexity of the query (see sub query)
			RestoreOriginalParameterLocations();
			foreach (int filterParameterLocation in parameters.FilteredParameterLocations)
			{
				foreach (ParameterInfo entry in _namedParameters.Values)
				{
					entry.IncrementLocationAfterFilterLocation(filterParameterLocation);
				}
			}
		}

		public int GetOrdinalParameterSqlLocation(int ordinalPosition)
		{
			return GetOrdinalParameterInfo(ordinalPosition).SqlLocations[0];
		}

		public IType GetOrdinalParameterExpectedType(int ordinalPosition)
		{
			return GetOrdinalParameterInfo(ordinalPosition).ExpectedType;
		}

		public IEnumerable<string> GetNamedParameterNames()
		{
			return _namedParameters.Keys;
		}

		public int[] GetNamedParameterSqlLocations(string name)
		{
			return GetNamedParameterInfo(name).SqlLocations;
		}

		public IType GetNamedParameterExpectedType(string name)
		{
			return GetNamedParameterInfo(name).ExpectedType;
		}

		public bool SupportsOrdinalParameterMetadata
		{
			get { return true; }
		}

		public int OrdinalParameterCount
		{
			get { return _ordinalParameters.Length; }
		}

		private void RestoreOriginalParameterLocations()
		{
			foreach (ParameterInfo entry in _namedParameters.Values)
			{
				entry.RestoreOriginalParameterLocations();
			}
		}

		private ParameterInfo GetOrdinalParameterInfo(int ordinalPosition)
		{
			// remember that ordinal parameters numbers are 1-based!!!
			return _ordinalParameters[ordinalPosition - 1];
		}

		private ParameterInfo GetNamedParameterInfo(String name)
		{
			return _namedParameters[name];
		}

		class NamedParamTempHolder
		{
			internal String name;
			internal IType type;
			internal readonly List<int> positions = new List<int>();
		}
	}

	[Serializable]
	public class ParameterInfo 
	{
		private readonly int[] originalLocation;
		private readonly int[] sqlLocations;

		public ParameterInfo(int[] sqlPositions, IType expectedType) 
		{
			originalLocation = (int[])sqlPositions.Clone();
			sqlLocations = sqlPositions;
			ExpectedType = expectedType;
		}

		public ParameterInfo(int sqlPosition, IType expectedType)
		{
			originalLocation = new[] { sqlPosition };
			sqlLocations = new[] { sqlPosition };
			ExpectedType = expectedType;
		}

		public int[] SqlLocations
		{
			get { return sqlLocations; }
		}

		public IType ExpectedType { get; private set; }

		public void RestoreOriginalParameterLocations()
		{
			for (int i = 0; i < sqlLocations.Length; i++)
			{
				sqlLocations[i] = originalLocation[i];
			}
		}

		public void IncrementLocationAfterFilterLocation(int filterParameterLocation)
		{
			for (int i = 0; i < sqlLocations.Length; i++)
			{
				if (sqlLocations[i] >= filterParameterLocation)
				{
					sqlLocations[i]++;
				}
			}
		}
	}
}