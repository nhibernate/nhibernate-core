using System;
using System.Collections.Generic;

namespace NHibernate.Util
{
	/// <summary>
	/// A StringTokenizer java like object 
	/// </summary>
	public class StringTokenizer : IEnumerable<string>
	{
		private const string _defaultDelim = " \t\n\r\f";
		private string _origin;
		private string _delim;
		private bool _returnDelim;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		public StringTokenizer(string str)
		{
			_origin = str;
			_delim = _defaultDelim;
			_returnDelim = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="delim"></param>
		public StringTokenizer(string str, string delim)
		{
			_origin = str;
			_delim = delim;
			_returnDelim = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="delim"></param>
		/// <param name="returnDelims"></param>
		public StringTokenizer(string str, string delim, bool returnDelims)
		{
			_origin = str;
			_delim = delim;
			_returnDelim = returnDelims;
		}

		#region IEnumerable<string> Members

		public IEnumerator<string> GetEnumerator()
		{
			return new StringTokenizerEnumerator(this);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new StringTokenizerEnumerator(this);
		}

		#endregion


		private class StringTokenizerEnumerator : IEnumerator<string>
		{
			private StringTokenizer _stokenizer;
			private int _cursor = 0;
			private String _next = null;

			public StringTokenizerEnumerator(StringTokenizer stok)
			{
				_stokenizer = stok;
			}

			#region IEnumerator<string> Members

			public string Current
			{
				get { return _next; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
			  _next = GetNext();
			  return _next != null;
			}

			public void Reset()
			{
				_cursor = 0;
			}

			#endregion

			private string GetNext()
			{
				char c;
				bool isDelim;

				if (_cursor >= _stokenizer._origin.Length)
					return null;

				c = _stokenizer._origin[_cursor];
				isDelim = (_stokenizer._delim.IndexOf(c) != -1);

				if (isDelim)
				{
					_cursor++;
					if (_stokenizer._returnDelim)
					{
						return c.ToString();
					}
					return GetNext();
				}

				int nextDelimPos = _stokenizer._origin.IndexOfAny(_stokenizer._delim.ToCharArray(), _cursor);
				if (nextDelimPos == -1)
				{
					nextDelimPos = _stokenizer._origin.Length;
				}

				string nextToken = _stokenizer._origin.Substring(_cursor, nextDelimPos - _cursor);
				_cursor = nextDelimPos;
				return nextToken;
			}
		}
	}
}