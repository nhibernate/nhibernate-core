using System;

namespace NHibernate.Linq
{
	/// <summary>
	/// A Linq query transformer that is used to transform the result returned by <see cref="ResultTransformer.TransformList"/>.
	/// </summary>
	public class PostResultTransformer
	{
		private readonly Func<object, object[], object> _transformer;
		private readonly object[] _parameterValues;

		internal PostResultTransformer(Func<object, object[], object> transformer)
			: this(transformer, null)
		{
		}

		private PostResultTransformer(Func<object, object[], object> transformer, object[] parameterValues)
		{
			_transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
			_parameterValues = parameterValues;
		}

		internal PostResultTransformer WithParameterValues(object[] parameterValues)
		{
			return new PostResultTransformer(_transformer, parameterValues);
		}

		/// <summary>
		/// Transform the given query result.
		/// </summary>
		/// <param name="result">The query result to transform.</param>
		/// <returns>The transformed query result.</returns>
		public object Transform(object result)
		{
			return _transformer(result, _parameterValues);
		}

		// TODO 6.0: Remove
		internal Delegate GetDelegate()
		{
			return (Func<object, object>) Func;

			object Func(object l) => Transform(l);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_transformer.GetHashCode() * 397) ^ (_parameterValues?.GetHashCode() ?? 0);
			}
		}

		public bool Equals(PostResultTransformer other)
		{
			return _transformer == other._transformer &&
			       _parameterValues == other._parameterValues;
		}
	}
}
