using System;

using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Provides a standard implementation that supports the majority of the HQL
	/// functions that are translated to SQL.
	/// </summary>
	/// <remarks>
	/// The Dialect and its sub-classes use this class to provide details required
	/// for processing of the associated function.
	/// </remarks>
	public class QueryFunctionStandard : IQueryFunctionInfo
	{
		private IType returnType = null;

		public QueryFunctionStandard()
		{
		}

		public QueryFunctionStandard(IType returnType) 
		{
			this.returnType = returnType;
		}

		#region IQueryFunctionInfo Members

		public IType QueryFunctionType(IType columnType, IMapping mapping)
		{
			if(returnType==null) return columnType;

			return returnType;
		}

		public bool IsFunctionArgs
		{
			get{ return true;}
		}

		public bool IsFunctionNoArgsUseParanthesis
		{
			get	{ return true;}
		}

		#endregion
	}
}
