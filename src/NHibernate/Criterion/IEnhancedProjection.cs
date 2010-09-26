using System;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An "enhanced" Projection for a <see cref="ICriteria" /> query.
	/// </summary>
	public interface IEnhancedProjection : IProjection 
	{
		/// <summary>
		/// Get the SQL column aliases used by this projection for the columns it writes for inclusion into the
		/// <code>SELECT</code> clause <see cref="IProjection.ToSqlString" />.  NHibernate always uses column aliases 
		/// to extract data from the <see cref="System.Data.IDataReader" />, so it is important that these be implemented 
		/// correctly in order for NHibernate to be able to extract these values correctly.
		/// </summary>
		/// <param name="position">Just as in <see cref="IProjection.ToSqlString" />, represents the number of columns rendered prior to this projection.</param>
		/// <param name="criteria">The local criteria to which this project is attached (for resolution).</param>
		/// <param name="criteriaQuery">The overall criteria query instance.</param>
		/// <returns>The columns aliases.</returns>
		string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery);
		
		/// <summary>
		/// Get the SQL column aliases used by this projection for the columns it writes for inclusion into the
	 	/// <code>SELECT</code> clause (<see cref="IProjection.ToSqlString" />) for a particular criteria-level alias.
		/// </summary>
		/// <param name="alias">The criteria-level alias.</param>
		/// <param name="position">Just as in <see cref="IProjection.ToSqlString" />, represents the number of columns rendered prior to this projection.</param>
		/// <param name="criteria">The local criteria to which this project is attached (for resolution).</param>
		/// <param name="criteriaQuery">The overall criteria query instance.</param>
		/// <returns>The columns aliases.</returns>
		string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery);
	}
}