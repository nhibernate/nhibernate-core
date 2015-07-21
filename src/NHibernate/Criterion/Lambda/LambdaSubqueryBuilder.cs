
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class LambdaSubqueryBuilder
	{
		private string propertyName;
		private object value;

		/// <summary>
		/// Constructed with property name
		/// </summary>
		public LambdaSubqueryBuilder(string propertyName, object value)
		{
			this.propertyName = propertyName;
			this.value = value;
		}

		private AbstractCriterion CreatePropertyCriterion<U>(	Func<string, DetachedCriteria, AbstractCriterion>   propertyFactoryMethod,
																Func<object, DetachedCriteria, AbstractCriterion>	valueFactoryMethod,
																QueryOver<U>										detachedCriteria)
		{
			if (propertyName != null)
			{
				return propertyFactoryMethod(propertyName, detachedCriteria.DetachedCriteria);
			}
			else
			{
				return valueFactoryMethod(value, detachedCriteria.DetachedCriteria);
			}
		}

		/// <summary>
		/// Add a property equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion Eq<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyEq, Subqueries.Eq, detachedCriteria);
		}

		/// <summary>
		/// Add a property equal all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion EqAll<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyEqAll, Subqueries.EqAll, detachedCriteria);
		}

		/// <summary>
		/// Create a property greater than or equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion Ge<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyGe, Subqueries.Ge, detachedCriteria);
		}

		/// <summary>
		/// Create a property greater than or equal all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion GeAll<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyGeAll, Subqueries.GeAll, detachedCriteria);
		}

		/// <summary>
		/// Create a property greater than or equal some subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion GeSome<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyGeSome, Subqueries.GeSome, detachedCriteria);
		}

		/// <summary>
		/// Create a property greater than subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion Gt<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyGt, Subqueries.Gt, detachedCriteria);
		}

		/// <summary>
		/// Create a property greater than all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion GtAll<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyGtAll, Subqueries.GtAll, detachedCriteria);
		}

		/// <summary>
		/// Create a property greater than some subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion GtSome<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyGtSome, Subqueries.GtSome, detachedCriteria);
		}

		/// <summary>
		/// Create a property in subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion In<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyIn, Subqueries.In, detachedCriteria);
		}

		/// <summary>
		/// Create a property less than or equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion Le<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyLe, Subqueries.Le, detachedCriteria);
		}

		/// <summary>
		/// Create a property less than or equal all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion LeAll<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyLeAll, Subqueries.LeAll, detachedCriteria);
		}

		/// <summary>
		/// Create a property less than or equal some subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion LeSome<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyLeSome, Subqueries.LeSome, detachedCriteria);
		}

		/// <summary>
		/// Create a property less than subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion Lt<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyLt, Subqueries.Lt, detachedCriteria);
		}

		/// <summary>
		/// Create a property less than all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion LtAll<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyLtAll, Subqueries.LtAll, detachedCriteria);
		}

		/// <summary>
		/// Create a property less than some subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion LtSome<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyLtSome, Subqueries.LtSome, detachedCriteria);
		}

		/// <summary>
		/// Create a property not equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion Ne<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyNe, Subqueries.Ne, detachedCriteria);
		}

		/// <summary>
		/// Create a property not in subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public AbstractCriterion NotIn<U>(QueryOver<U> detachedCriteria)
		{
			return CreatePropertyCriterion(Subqueries.PropertyNotIn, Subqueries.NotIn, detachedCriteria);
		}

	}

}
