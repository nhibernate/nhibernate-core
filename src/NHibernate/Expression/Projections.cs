using System;

using NHibernate.Type;


namespace NHibernate.Expression
{

	/// <summary>
	/// The <tt>criterion</tt> package may be used by applications as a framework for building
	/// new kinds of <tt>Projection</tt>. However, it is intended that most applications will
	/// simply use the built-in projection types via the static factory methods of this class.<br/>
	/// <br/>
	/// The factory methods that take an alias allow the projected value to be referred to by 
	/// criterion and order instances.
	/// </summary>
	public class Projections
	{
		Projections() 
		{
			// Private Constructor, never called.
		}

		/// <summary>
		/// Create a distinct projection from a projection
		/// </summary>
		/// <param name="proj"></param>
		/// <returns></returns>
		public static IProjection Distinct(IProjection proj)
		{
			return new Distinct(proj);
		}

		/// <summary>
		/// Create a new projection list
		/// </summary>
		/// <returns></returns>
		public static ProjectionList ProjectionList()
		{
			return new ProjectionList();
		}

		/// <summary>
		/// The query row count, ie. <tt>count(*)</tt>
		/// </summary>
		/// <returns></returns>
		public static IProjection RowCount()
		{
			return new RowCountProjection();
		}

		/// <summary>
		/// A property value count
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static CountProjection count(string propertyName)
		{
			return new CountProjection(propertyName);
		}

		/// <summary>
		/// A distinct property value count
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static CountProjection CountDistinct(string propertyName)
		{
			return new CountProjection(propertyName).SetDistinct();
		}

		/// <summary>
		/// A property maximum value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AggregateProjection Max(String propertyName)
		{
			return new AggregateProjection("max", propertyName);
		}

		/// <summary>
		/// A property minimum value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AggregateProjection Min(String propertyName)
		{
			return new AggregateProjection("min", propertyName);
		}

		/// <summary>
		/// A property average value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AggregateProjection Avg(string propertyName)
		{
			return new AvgProjection(propertyName);
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AggregateProjection Sum(String propertyName)
		{
			return new AggregateProjection("sum", propertyName);
		}

		/// <summary>
		/// A SQL projection, a typed select clause fragment
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="columnAliases"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static IProjection SqlProjection(string sql, string[] columnAliases, IType[] types)
		{
			return new SQLProjection(sql, columnAliases, types);
		}

		/// <summary>
		/// A grouping SQL projection, specifying both select clause and group by clause fragments
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="groupBy"></param>
		/// <param name="columnAliases"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static IProjection SqlGroupProjection(string sql, string groupBy, string[] columnAliases, IType[] types)
		{
			return new SQLProjection(sql, groupBy, columnAliases, types);
		}

		/// <summary>
		/// A grouping property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static PropertyProjection GroupProperty(string propertyName)
		{
			return new PropertyProjection(propertyName, true);
		}

		/// <summary>
		/// A projected property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static PropertyProjection Property(string propertyName)
		{
			return new PropertyProjection(propertyName);
		}

		/// <summary>
		/// A projected identifier value
		/// </summary>
		/// <returns></returns>
		public static IdentifierProjection Id()
		{
			return new IdentifierProjection();
		}

		/// <summary>
		/// Assign an alias to a projection, by wrapping it
		/// </summary>
		/// <param name="projection"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static IProjection Alias(IProjection projection, string alias)
		{
			return new AliasedProjection(projection, alias);
		}

	}
}