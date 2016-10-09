using System.Collections;

namespace NHibernate.Transform
{
	/// <summary>
	/// Implementers define a strategy for transforming criteria query
	/// results into the actual application-visible query result list.
	/// </summary>
	/// <seealso cref="NHibernate.ICriteria.SetResultTransformer(IResultTransformer)" />
	public interface IResultTransformer
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tuple"></param>
		/// <param name="aliases"></param>
		/// <returns></returns>
		object TransformTuple(object[] tuple, string[] aliases);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		IList TransformList(IList collection);
	}

    /// <summary>
    /// Implementers define a strategy for transforming a <typeparamref name="T"/>
    /// specific <see cref="Result"/> for transforming criteria query results into
    /// the application specific query result list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResultTransformer<out T> : IResultTransformer
        where T : new()
    {
        T Result { get; }
    }
}