using System;
using System.Collections;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Transform
{
	/// <summary>
	/// A ResultTransformer that is used to transform tuples to a value(s) that can be cached.
	/// </summary>
	/// @author Gail Badner
	[Serializable]
	public class CacheableResultTransformer : IResultTransformer
	{
		// Applies to Java original:
		// would be nice to be able to have this class extend
		// PassThroughResultTransformer, but the default constructor
		// is private (as it should be for a singleton)
		//private const PassThroughResultTransformer ACTUAL_TRANSFORMER =
		//    PassThroughResultTransformer.INSTANCE;
		private readonly PassThroughResultTransformer _actualTransformer = PassThroughResultTransformer.Instance;

		public bool AutoDiscoverTypes { get; }

		private readonly SqlString _autoDiscoveredQuery;
		private readonly bool _skipTransformer;
		private int _tupleLength;
		private int _tupleSubsetLength;

		/// <summary>
		/// Array with the i-th element indicating whether the i-th
		/// expression returned by a query is included in the tuple.
		/// </summary>
		/// IMPLEMENTATION NOTE:
		/// "joined" and "fetched" associations may use the same SQL,
		/// but result in different tuple and cached values. This is
		/// because "fetched" associations are excluded from the tuple.
		///  includeInTuple provides a way to distinguish these 2 cases.
		private bool[] _includeInTuple;

		/// <summary>
		/// Indexes for tuple that are included in the transformation.
		/// Set to null if all elements in the tuple are included.
		/// </summary>
		private int[] _includeInTransformIndex;

		/// <summary>
		/// Returns a CacheableResultTransformer that is used to transform
		/// tuples to a value(s) that can be cached.
		/// </summary>
		/// <param name="transformer">result transformer that will ultimately be used (after caching results)</param>
		/// <param name="aliases">the aliases that correspond to the tuple;
		///   if it is non-null, its length must equal the number
		///   of true elements in includeInTuple[]</param>
		/// <param name="includeInTuple">array with the i-th element indicating
		///   whether the i-th expression returned by a query is
		///   included in the tuple; the number of true values equals
		///   the length of the tuple that will be transformed;
		///   must be non-null</param>
		/// <returns>a CacheableResultTransformer that is used to transform
		///    tuples to a value(s) that can be cached.</returns>
		// Since v5.1
		[Obsolete("Please use overload with skipTransformer parameter.")]
		public static CacheableResultTransformer Create(IResultTransformer transformer,
		                                                string[] aliases,
		                                                bool[] includeInTuple)
		{
			return Create(transformer, aliases, includeInTuple, false, null);
		}

		// Since 5.2
		[Obsolete("Please use override with skipTransformer parameter.")]
		public static CacheableResultTransformer Create(
			IResultTransformer transformer, string[] aliases, bool[] includeInTuple, bool autoDiscoverTypes,
			SqlString autoDiscoveredQuery)
		{
			return autoDiscoverTypes
				? Create(autoDiscoveredQuery)
				: Create(includeInTuple, GetIncludeInTransform(transformer, aliases, includeInTuple), false);
		}

		/// <summary>
		/// Returns a CacheableResultTransformer that is used to transform
		/// tuples to a value(s) that can be cached.
		/// </summary>
		/// <param name="transformer">result transformer that will ultimately be used (after caching results)</param>
		/// <param name="aliases">the aliases that correspond to the tuple;
		///   if it is non-null, its length must equal the number
		///   of true elements in includeInTuple[]</param>
		/// <param name="includeInTuple">array with the i-th element indicating
		///   whether the i-th expression returned by a query is
		///   included in the tuple; the number of true values equals
		///   the length of the tuple that will be transformed;
		///   must be non-null</param>
		/// <param name="autoDiscoverTypes">Indicates if types auto-discovery is enabled.</param>
		/// <param name="autoDiscoveredQuery">If <paramref name="autoDiscoverTypes"/>, the query for which they
		/// will be autodiscovered.</param>
		/// <param name="skipTransformer">If true cache results untransformed.</param>
		/// <returns>a CacheableResultTransformer that is used to transform
		///    tuples to a value(s) that can be cached.</returns>
		public static CacheableResultTransformer Create(
			IResultTransformer transformer,
			string[] aliases,
			bool[] includeInTuple,
			bool autoDiscoverTypes,
			SqlString autoDiscoveredQuery,
			bool skipTransformer)
		{
			return autoDiscoverTypes
				? Create(autoDiscoveredQuery)
				: Create(
					includeInTuple,
					skipTransformer
						? null
						: GetIncludeInTransform(transformer, aliases, includeInTuple),
					skipTransformer);
		}

		/// <summary>
		/// Returns a CacheableResultTransformer that is used to transform
		/// tuples to a value(s) that can be cached.
		/// </summary>
		/// <param name="includeInTuple">array with the i-th element indicating
		///   whether the i-th expression returned by a query is
		///   included in the tuple; the number of true values equals
		///   the length of the tuple that will be transformed;
		///   must be non-null</param>
		/// <param name="includeInTransform">Indexes that are included in the transformation.
		/// <c>null</c> if all elements in the tuple are included.</param>
		/// <param name="skipTransformer"></param>
		/// <returns>a CacheableResultTransformer that is used to transform
		///    tuples to a value(s) that can be cached.</returns>
		private static CacheableResultTransformer Create(bool[] includeInTuple, bool[] includeInTransform, bool skipTransformer)
		{
			return new CacheableResultTransformer(includeInTuple, includeInTransform, skipTransformer);
		}

		private static CacheableResultTransformer Create(SqlString autoDiscoveredQuery)
		{
			return new CacheableResultTransformer(autoDiscoveredQuery);
		}

		private static bool[] GetIncludeInTransform(IResultTransformer transformer, string[] aliases, bool[] includeInTuple)
		{
			if (!(transformer is ITupleSubsetResultTransformer resultTransformer))
				return null;

			var tupleLength = ArrayHelper.CountTrue(includeInTuple);
			if (aliases != null && aliases.Length != tupleLength)
			{
				throw new ArgumentException(
					$"If {nameof(aliases)} is not null, then the length of {nameof(aliases)}[] must equal the number " +
					$"of true elements in {nameof(includeInTuple)}; " +
					$"{nameof(aliases)}.Length={aliases.Length}; tuple 'true' length={tupleLength}",
					nameof(aliases));
			}

			return resultTransformer.IncludeInTransform(aliases, tupleLength);
		}

		private CacheableResultTransformer(bool[] includeInTuple, bool[] includeInTransform, bool skipTransformer)
		{
			_skipTransformer = skipTransformer;
			InitializeTransformer(includeInTuple, includeInTransform);
		}

		private CacheableResultTransformer(SqlString autoDiscoveredQuery)
		{
			AutoDiscoverTypes = true;
			_autoDiscoveredQuery = autoDiscoveredQuery;
		}

		private void InitializeTransformer(bool[] includeInTuple, bool[] includeInTransform)
		{
			_includeInTuple = includeInTuple ?? throw new ArgumentNullException(nameof(includeInTuple));
			_tupleLength = ArrayHelper.CountTrue(includeInTuple);
			_tupleSubsetLength = includeInTransform == null
				? _tupleLength
				: ArrayHelper.CountTrue(includeInTransform);
			if (_tupleSubsetLength == _tupleLength)
			{
				_includeInTransformIndex = null;
			}
			else
			{
				_includeInTransformIndex = new int[_tupleSubsetLength];
				for (int i = 0, j = 0; i < includeInTransform.Length; i++)
				{
					if (includeInTransform[i])
					{
						_includeInTransformIndex[j] = i;
						j++;
					}
				}
			}
		}

		internal void SupplyAutoDiscoveredParameters(IResultTransformer transformer, string[] aliases)
		{
			if (!AutoDiscoverTypes)
				throw new InvalidOperationException(
					"Cannot supply auto-discovered parameters when it is not enabled on the transformer.");

			var includeInTuple = ArrayHelper.Fill(true, aliases?.Length ?? 0);
			InitializeTransformer(includeInTuple, GetIncludeInTransform(transformer, aliases, includeInTuple));
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			if (_includeInTuple == null)
				throw new InvalidOperationException("This transformer is not initialized");
			return _actualTransformer.TransformTuple(Index(tuple), null);
		}

		/// <summary>
		/// Re-transforms, if necessary, a List of values previously
		/// transformed by this (or an equivalent) CacheableResultTransformer.
		/// Each element of the list is re-transformed in place (i.e, List
		/// elements are replaced with re-transformed values) and the original
		/// List is returned. If re-transformation is unnecessary, the original List is returned
		/// unchanged.
		/// </summary>
		/// <param name="transformedResults">Results that were previously transformed.</param>
		/// <param name="aliases">The aliases that correspond to the untransformed tuple.</param>
		/// <param name="transformer">The transformer for the re-transformation.</param>
		/// <param name="includeInTuple"></param>
		/// <returns>transformedResults, with each element re-transformed (if necessary).</returns>
		public IList RetransformResults(IList transformedResults,
		                                string[] aliases,
		                                IResultTransformer transformer,
		                                bool[] includeInTuple)
		{
			if (transformer == null)
				throw new ArgumentNullException(nameof(transformer));
			if (_includeInTuple == null)
				throw new InvalidOperationException("This transformer is not initialized");

			if (!HasSameParameters(Create(transformer, aliases, includeInTuple, false, null, _skipTransformer)))
			{
				throw new InvalidOperationException(
					"this CacheableResultTransformer is inconsistent with specified arguments; cannot re-transform"
					);
			}
			bool requiresRetransform = true;
			string[] aliasesToUse = aliases == null ? null : Index(aliases);

			if (_skipTransformer)
			{
			}
			else if (transformer.Equals(_actualTransformer))
			{
				requiresRetransform = false;
			}
			else if (transformer is ITupleSubsetResultTransformer)
			{
				requiresRetransform =
					!((ITupleSubsetResultTransformer) transformer).IsTransformedValueATupleElement(aliasesToUse, _tupleLength);
			}

			if (requiresRetransform)
			{
				for (int i = 0; i < transformedResults.Count; i++)
				{
					object[] tuple = _actualTransformer.UntransformToTuple(
						transformedResults[i],
						_tupleSubsetLength == 1
						);
					transformedResults[i] = transformer.TransformTuple(tuple, aliasesToUse);
				}
			}

			return transformedResults;
		}

		/// <summary>
		/// Untransforms, if necessary, a List of values previously
		/// transformed by this (or an equivalent) CacheableResultTransformer.
		/// Each element of the list is untransformed in place (i.e, List
		/// elements are replaced with untransformed values) and the original
		/// List is returned.
		/// <para>
		/// If not unnecessary, the original List is returned
		/// unchanged.
		/// </para>
		/// </summary>
		/// <remarks>
		/// NOTE: If transformed values are a subset of the original
		/// tuple, then, on return, elements corresponding to
		/// excluded tuple elements will be null.
		/// </remarks>
		/// <param name="results">Results that were previously transformed.</param>
		/// <returns>results, with each element untransformed (if necessary).</returns>
		public IList UntransformToTuples(IList results)
		{
			if (_includeInTuple == null)
				throw new InvalidOperationException("This transformer is not initialized");
			if (_includeInTransformIndex == null)
			{
				results = _actualTransformer.UntransformToTuples(
					results,
					_tupleSubsetLength == 1
					);
			}
			else
			{
				for (int i = 0; i < results.Count; i++)
				{
					object[] tuple = _actualTransformer.UntransformToTuple(
						results[i], _tupleSubsetLength == 1);
					results[i] = Unindex(tuple);
				}
			}

			return results;
		}

		/// <summary>
		/// Returns the result types for the transformed value.
		/// </summary>
		public IType[] GetCachedResultTypes(IType[] tupleResultTypes)
		{
			if (_includeInTuple == null)
				throw new InvalidOperationException("This transformer is not initialized");
			return _tupleLength != _tupleSubsetLength
				       ? Index(tupleResultTypes)
				       : tupleResultTypes;
		}

		public IList TransformList(IList list)
		{
			return list;
		}

		/// <summary>
		/// "Compact" the given array by picking only the elements identified by
		/// the _includeInTransformIndex array. The picked elements are returned
		/// in a new array.
		/// </summary>
		private T[] Index<T>(T[] objects)
		{
			T[] objectsIndexed = objects;
			if (objects != null &&
			    _includeInTransformIndex != null &&
			    objects.Length != _tupleSubsetLength)
			{
				objectsIndexed = new T[_tupleSubsetLength];
				for (int i = 0; i < _tupleSubsetLength; i++)
				{
					objectsIndexed[i] = objects[_includeInTransformIndex[i]];
				}
			}
			return objectsIndexed;
		}

		/// <summary>
		/// Expand the given array by putting each of its elements at the
		/// position identified by the _includeInTransformIndex array. The
		/// elements are placed in a new array - the original array will
		/// not be modified.
		/// </summary>
		private T[] Unindex<T>(T[] objects)
		{
			T[] objectsUnindexed = objects;
			if (objects != null &&
			    _includeInTransformIndex != null &&
			    objects.Length != _tupleLength)
			{
				objectsUnindexed = new T[_tupleLength];
				for (int i = 0; i < _tupleSubsetLength; i++)
				{
					objectsUnindexed[_includeInTransformIndex[i]] = objects[i];
				}
			}
			return objectsUnindexed;
		}

		public override bool Equals(object o)
		{
			if (this == o)
				return true;

			if (o == null || GetType() != o.GetType())
				return false;

			var that = (CacheableResultTransformer) o;

			// Auto-discovery does not allow distinguishing by transformer: just compare their queries
			if (AutoDiscoverTypes && that.AutoDiscoverTypes)
				return _autoDiscoveredQuery == that._autoDiscoveredQuery;

			return HasSameParameters(that);
		}

		private bool HasSameParameters(CacheableResultTransformer other)
		{
			return _tupleLength == other._tupleLength
				&& _tupleSubsetLength == other._tupleSubsetLength
				&& ArrayHelper.ArrayEquals(_includeInTuple, other._includeInTuple)
				&& ArrayHelper.ArrayEquals(_includeInTransformIndex, other._includeInTransformIndex);
		}

		public override int GetHashCode()
		{
			if (AutoDiscoverTypes)
				return _autoDiscoveredQuery.GetHashCode();

			var result = _tupleLength;
			result = 31*result + _tupleSubsetLength;
			result = 31*result + (_includeInTuple != null ? ArrayHelper.ArrayGetHashCode(_includeInTuple) : 0);
			result = 31*result + (_includeInTransformIndex != null ? ArrayHelper.ArrayGetHashCode(_includeInTransformIndex) : 0);
			return result;
		}
	}
}
