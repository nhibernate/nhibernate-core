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
	internal struct InitializeResult
	{
		public InitializeResult(DbCommand command, DbDataReader dataReader)
		{
			Command = command;
			DataReader = dataReader;
		}

		public DbCommand Command { get; }

		public DbDataReader DataReader { get; }
	}

	internal delegate Task<InitializeResult> InitializeAsync(CancellationToken cancellationToken);

	internal class AsyncEnumerableImpl<T> : IAsyncEnumerable<T>
	{
		private readonly InitializeAsync _initializeAsync;
		private readonly bool _readOnly;
		private readonly IType[] _types;
		private string[][] _columnNames;
		private readonly RowSelection _selection;
		private readonly IResultTransformer _resultTransformer;
		private readonly string[] _returnAliases;
		private readonly IEventSource _session;

		public AsyncEnumerableImpl(
			InitializeAsync initializeAsync,
			bool readOnly,
			IType[] types,
			string[][] columnNames,
			RowSelection selection,
			IResultTransformer resultTransformer,
			string[] returnAliases,
			IEventSource session)
		{
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

		private sealed class AsyncEnumeratorImpl : IAsyncEnumerator<T>
		{
			private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(AsyncEnumeratorImpl));

			private readonly InitializeAsync _initializeAsync;
			private readonly bool _readOnly;
			private readonly IType[] _types;
			private string[][] _columnNames;
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
				InitializeAsync initializeAsync,
				bool readOnly,
				IType[] types,
				string[][] columnNames,
				RowSelection selection,
				IResultTransformer resultTransformer,
				string[] returnAliases,
				IEventSource session,
				CancellationToken cancellationToken)
			{
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

			public async ValueTask<bool> MoveNextAsync()
			{
				if (_reader == null)
				{
					await InitializeAsync().ConfigureAwait(false);
				}

				try
				{
					_hasNext = await _reader.ReadAsync(_cancellationToken).ConfigureAwait(false);
					_startedReading = true;
					_currentRow++;
				}
				catch (DbException e)
				{
					throw ADOExceptionHelper.Convert(_session.Factory.SQLExceptionConverter, e, "Error executing Enumerable() query",
													   new SqlString(_command.CommandText));
				}

				if (_selection != null && _selection.MaxRows != RowSelection.NoValue)
				{
					_hasNext = _hasNext && (_currentRow < _selection.MaxRows);
				}

				bool sessionDefaultReadOnlyOrig = _session.DefaultReadOnly;
				_session.DefaultReadOnly = _readOnly;
				bool isCurrentValid;
				try
				{
					isCurrentValid = await PostMoveNextAsync().ConfigureAwait(false);
				}
				finally
				{
					_session.DefaultReadOnly = sessionDefaultReadOnlyOrig;
				}

				// In order to simulate what SafetyEnumerable does, we have to skip elements of different type
				if (!isCurrentValid)
				{
					await MoveNextAsync().ConfigureAwait(false);
				}

				return _hasNext;
			}

			private async Task<bool> PostMoveNextAsync()
			{
				if (!_hasNext)
				{
					// there are no more records in the DataReader so clean up
					log.Debug("exhausted results");
					Current = default;
					_session.Batcher.CloseCommand(_command, _reader);
					return true;
				}

				log.Debug("retrieving next results");
				if (_single && _resultTransformer == null)
				{
					return TrySetCurrent(await _types[0].NullSafeGetAsync(_reader, _columnNames[0], _session, null, _cancellationToken).ConfigureAwait(false));
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
					currentResults[i] = await _types[i].NullSafeGetAsync(_reader, _columnNames[i], _session, null, _cancellationToken).ConfigureAwait(false);
				}

				return TrySetCurrent(_resultTransformer != null
						? _resultTransformer.TransformTuple(currentResults, _returnAliases)
						: currentResults);
			}

			private bool TrySetCurrent(object value)
			{
				if (value is T element)
				{
					Current = element;
					return true;
				}
				else if (value == null)
				{
					Current = default;
					return true;
				}

				return false;
			}

			private async Task InitializeAsync()
			{
				var result = await _initializeAsync(_cancellationToken).ConfigureAwait(false);
				_command = result.Command;
				_reader = result.DataReader;
			}

			/// <summary>
			/// Takes care of freeing the managed and unmanaged resources that this class is responsible for.
			/// </summary>
			/// <remarks>
			/// The command is closed and the reader is disposed. This allows other ADO.NET
			/// related actions to occur without needing to move all the way through the
			/// AsyncEnumeratorImpl.
			/// </remarks>
			public ValueTask DisposeAsync()
			{
				log.Debug($"running {nameof(AsyncEnumeratorImpl)}.{nameof(AsyncEnumeratorImpl.DisposeAsync)}()");

				if (_isAlreadyDisposed)
				{
					// don't dispose of multiple times.
					return default;
				}

				// if there is still a possibility of moving next then we need to clean up
				// the resources - otherwise the cleanup has already been done by the 
				// PostMoveNext method.
				if (_hasNext || !_startedReading)
				{
					Current = default;
					_session.Batcher.CloseCommand(_command, _reader);
				}

				_isAlreadyDisposed = true;

				return default;
			}
		}
	}
}
