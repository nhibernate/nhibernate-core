using System;

namespace NHibernate.Type
{
	/// <summary>
	/// TrueFalseType.
	/// </summary>
	public class TrueFalseType: CharBooleanType {

		protected override sealed string TrueString {
			get { return "T"; }
		}

		protected override sealed string FalseString {
			get { return "F"; }
		}

		public override string Name {
			get { return "true_false"; }
		}
	}
}