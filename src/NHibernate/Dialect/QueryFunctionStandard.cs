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

		/// <summary></summary>
		public QueryFunctionStandard()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="returnType"></param>
		public QueryFunctionStandard( IType returnType )
		{
			this.returnType = returnType;
		}

		#region IQueryFunctionInfo Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnType"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public IType QueryFunctionType( IType columnType, IMapping mapping )
		{
			if( returnType == null )
			{
				return columnType;
			}

			return returnType;
		}

		/// <summary></summary>
		public bool IsFunctionArgs
		{
			get { return true; }
		}

		/// <summary></summary>
		public bool IsFunctionNoArgsUseParanthesis
		{
			get { return true; }
		}

		#endregion
	}
}