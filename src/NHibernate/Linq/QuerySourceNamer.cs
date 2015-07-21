using System;
using System.Collections.Generic;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq
{
	/// <summary>
	/// Associate unique names to query sources. The HQL AST parser will rename them anyway, but we need to
	/// ensure uniqueness that is not provided by IQuerySource.ItemName.
	/// </summary>
	public class QuerySourceNamer
	{
		private readonly IDictionary<IQuerySource, string> _map = new Dictionary<IQuerySource, string>();
		private readonly IList<string> _names = new List<string>();
		private int _differentiator = 1;

		public void Add(IQuerySource querySource)
		{
			if (_map.ContainsKey(querySource))
				return;

			_map.Add(querySource, CreateUniqueName(querySource.ItemName));
		}

		public string GetName(IQuerySource querySource)
		{
			string result;
			if (!_map.TryGetValue(querySource, out result))
			{
				throw new HibernateException(
					String.Format("Query Source could not be identified: ItemName = {0}, ItemType = {1}, Expression = {2}",
								  querySource.ItemName,
								  querySource.ItemType,
								  querySource));
			}

			return result;
		}

		private string CreateUniqueName(string proposedName)
		{
			string uniqueName = proposedName;

			if (_names.Contains(proposedName))
			{
				// make the name unique
				uniqueName = string.Format("{0}{1:000}", proposedName, _differentiator);
				_differentiator++;
			}

			_names.Add(uniqueName);

			return uniqueName;
		}
	}
}