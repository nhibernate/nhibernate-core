using System.Collections;
using log4net;
using NHibernate.Type;
using NHibernate.Impl;

namespace NHibernate.Engine
{
	/// <summary>
	/// Container for data that is used during the NHibernate query/load process. 
	/// </summary>
	public sealed class QueryParameters
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( QueryParameters ) );

		private IType[ ] _positionalParameterTypes;
		private object[ ] _positionalParameterValues;
		private IDictionary _namedParameters;
		private IDictionary _lockModes;
		private RowSelection _rowSelection;
		private bool _cacheable;
		private string _cacheRegion;
		private bool _forceCacheRefresh;
		// not implemented: private ScrollMode _scrollMode;

		/// <summary>
		/// Initializes an instance of the <see cref="QueryParameters"/> class.
		/// </summary>
		/// <param name="positionalParameterTypes">An array of <see cref="IType"/> objects for the parameters.</param>
		/// <param name="positionalParameterValues">An array of <see cref="object"/> objects for the parameters.</param>
		public QueryParameters( IType[ ] positionalParameterTypes, object[ ] positionalParameterValues )
			: this( positionalParameterTypes, positionalParameterValues, null, null )
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="QueryParameters"/> class.
		/// </summary>
		/// <param name="positionalParameterTypes">An array of <see cref="IType"/> objects for the parameters.</param>
		/// <param name="positionalParameterValues">An array of <see cref="object"/> objects for the parameters.</param>
		/// <param name="lockModes">An <see cref="IDictionary"/> that is hql alias keyed to a LockMode value.</param>
		/// <param name="rowSelection"></param>
		public QueryParameters( IType[ ] positionalParameterTypes, object[ ] positionalParameterValues, IDictionary lockModes, RowSelection rowSelection )
			: this( positionalParameterTypes, positionalParameterValues, null, lockModes, rowSelection, false, null, false )
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="QueryParameters"/> class.
		/// </summary>
		/// <param name="positionalParameterTypes">An array of <see cref="IType"/> objects for the parameters.</param>
		/// <param name="positionalParameterValues">An array of <see cref="object"/> objects for the parameters.</param>
		/// <param name="namedParameters">An <see cref="IDictionary"/> that is <c>parameter name</c> keyed to a <see cref="TypedValue"/> value.</param>
		/// <param name="lockModes">An <see cref="IDictionary"/> that is <c>hql alias</c> keyed to a LockMode value.</param>
		/// <param name="rowSelection"></param>
		public QueryParameters(
			IType[ ] positionalParameterTypes,
			object[ ] positionalParameterValues,
			IDictionary namedParameters,
			IDictionary lockModes,
			RowSelection rowSelection,
			bool cacheable,
			string cacheRegion,
			bool forceCacheRefresh )
		{
			_positionalParameterTypes = positionalParameterTypes;
			_positionalParameterValues = positionalParameterValues;
			_namedParameters = namedParameters;
			_lockModes = lockModes;
			_rowSelection = rowSelection;
			_cacheable = cacheable;
			_cacheRegion = cacheRegion;
			_forceCacheRefresh = forceCacheRefresh;
		}

		/// <summary></summary>
		public bool HasRowSelection
		{
			get { return _rowSelection != null; }
		}

		/// <summary>
		/// Gets or sets an <see cref="IDictionary"/> that contains the named 
		/// parameter as the key and the <see cref="TypedValue"/> as the value.
		/// </summary>
		/// <value>An <see cref="IDictionary"/> of named parameters.</value>
		public IDictionary NamedParameters
		{
			get { return _namedParameters; }
			set { _namedParameters = value; }
		}

		/// <summary>
		/// Gets or sets an array of <see cref="IType"/> objects that is stored at the index 
		/// of the Parameter.
		/// </summary>
		public IType[ ] PositionalParameterTypes
		{
			get { return _positionalParameterTypes; }
			set { _positionalParameterTypes = value; }
		}

		/// <summary>
		/// Gets or sets an array of <see cref="object"/> objects that is stored at the index 
		/// of the Parameter.
		/// </summary>
		public object[ ] PositionalParameterValues
		{
			get { return _positionalParameterValues; }
			set { _positionalParameterValues = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="RowSelection"/> for the Query.
		/// </summary>
		public RowSelection RowSelection
		{
			get { return _rowSelection; }
			set { _rowSelection = value; }
		}

		/// <summary>
		/// Gets or sets an <see cref="IDictionary"/> that contains the alias name of the
		/// object from hql as the key and the <see cref="LockMode"/> as the value.
		/// </summary>
		/// <value>An <see cref="IDictionary"/> of lock modes.</value>
		public IDictionary LockModes
		{
			get { return _lockModes; }
			set { _lockModes = value; }
		}

        private int SafeLength( System.Array array )
        {
            if( array == null ) return 0;
            return array.Length;
        }            

		/// <summary></summary>
		public void LogParameters( ISessionFactoryImplementor factory )
		{
			Printer print = new Printer( factory );
			if( _positionalParameterValues.Length != 0 )
			{
				log.Debug( "parameters: "
					+ print.ToString( _positionalParameterTypes, _positionalParameterValues ) );
			}

			if( _namedParameters != null )
			{
				log.Debug( "named parameters: "
					+ print.ToString( _namedParameters ) );
			}
		}

		public bool Cacheable
		{
			get { return _cacheable; }
			set { _cacheable = value; }
		}

		public string CacheRegion
		{
			get { return _cacheRegion; }
			set { _cacheRegion = value; }
		}

		/// <summary>
		/// Ensure the Types and Values are the same length.
		/// </summary>
		/// <exception cref="QueryException">
		/// If the Lengths of <see cref="PositionalParameterTypes"/> and 
		/// <see cref="PositionalParameterValues"/> are not equal.
		/// </exception>
		public void ValidateParameters()
		{
			int typesLength = SafeLength( PositionalParameterTypes );
			int valuesLength = SafeLength( PositionalParameterValues );

			if( typesLength != valuesLength )
			{
				throw new QueryException( "Number of positional parameter types (" + typesLength + ") does not match number of positional parameter values (" + valuesLength + ")" );
			}
		}

		public bool ForceCacheRefresh
		{
			get { return _forceCacheRefresh; }
			set { _forceCacheRefresh = value; }
		}
	}

}