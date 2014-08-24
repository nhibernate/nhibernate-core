using System;
using System.Linq.Expressions;

using NHibernate.Impl;

namespace NHibernate.Criterion
{
	public static class QueryOverBuilderExtensions
	{
		// Fetch builder
		public static QueryOver<TRoot, TSubType> Default<TRoot, TSubType>(this Lambda.QueryOverFetchBuilder<TRoot, TSubType> builder)
		{
			return builder.Default;
		}

		public static QueryOver<TRoot, TSubType> Eager<TRoot, TSubType>(this Lambda.QueryOverFetchBuilder<TRoot, TSubType> builder)
		{
			return builder.Eager;
		}

		public static QueryOver<TRoot, TSubType> Lazy<TRoot, TSubType>(this Lambda.QueryOverFetchBuilder<TRoot, TSubType> builder)
		{
			return builder.Lazy;
		}

		public static IQueryOver<TRoot, TSubType> Default<TRoot, TSubType>(this Lambda.IQueryOverFetchBuilder<TRoot, TSubType> builder)
		{
			return builder.Default;
		}

		public static IQueryOver<TRoot, TSubType> Eager<TRoot, TSubType>(this Lambda.IQueryOverFetchBuilder<TRoot, TSubType> builder)
		{
			return builder.Eager;
		}

		public static IQueryOver<TRoot, TSubType> Lazy<TRoot, TSubType>(this Lambda.IQueryOverFetchBuilder<TRoot, TSubType> builder)
		{
			return builder.Lazy;
		}


		// Lock builder
		public static QueryOver<TRoot, TSubType> Force<TRoot, TSubType>(this Lambda.QueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.Force;
		}

		public static QueryOver<TRoot, TSubType> None<TRoot, TSubType>(this Lambda.QueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.None;
		}

		public static QueryOver<TRoot, TSubType> Read<TRoot, TSubType>(this Lambda.QueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.Read;
		}

		public static QueryOver<TRoot, TSubType> Upgrade<TRoot, TSubType>(this Lambda.QueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.Upgrade;
		}

		public static QueryOver<TRoot, TSubType> UpgradeNoWait<TRoot, TSubType>(this Lambda.QueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.UpgradeNoWait;
		}

		public static QueryOver<TRoot, TSubType> Write<TRoot, TSubType>(this Lambda.QueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.Write;
		}

		public static IQueryOver<TRoot, TSubType> Force<TRoot, TSubType>(this Lambda.IQueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.Force;
		}

		public static IQueryOver<TRoot, TSubType> None<TRoot, TSubType>(this Lambda.IQueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.None;
		}

		public static IQueryOver<TRoot, TSubType> Read<TRoot, TSubType>(this Lambda.IQueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.Read;
		}

		public static IQueryOver<TRoot, TSubType> Upgrade<TRoot, TSubType>(this Lambda.IQueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.Upgrade;
		}

		public static IQueryOver<TRoot, TSubType> UpgradeNoWait<TRoot, TSubType>(this Lambda.IQueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.UpgradeNoWait;
		}

		public static IQueryOver<TRoot, TSubType> Write<TRoot, TSubType>(this Lambda.IQueryOverLockBuilder<TRoot, TSubType> builder)
		{
			return builder.Write;
		}


		// Order builder
		public static QueryOver<TRoot, TSubType> Asc<TRoot, TSubType>(this Lambda.QueryOverOrderBuilder<TRoot, TSubType> builder)
		{
			return builder.Asc;
		}

		public static QueryOver<TRoot, TSubType> Desc<TRoot, TSubType>(this Lambda.QueryOverOrderBuilder<TRoot, TSubType> builder)
		{
			return builder.Desc;
		}

		public static IQueryOver<TRoot, TSubType> Asc<TRoot, TSubType>(this Lambda.IQueryOverOrderBuilder<TRoot, TSubType> builder)
		{
			return builder.Asc;
		}

		public static IQueryOver<TRoot, TSubType> Desc<TRoot, TSubType>(this Lambda.IQueryOverOrderBuilder<TRoot, TSubType> builder)
		{
			return builder.Desc;
		}


		// Restriction builder
		public static QueryOver<TRoot, TSubType> IsEmpty<TRoot, TSubType>(this Lambda.QueryOverRestrictionBuilder<TRoot, TSubType> builder)
		{
			return builder.IsEmpty;
		}

		public static QueryOver<TRoot, TSubType> IsNotEmpty<TRoot, TSubType>(this Lambda.QueryOverRestrictionBuilder<TRoot, TSubType> builder)
		{
			return builder.IsNotEmpty;
		}

		public static QueryOver<TRoot, TSubType> IsNotNull<TRoot, TSubType>(this Lambda.QueryOverRestrictionBuilder<TRoot, TSubType> builder)
		{
			return builder.IsNotNull;
		}

		public static QueryOver<TRoot, TSubType> IsNull<TRoot, TSubType>(this Lambda.QueryOverRestrictionBuilder<TRoot, TSubType> builder)
		{
			return builder.IsNull;
		}

		public static IQueryOver<TRoot, TSubType> IsEmpty<TRoot, TSubType>(this Lambda.IQueryOverRestrictionBuilder<TRoot, TSubType> builder)
		{
			return builder.IsEmpty;
		}

		public static IQueryOver<TRoot, TSubType> IsNotEmpty<TRoot, TSubType>(this Lambda.IQueryOverRestrictionBuilder<TRoot, TSubType> builder)
		{
			return builder.IsNotEmpty;
		}

		public static IQueryOver<TRoot, TSubType> IsNotNull<TRoot, TSubType>(this Lambda.IQueryOverRestrictionBuilder<TRoot, TSubType> builder)
		{
			return builder.IsNotNull;
		}

		public static IQueryOver<TRoot, TSubType> IsNull<TRoot, TSubType>(this Lambda.IQueryOverRestrictionBuilder<TRoot, TSubType> builder)
		{
			return builder.IsNull;
		}

	}
}
