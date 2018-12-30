﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Indicates a condition where an instrumented/enhanced class was expected, but the class was not
	/// instrumented/enhanced.
	/// 
	/// Author: Steve Ebersole
	/// </summary>
	[Serializable]
	public class NotInstrumentedException : HibernateException
	{
		/// <summary>
		/// Constructs a NotInstrumentedException.
		/// </summary>
		/// <param name="message">Message explaining the exception condition.</param>
		public NotInstrumentedException(string message) : base(message)
		{
		}

		/// <inheritdoc />
		protected NotInstrumentedException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}
}
