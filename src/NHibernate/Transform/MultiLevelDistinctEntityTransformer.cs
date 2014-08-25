#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#endregion

namespace NHibernate.Transform
{
	/// <summary>
	/// Result transformer that allows to transform a result to
	/// distinct list of an Entity from the provided fetcheed collection at all levels.
	/// </summary>
	/// <example>
	/// <code>
	/// var fetchedCollectionProperties = new Dictionary<Type, List<String>>()
	///																{
	///																	{typeof(Employee),new List<String>
	///																		{ "Children","Educations" }
	///																	}
	///																}
	/// IList<Employee> resultWithDistinctMultiLevel = unitOfWork.Session.QueryOver<Employee>()
    /// 	.Fetch(x => x.Nationality).Eager
	///		.Fetch(x => x.Children).Eager
    /// 	.Fetch(x => x.Educations).Eager
    ///		.TransformUsing(new Transformers.MultiLevelDistinctRootEntity(fetchedCollectionProperties)).List();
	/// </code>
	/// </example>
    [Serializable]
    public class MultiLevelDistinctEntityTransformer : IResultTransformer
    {
        private readonly Dictionary<System.Type, List<String>> _fetchedCollectionProperties;

		internal sealed class Identity
		{
			internal readonly object Entity;

			internal Identity(object entity)
			{
				this.Entity = entity;
			}

			public override bool Equals(object other)
			{
				var that = (Identity) other;
				return ReferenceEquals(Entity, that.Entity);
			}

			public override int GetHashCode()
			{
				return RuntimeHelpers.GetHashCode(Entity);
			}
		}
		
        /// <summary>
        /// Create a new transformer.
        /// </summary>
        /// <param name="fetchedCollectionProperties">The fetched properties of each type.</param>
        public MultiLevelDistinctEntityTransformer(Dictionary<System.Type, List<String>> fetchedCollectionProperties)
        {
            _fetchedCollectionProperties = fetchedCollectionProperties;
        }

        #region Implementation of IResultTransformer

        /// <summary>
        /// Not used in this transformer.
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="aliases"></param>
        /// <returns></returns>
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple.Last();
        }

        /// <summary>
        /// Returns a distinct list from the provided collection at all levels.
        /// </summary>
        /// <param name="list">The collection to be transformed</param>
        /// <returns>The transformed list</returns>
        public IList TransformList(IList list)
        {
            if (list.Count == 0)
                return list;
            var result = (IList) Activator.CreateInstance(list.GetType());
            var distinct = new HashSet<Identity>();
            foreach (object item in list)
            {
                if (distinct.Add(new Identity(item)))
                {
                    result.Add(item);
                    HandleItemDetails(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Remove duplications from the item details.
        /// </summary>
        /// <param name="item">The item to remove details duplication from.</param>
        private void HandleItemDetails(object item)
        {
            IEnumerable<PropertyInfo> collectionProperties =
                item.GetType().GetProperties().Where(
                    prop =>
                        prop.PropertyType != typeof(string) &&
                        prop.PropertyType.GetInterfaces().Any(inter => inter.IsGenericType &&
                            inter.GetGenericTypeDefinition() == typeof(IEnumerable<>)) &&
                     _fetchedCollectionProperties.ContainsKey(item.GetType()) &&
                    _fetchedCollectionProperties[item.GetType()].Contains(prop.Name));
            foreach (PropertyInfo collectionProperty in collectionProperties)
            {
                var detailList =(IEnumerable) collectionProperty.GetValue(item, null);
                if (detailList != null)
                {
                    var uniqueValues =(IList)
                        Activator.CreateInstance(
                            typeof (List<>).MakeGenericType(collectionProperty.PropertyType.GetGenericArguments()[0]));
                    var distinct = new HashSet<Identity>();
                    foreach (var subItem in detailList)
                    {
                        if (distinct.Add(new Identity(subItem)))
                        {
                            uniqueValues.Add(subItem);
                            HandleItemDetails(subItem);
                        }
                    }
                    collectionProperty.SetValue(item, uniqueValues, null);
                }
            }
        }

        #endregion
    }
}