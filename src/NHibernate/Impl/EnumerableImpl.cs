using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl 
{
	/// <summary>
	/// Provides an <see cref="IEnumerable"/> wrapper over the results of an <see cref="IQuery"/>.
	/// </summary>
	/// <remarks>
	/// This is the IteratorImpl in H2.0.3
	/// </remarks>
	internal class EnumerableImpl : IEnumerable, IEnumerator
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EnumerableImpl));
		
		private IDataReader _reader;
		private ISessionImplementor _sess;
		private IType[] _types;
		private bool _single;
		private object[] _currentResults;
		private bool _hasNext;
		private string[][] _names;
		private IDbCommand _cmd;

		// when we start enumerating through the DataReader we are positioned
		// before the first record we need
		private int _currentRow = -1;

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
		/// <remarks>
		/// The <see cref="IDataReader"/> should already be positioned on the first record in <see cref="RowSelection"/>.
		/// </remarks>
		public EnumerableImpl(IDataReader reader, IDbCommand cmd, ISessionImplementor sess, IType[] types, string[][] columnNames, RowSelection selection) 
		{
			_reader = reader;
			_cmd = cmd;
			_sess = sess;
			_types = types;
			_names = columnNames;
			_selection = selection;

			_single = _types.Length==1;
		}

		private void PostMoveNext(bool hasNext) 
		{
			_hasNext = hasNext;
			_currentRow++;
			if( _selection!=null && _selection.MaxRows!=RowSelection.NoValue ) 
			{
				_hasNext = _hasNext && ( _currentRow < _selection.MaxRows );
			}
			// there are no more records in the DataReader so clean up
			if( !_hasNext ) 
			{
				log.Debug("exhausted results");
				_currentResults = null;
				_sess.Batcher.CloseQueryCommand( _cmd, _reader );
			} 
			else 
			{
				log.Debug("retreiving next results");
				_currentResults = new object[_types.Length];
				
				// move through each of the ITypes contained in the IDataReader and convert them
				// to their objects.  
				for (int i=0; i<_types.Length; i++) 
				{
					// The IType knows how to extract its value out of the IDataReader.  If the IType
					// is a value type then the value will simply be pulled out of the IDataReader.  If
					// the IType is an Entity type then the IType will extract the id from the IDataReader
					// and use the ISession to load an instance of the object.
					_currentResults[i] = _types[i].NullSafeGet(_reader, _names[i], _sess, null);
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
			get 
			{
				if( _single ) 
				{
					return _currentResults[0];
				} 
				else 
				{
					return _currentResults;
				}
			}
		}

		/// <summary>
		/// Advances the enumerator to the next element of the query results.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the enumerator was successfully advanced to the next query results
		/// ; <c>false</c> if the enumerator has passed the end of the query results.
		///</returns>
		public bool MoveNext() 
		{
			PostMoveNext( _reader.Read() );
			
			return _hasNext;
		}

		public void Reset() 
		{
			//can't reset the reader...we are SOL
		}


	}
}
