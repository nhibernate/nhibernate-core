using System;
using System.Collections;
using System.Data.Common;

using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Provides an <see cref="IEnumerable"/> wrapper over the results of an <see cref="IQuery"/>.
	/// </summary>
	/// <remarks>
	/// <para>This is the IteratorImpl in H2.0.3</para>
	/// <para>This thing is scary. It is an <see cref="IEnumerable" /> which returns itself as a <see cref="IEnumerator" />
	/// when <c>GetEnumerator</c> is called, and <c>EnumerableImpl</c> is disposable. Iterating over it with a <c>foreach</c>
	/// will cause it to be disposed, probably unexpectedly for the developer. (https://stackoverflow.com/a/11179175/1178314)
	/// "Fortunately", it does not currently support multiple iterations anyway.</para>
	/// </remarks>
	public class EnumerableImpl : IEnumerable, IEnumerator, IDisposable
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(EnumerableImpl));

		private DbDataReader _reader;
		private IEventSource _session;
		private IType[] _types;
		private bool _single;
		private object _currentResult;
		private bool _hasNext;
		private bool _startedReading; // True if at least one MoveNext call was made.
		private string[][] _names;
		private DbCommand _cmd;
		private bool _readOnly;

		// when we start enumerating through the DataReader we are positioned
		// before the first record we need
		private int _currentRow = -1;
		private IResultTransformer _resultTransformer;
		private string[] _returnAliases;
		private RowSelection _selection;

		/// <summary>
		/// Create an <see cref="IEnumerable"/> wrapper over an <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="reader">The <see cref="DbDataReader"/> to enumerate over.</param>
		/// <param name="cmd">The <see cref="DbCommand"/> used to create the <see cref="DbDataReader"/>.</param>
		/// <param name="session">The <see cref="ISession"/> to use to load objects.</param>
		/// <param name="readOnly"></param>
		/// <param name="types">The <see cref="IType"/>s contained in the <see cref="DbDataReader"/>.</param>
		/// <param name="columnNames">The names of the columns in the <see cref="DbDataReader"/>.</param>
		/// <param name="selection">The <see cref="RowSelection"/> that should be applied to the <see cref="DbDataReader"/>.</param>
		/// <param name="holderInstantiator">Instantiator of the result holder (used for "select new SomeClass(...)" queries).</param>
		/// <remarks>
		/// The <see cref="DbDataReader"/> should already be positioned on the first record in <see cref="RowSelection"/>.
		/// </remarks>
		//Since v5.2
		[Obsolete("Please use the constructor accepting resultTransformer and queryReturnAliases")]
		public EnumerableImpl(DbDataReader reader,
							  DbCommand cmd,
							  IEventSource session,
							  bool readOnly,
							  IType[] types,
							  string[][] columnNames,
							  RowSelection selection,
							  HolderInstantiator holderInstantiator)
			: this(reader, cmd, session, readOnly, types, columnNames, selection, holderInstantiator.ResultTransformer, holderInstantiator.QueryReturnAliases)
		{
		}

		/// <summary>
		/// Create an <see cref="IEnumerable"/> wrapper over an <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="reader">The <see cref="DbDataReader"/> to enumerate over.</param>
		/// <param name="cmd">The <see cref="DbCommand"/> used to create the <see cref="DbDataReader"/>.</param>
		/// <param name="session">The <see cref="ISession"/> to use to load objects.</param>
		/// <param name="readOnly"></param>
		/// <param name="types">The <see cref="IType"/>s contained in the <see cref="DbDataReader"/>.</param>
		/// <param name="columnNames">The names of the columns in the <see cref="DbDataReader"/>.</param>
		/// <param name="selection">The <see cref="RowSelection"/> that should be applied to the <see cref="DbDataReader"/>.</param>
		/// <param name="resultTransformer">The <see cref="IResultTransformer"/> that should be applied to a result row or <c>null</c>.</param>
		/// <param name="returnAliases">The aliases that correspond to a result row.</param>
		/// <remarks>
		/// The <see cref="DbDataReader"/> should already be positioned on the first record in <see cref="RowSelection"/>.
		/// </remarks>
		public EnumerableImpl(
			DbDataReader reader,
			DbCommand cmd,
			IEventSource session,
			bool readOnly,
			IType[] types,
			string[][] columnNames,
			RowSelection selection,
			IResultTransformer resultTransformer,
			string[] returnAliases)
		{
			_reader = reader;
			_cmd = cmd;
			_session = session;
			_readOnly = readOnly;
			_types = types;
			_names = columnNames;
			_selection = selection;

			_single = _types.Length == 1;
			_resultTransformer = resultTransformer;
			_returnAliases = returnAliases;
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the query results.
		/// </summary>
		/// <returns>
		/// An <see cref="IEnumerator" /> that can be used to iterate through the query results.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			this.Reset();
			return this;
		}

		/// <summary>
		/// Gets the current element in the query results.
		/// </summary>
		/// <value>
		/// The current element in the query results which is either an object or 
		/// an object array.
		/// </value>
		/// <remarks>
		/// If the <see cref="IQuery"/> only returns one type of Entity then an object will
		/// be returned.  If this is a multi-column resultset then an object array will be
		/// returned.
		/// </remarks>
		public object Current
		{
			get { return _currentResult; }
		}

		/// <summary>
		/// Advances the enumerator to the next element of the query results.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if the enumerator was successfully advanced to the next query results
		/// ; <see langword="false" /> if the enumerator has passed the end of the query results.
		///</returns>
		public bool MoveNext()
		{
			bool readResult;
			try
			{
				readResult = _reader.Read();
			}
			catch (DbException e)
			{
				throw ADOExceptionHelper.Convert(_session.Factory.SQLExceptionConverter, e, "Error executing Enumerable() query",
												   new SqlString(_cmd.CommandText));
			}
			PostMoveNext(readResult);
			return _hasNext;
		}
		
		private void PostNext()
		{
			log.Debug("attempting to retrieve next results");
			bool readResult;
			try
			{
				readResult = _reader.Read();
				if (!readResult)
				{
					log.Debug("exhausted results");
					_currentResult = null;
					_session.Batcher.CloseCommand(_cmd, _reader);
				}
				else
					log.Debug("retrieved next results");
			}
			catch (DbException e)
			{
				throw ADOExceptionHelper.Convert(_session.Factory.SQLExceptionConverter, e, "Error executing Enumerable() query",
												 new SqlString(_cmd.CommandText));
			}
		}

		private void PostMoveNext(bool hasNext)
		{
			_startedReading = true;
			_hasNext = hasNext;
			_currentRow++;
			
			if (_selection != null && _selection.MaxRows != RowSelection.NoValue)
			{
				_hasNext = _hasNext && (_currentRow < _selection.MaxRows);
			}
			
			bool sessionDefaultReadOnlyOrig = _session.DefaultReadOnly;
			
			_session.DefaultReadOnly = _readOnly;

			try
			{
				if (!_hasNext)
				{
					// there are no more records in the DataReader so clean up
					log.Debug("exhausted results");
					_currentResult = null;
					_session.Batcher.CloseCommand(_cmd, _reader);
				}
				else
				{
					log.Debug("retrieving next results");

					if (_single && _resultTransformer == null)
					{
						_currentResult = _types[0].NullSafeGet(_reader, _names[0], _session, null);
					}
					else
					{
						object[] currentResults = new object[_types.Length];
	
						// move through each of the ITypes contained in the DbDataReader and convert them
						// to their objects.  
						for (int i = 0; i < _types.Length; i++)
						{
							// The IType knows how to extract its value out of the DbDataReader.  If the IType
							// is a value type then the value will simply be pulled out of the DbDataReader.  If
							// the IType is an Entity type then the IType will extract the id from the DbDataReader
							// and use the ISession to load an instance of the object.
							currentResults[i] = _types[i].NullSafeGet(_reader, _names[i], _session, null);
						}

						if (_resultTransformer != null)
						{
							_currentResult = _resultTransformer.TransformTuple(currentResults, _returnAliases);
						}
						else
						{
							_currentResult = currentResults;
						}
					}
				}
			}
			finally
			{
				_session.DefaultReadOnly = sessionDefaultReadOnlyOrig;
			}
		}		
		
		public void Reset()
		{
			//can't reset the reader...we are SOL
		}

		#region IDisposable Members

		/// <summary>
		/// A flag to indicate if <c>Dispose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~EnumerableImpl()
		{
			Dispose(false);
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		public void Dispose()
		{
			log.Debug("running EnumerableImpl.Dispose()");
			Dispose(true);
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this EnumerableImpl is being Disposed of or Finalized.</param>
		/// <remarks>
		/// The command is closed and the reader is disposed.  This allows other ADO.NET
		/// related actions to occur without needing to move all the way through the
		/// EnumerableImpl.
		/// </remarks>
		protected virtual void Dispose(bool isDisposing)
		{
			if (_isAlreadyDisposed)
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the EnumerableImpl if we
			// know this call came through Dispose()
			if (isDisposing)
			{
				// if there is still a possibility of moving next then we need to clean up
				// the resources - otherwise the cleanup has already been done by the 
				// PostMoveNext method.
				if (_hasNext || !_startedReading)
				{
					_currentResult = null;
					_session.Batcher.CloseCommand(_cmd, _reader);
				}
				// nothing for Finalizer to do - so tell the GC to ignore it
				GC.SuppressFinalize(this);
			}

			// free unmanaged resources here

			_isAlreadyDisposed = true;
		}

		#endregion
	}
}
