using System;
using System.Collections;
using System.Text;

namespace NHibernate.Util {
	/// <summary>
	/// A StringTokenizer java like object 
	/// </summary>
	public class StringTokenizer : IEnumerable {

		private static readonly string _defaultDelim=" \t\n\r\f";
		string _origin;
		string _delim;
		bool _returnDelim;

		public StringTokenizer(string str) {
			_origin = str;
			_delim = _defaultDelim;
			_returnDelim = false;
		}

		public StringTokenizer(string str, string delim) {
			_origin = str;
			_delim = delim;
			_returnDelim = true;
		}

		public StringTokenizer(string str, string delim, bool returnDelims) {
			_origin = str;
			_delim = delim;
			_returnDelim = returnDelims;
		}

		public IEnumerator GetEnumerator() {
			return new StringTokenizerEnumerator(this);
		}

		private class StringTokenizerEnumerator : IEnumerator {
			private StringTokenizer _stokenizer;
			private int _cursor = 0;
			private String _next = null;
			
			public StringTokenizerEnumerator(StringTokenizer stok) {
				_stokenizer = stok;
			}

			public bool MoveNext() {
				_next = GetNext();
				if (_next == null)
					return false;
				return true;
			}

			public void Reset() {
				_cursor = 0;
			}

			public object Current {
				get {
					return _next;
				}
			}

			private string GetNext() {
				StringBuilder sb;
				char c;
				bool isDelim;
				
				if( _cursor == _stokenizer._origin.Length )
					return null;

				sb = new StringBuilder();
				
				c = _stokenizer._origin[_cursor];
				isDelim = (_stokenizer._delim.IndexOf(c) != -1);
				
				if( isDelim ) {
					_cursor++;
					if ( _stokenizer._returnDelim ) {
						return c.ToString();
					}
					return GetNext();
				}

				while( _cursor < _stokenizer._origin.Length && !isDelim ) {
					sb.Append(c);
					if( ++_cursor == _stokenizer._origin.Length)
						break;
					c = _stokenizer._origin[_cursor];
					isDelim = (_stokenizer._delim.IndexOf(c) != -1);
				}
				
				return sb.ToString();
			}

		}
	}
}