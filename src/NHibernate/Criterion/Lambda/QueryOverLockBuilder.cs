
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverLockBuilder<TRoot,TSubType> : QueryOverLockBuilderBase<QueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public QueryOverLockBuilder(QueryOver<TRoot,TSubType> root, Expression<Func<object>> alias)
			: base(root, alias) { }

	}

	public class IQueryOverLockBuilder<TRoot,TSubType> : QueryOverLockBuilderBase<IQueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public IQueryOverLockBuilder(IQueryOver<TRoot,TSubType> root, Expression<Func<object>> alias)
			: base(root, alias) { }

	}

	public class QueryOverLockBuilderBase<TReturn, TRoot, TSubType> where TReturn : IQueryOver<TRoot,TSubType>
	{

		protected TReturn root;
		protected string alias;

		protected QueryOverLockBuilderBase(TReturn root, Expression<Func<object>> alias)
		{
			this.root = root;

			if (alias != null)
				this.alias = ExpressionProcessor.FindMemberExpression(alias.Body);
		}

		private void SetLockMode(LockMode lockMode)
		{
			if (alias != null)
				root.UnderlyingCriteria.SetLockMode(alias, lockMode);
			else
				root.UnderlyingCriteria.SetLockMode(lockMode);
		}

		public TReturn Force
		{
			get
			{
				SetLockMode(LockMode.Force);
				return this.root;
			}
		}

		public TReturn None
		{
			get
			{
				SetLockMode(LockMode.None);
				return this.root;
			}
		}

		public TReturn Read
		{
			get
			{
				SetLockMode(LockMode.Read);
				return this.root;
			}
		}

		public TReturn Upgrade
		{
			get
			{
				SetLockMode(LockMode.Upgrade);
				return this.root;
			}
		}

		public TReturn UpgradeNoWait
		{
			get
			{
				SetLockMode(LockMode.UpgradeNoWait);
				return this.root;
			}
		}

		public TReturn Write
		{
			get
			{
				SetLockMode(LockMode.Write);
				return this.root;
			}
		}

	}

}
