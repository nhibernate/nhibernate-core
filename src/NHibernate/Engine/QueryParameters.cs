using System;
using System.Collections;
using System.Text;

using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	/// <summary>
	/// Container for data that is used during the NHibernate query/load process. 
	/// </summary>
	public class QueryParameters
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof(QueryParameters) );

		private IType[] _positionalParameterTypes;
		private object[] _positionalParameterValues;
		private RowSelection _rowSelection;
		private IDictionary _lockModes;
		private IDictionary _namedParameters;

		/// <summary>
		/// Initializes an instance of the <see cref="QueryParameters"/> class.
		/// </summary>
		/// <param name="positionalParameterTypes">An array of <see cref="IType"/> objects for the parameters.</param>
		/// <param name="positionalParameterValues">An array of <see cref="object"/> objects for the parameters.</param>
		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues)
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
		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues, IDictionary lockModes, RowSelection rowSelection)
			: this( positionalParameterTypes, positionalParameterValues, null, lockModes, rowSelection )
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
		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues, IDictionary namedParameters, IDictionary lockModes, RowSelection rowSelection) 
		{
			_positionalParameterTypes = positionalParameterTypes;
			_positionalParameterValues = positionalParameterValues;
			_rowSelection = rowSelection;
			_lockModes = lockModes;
			_namedParameters = namedParameters;
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
		public IType[] PositionalParameterTypes
		{
			get { return _positionalParameterTypes; }
			set { _positionalParameterTypes = value; }
		}

		/// <summary>
		/// Gets or sets an array of <see cref="object"/> objects that is stored at the index 
		/// of the Parameter.
		/// </summary>
		public object[] PositionalParameterValues
		{
			get { return _positionalParameterValues; }
			set { _positionalParameterValues = value; }
		}

		public bool HasRowSelection 
		{
			get { return _rowSelection!=null; }
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
		/// Ensure the Types and Values are the same length.
		/// </summary>
		/// <exception cref="QueryException">
		/// If the Lengths of <see cref="PositionalParameterTypes"/> and 
		/// <see cref="PositionalParameterValues"/> are not equal.
		/// </exception>
		public void ValidateParameters() 
		{
			int typesLength = PositionalParameterTypes!=null ? 0 : PositionalParameterTypes.Length;
			int valuesLength = PositionalParameterValues!=null ? 0 : PositionalParameterValues.Length;

			if( typesLength!=valuesLength ) 
			{
				throw new QueryException( "Number of positional parameter types (" + typesLength + ") does not match number of positional parameter values (" + valuesLength + ")" );
			}
		}

		internal void LogParameters() 
		{
			StringBuilder builder = new StringBuilder();
			
			if( PositionalParameterTypes!=null && PositionalParameterTypes.Length>0 ) 
			{
				for( int i=0; i<PositionalParameterTypes.Length; i++ ) 
				{
					if( PositionalParameterTypes[i]!=null ) 
					{
						builder.Append( PositionalParameterTypes[i].Name );
					}
					else 
					{
						builder.Append( "null type" );
					}

					builder.Append( " = " );

					if( PositionalParameterValues[i]!=null ) 
					{
						builder.Append( PositionalParameterValues[i].ToString() );
					}
					else 
					{
						builder.Append( "null value" );
					}

					builder.Append( ", " );
				}
			}
			else 
			{
				builder.Append( "No Types and Values" );
			}

			//TODO: add logging for named parameters
			log.Debug( builder.ToString() );
		}
	}
}
