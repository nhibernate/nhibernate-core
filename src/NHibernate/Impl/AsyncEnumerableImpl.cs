using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Impl
{
	internal struct InitializeEnumerableResult
	{
		public InitializeEnumerableResult(DbCommand command, DbDataReader dataReader)
		{
			Command = command;
			DataReader = dataReader;
		}

		public DbCommand Command { get; }

		public DbDataReader DataReader { get; }
	}

	internal delegate Task<InitializeEnumerableResult> InitializeEnumerableAsync(CancellationToken cancellationToken);

	internal delegate InitializeEnumerableResult InitializeEnumerable();

	internal partial class AsyncEnumerableImpl<T> : IAsyncEnumerable<T>, IEnumerable<T>
	{
		private readonly InitializeEnumerableAsync _initializeAsync;
		private readonly InitializeEnumerable _initialize;
		private readonly bool _readOnly;
		private readonly IType[] _types;
		private readonly string[][] _columnNames;
		private readonly RowSelection _selection;
		private readonly IResultTransformer _resultTransformer;
		private readonly string[] _returnAliases;
		private readonly IEventSource _session;

		public AsyncEnumerableImpl(
			InitializeEnumerable initialize,
			InitializeEnumerableAsync initializeAsync,
			bool readOnly,
			IType[] types,
			string[][] columnNames,
			RowSelection selection,
			IResultTransformer resultTransformer,
			string[] returnAliases,
			IEventSource session)
		{
			_initialize = initialize;
			_initializeAsync = initializeAsync;
			_readOnly = readOnly;
			_types = types;
			_columnNames = columnNames;
			_selection = selection;
			_resultTransformer = resultTransformer;
			_returnAliases = returnAliases;
			_session = session;
		}

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new AsyncEnumeratorImpl(
				_initialize,
				_initializeAsync,
				_readOnly,
				_types,
				_columnNames,
				_selection,
				_resultTransformer,
				_returnAliases,
				_session,
				cancellationToken);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new AsyncEnumeratorImpl(
				_initialize,
				_initializeAsync,
				_readOnly,
				_types,
				_columnNames,
				_selection,
				_resultTransformer,
				_returnAliases,
				_session,
				default);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		internal sealed partial class AsyncEnumeratorImpl : IAsyncEnumerator<T>, IEnumerator<T>
		{
			private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(AsyncEnumeratorImpl));

			private readonly InitializeEnumerableAsync _initializeAsync;
			private readonly InitializeEnumerable _initialize;
			private readonly bool _readOnly;
			private readonly IType[] _types;
			private readonly string[][] _columnNames;
			private readonly RowSelection _selection;
			private readonly IResultTransformer _resultTransformer;
			private readonly string[] _returnAliases;
			private readonly IEventSource _session;
			private readonly CancellationToken _cancellationToken;
			private readonly bool _single;

			private DbDataReader _reader;
			private DbCommand _command;
			private bool _hasNext;
			private bool _startedReading; // True if at least one MoveNext call was made.
			// when we start enumerating through the DataReader we are positioned
			// before the first record we need
			private int _currentRow = -1;
			private bool _isAlreadyDisposed;

			public AsyncEnumeratorImpl(
				InitializeEnumerable initialize,
				InitializeEnumerableAsync initializeAsync,
				bool readOnly,
				IType[] types,
				string[][] columnNames,
				RowSelection selection,
				IResultTransformer resultTransformer,
				string[] returnAliases,
				IEventSource session,
				CancellationToken cancellationToken)
			{
				_initialize = initialize;
				_initializeAsync = initializeAsync;
				_readOnly = readOnly;
				_types = types;
				_single = types.Length == 1;
				_columnNames = columnNames;
				_selection = selection;
				_resultTransformer = resultTransformer;
				_returnAliases = returnAliases;
				_session = session;
				_cancellationToken = cancellationToken;
			}

			public T Current { get; private set; }

			object IEnumerator.Current => Current;

			public async ValueTask<bool> MoveNextAsync()
			{
				if (_reader == null)
				{
					await InitializeAsync().ConfigureAwait(false);
				}

				await ReadAsync(_cancellationToken).ConfigureAwait(false);

				return _hasNext;
			}

			public bool MoveNext()
			{
				if (_reader == null)
				{
					Initialize();
				}

				Read();

				return _hasNext;
			}

			private void Read()
			{
				try
				{
					_hasNext = _reader.Read();
					_startedReading = true;
					_currentRow++;
				}
				catch (DbException e)
				{
					throw ADOExceptionHelper.Convert(_session.Factory.SQLExceptionConverter, e, "Error executing Enumerable() query",
													   new SqlString(_command.CommandText));
				}

				PostRead();
			}

			private void PostRead()
			{
				if (_selection != null && _selection.MaxRows != RowSelection.NoValue)
				{
					_hasNext = _hasNext && (_currentRow < _selection.MaxRows);
				}

				bool sessionDefaultReadOnlyOrig = _session.DefaultReadOnly;
				_session.DefaultReadOnly = _readOnly;
				try
				{
					MaterializeAndSetCurrent();
				}
				finally
				{
					_session.DefaultReadOnly = sessionDefaultReadOnlyOrig;
				}
			}

			public void Reset()
			{
				Dispose();
				_isAlreadyDisposed = false;
			}

			/// <summary>
			/// Takes care of freeing the managed and unmanaged resources that this class is responsible for.
			/// </summary>
			/// <remarks>
			/// The command is closed and the reader is disposed. This allows other ADO.NET
			/// related actions to occur without needing to move all the way through the
			/// AsyncEnumeratorImpl.
			/// </remarks>
			public void Dispose()
			{
				Log.Debug($"running {nameof(AsyncEnumeratorImpl)}.{nameof(AsyncEnumeratorImpl.Dispose)}()");

				if (_isAlreadyDisposed)
				{
					// don't dispose of multiple times.
					return;
				}

				// if there is still a possibility of moving next then we need to clean up
				// the resources - otherwise the cleanup has already been done by the 
				// PostMoveNext method.
				if (_hasNext || !_startedReading)
				{
					Current = default;
					_session.Batcher.CloseCommand(_command, _reader);
				}

				// Reset all fields
				_reader = null;
				_command = null;
				_hasNext = false;
				_startedReading = false;
				_currentRow = -1;
				_isAlreadyDisposed = true;
			}

			public ValueTask DisposeAsync()
			{
				Dispose();
				return default;
			}

			private void MaterializeAndSetCurrent()
			{
				if (!_hasNext)
				{
					// there are no more records in the DataReader so clean up
					Log.Debug("exhausted results");
					Current = default;
					_session.Batcher.CloseCommand(_command, _reader);
					return;
				}

				Log.Debug("retrieving next results");
				if (_single && _resultTransformer == null)
				{
					SetCurrent(_types[0].NullSafeGet(_reader, _columnNames[0], _session, null));
					return;
				}

				var currentResults = new object[_types.Length];
				// move through each of the ITypes contained in the DbDataReader and convert them
				// to their objects.  
				for (int i = 0; i < _types.Length; i++)
				{
					// The IType knows how to extract its value out of the DbDataReader.  If the IType
					// is a value type then the value will simply be pulled out of the DbDataReader.  If
					// the IType is an Entity type then the IType will extract the id from the DbDataReader
					// and use the ISession to load an instance of the object.
					currentResults[i] = _types[i].NullSafeGet(_reader, _columnNames[i], _session, null);
				}

				SetCurrent(_resultTransformer != null
						? _resultTransformer.TransformTuple(currentResults, _returnAliases)
						: currentResults);
			}

			private void SetCurrent(object value)
			{
				switch (value)
				{
					case T element:
						Current = element;
						break;
					case null:
						Current = default;
						break;
					default:
						throw new InvalidOperationException(
							$"An element of type {value.GetType()} was retrieved for an enumerable containing elements of type {typeof(T)}");
				}
			}

			private async Task InitializeAsync()
			{
				var result = await _initializeAsync(_cancellationToken).ConfigureAwait(false);
				_command = result.Command;
				_reader = result.DataReader;
			}

			private void Initialize()
			{
				var result = _initialize();
				_command = result.Command;
				_reader = result.DataReader;
			}
		}
	}
}
