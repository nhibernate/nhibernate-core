using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using log4net;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Provides an <see cref="IEnumerable"/> wrapper over the results of an <see cref="IQuery"/>.
	/// </summary>
	/// <remarks>
	/// This is the IteratorImpl in H2.0.3
	/// </remarks>
	public class EnumerableImpl : IEnumerable, IEnumerator, IDisposable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(EnumerableImpl));

		private IDataReader _reader;
		private ISessionImplementor _sess;
		private IType[] _types;
		private bool _single;
		private object _currentResult;
		private bool _hasNext;
		private bool _startedReading; // True if at least one MoveNext call was made.
		private string[][] _names;
		private IDbCommand _cmd;

		// when we start enumerating through the DataReader we are positioned
		// before the first record we need
		private int _currentRow = -1;
		private HolderInstantiator _holderInstantiator;
		private RowSelection _selection;

		/// <summary>
		/// Create an <see cref="IEnumerable"/> wrapper over an <see cref="IDataReader"/>.
		/// </summary>
		/// <param name="reader">The <see cref="IDataReader"/> to enumerate over.</param>
		/// <param name="cmd">The <see cref="IDbCommand"/> used to create the <see cref="IDataReader"/>.</param>
		/// <param name="sess">The <see cref="ISession"/> to use to load objects.</param>
		/// <param name="types">The <see cref="IType"/>s contained in the <see cref="IDataReader"/>.</param>
		/// <param name="columnNames">The names of the columns in the <see cref="IDataReader"/>.</param>
		/// <param name="selection">The <see cref="RowSelection"/> that should be applied to the <see cref="IDataReader"/>.</param>
		/// <param name="holderInstantiator">Instantiator of the result holder (used for "select new SomeClass(...)" queries).</param>
		/// <remarks>
		/// The <see cref="IDataReader"/> should already be positioned on the first record in <see cref="RowSelection"/>.
		/// </remarks>
		public EnumerableImpl(IDataReader reader, IDbCommand cmd, ISessionImplementor sess, IType[] types,
		                      string[][] columnNames, RowSelection selection,
		                      HolderInstantiator holderInstantiator)
		{
			_reader = reader;
			_cmd = cmd;
			_sess = sess;
			_types = types;
			_names = columnNames;
			_selection = selection;
			_holderInstantiator = holderInstantiator;

			_single = _types.Length == 1;
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
			// there are no more records in the DataReader so clean up
			if (!_hasNext)
			{
				log.Debug("exhausted results");
				_currentResult = null;
				_sess.Batcher.CloseCommand(_cmd, _reader);
			}
			else
			{
				log.Debug("retrieving next results");
				bool isHolder = _holderInstantiator.IsRequired;

				if (_single && !isHolder)
				{
					_currentResult = _types[0].NullSafeGet(_reader, _names[0], _sess, null);
				}
				else
				{
					object[] currentResults = new object[_types.Length];

					// move through each of the ITypes contained in the IDataReader and convert them
					// to their objects.  
					for (int i = 0; i < _types.Length; i++)
					{
						// The IType knows how to extract its value out of the IDataReader.  If the IType
						// is a value type then the value will simply be pulled out of the IDataReader.  If
						// the IType is an Entity type then the IType will extract the id from the IDataReader
						// and use the ISession to load an instance of the object.
						currentResults[i] = _types[i].NullSafeGet(_reader, _names[i], _sess, null);
					}

					if (isHolder)
					{
						_currentResult = _holderInstantiator.Instantiate(currentResults);
					}
					else
					{
						_currentResult = currentResults;
					}
				}
			}
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
				throw ADOExceptionHelper.Convert(_sess.Factory.SQLExceptionConverter, e, "Error executing Enumerable() query",
				                                 new SqlString(_cmd.CommandText));
			}
			PostMoveNext(readResult);
			return _hasNext;
		}

		/// <summary></summary>
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
					_sess.Batcher.CloseCommand(_cmd, _reader);
				}
			}

			// free unmanaged resources here

			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
