using System;

using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a System.Boolean to a single character column that stores a 
	/// "T"/F" to indicate true/false.
	/// </summary>
	public class TrueFalseType: CharBooleanType 
	{

		internal TrueFalseType(AnsiStringFixedLengthSqlType sqlType) : base(sqlType) {
		}

		protected override sealed string TrueString {
			get { return "T"; }
		}

		protected override sealed string FalseString {
			get { return "F"; }
		}

		public override string Name {
			get { return "TrueFalse"; }
		}
	}
}