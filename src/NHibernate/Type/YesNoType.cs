using System;

using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a System.Boolean to a single character column that stores a 
	/// "Y"/"N" to indicate true/false.
	/// </summary>
	public class YesNoType : CharBooleanType {

		internal YesNoType(AnsiStringFixedLengthSqlType sqlType) : base(sqlType) {
		}

		protected override sealed string TrueString {
			get { return "Y"; }
		}

		protected override sealed string FalseString {
			get { return "N"; }
		}

		public override string Name {
			get { return "YesNo"; }
		}
	}
}