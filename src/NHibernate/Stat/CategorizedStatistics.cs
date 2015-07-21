using System;

namespace NHibernate.Stat
{
	/// <summary> 
	/// Statistics for a particular "category" (a named entity,
	/// collection role, second level cache region or query). 
	/// </summary>
	[Serializable]
	public class CategorizedStatistics
	{
		private readonly string categoryName;

		internal CategorizedStatistics(string categoryName)
		{
			this.categoryName = categoryName;
		}

		public string CategoryName
		{
			get { return categoryName; }
		}
	}
}
