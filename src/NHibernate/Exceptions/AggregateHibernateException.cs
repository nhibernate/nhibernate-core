using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace NHibernate.Exceptions
{
	/// <summary>
	/// Exception aggregating exceptions that occurs in the O-R persistence layer.
	/// </summary>
	[Serializable]
	public class AggregateHibernateException : HibernateException
	{
		public ReadOnlyCollection<Exception> InnerExceptions { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AggregateHibernateException"/> class.
		/// </summary>
		/// <param name="innerExceptions">The exceptions to aggregate.</param>
		public AggregateHibernateException(IEnumerable<Exception> innerExceptions) :
			this(
				$"Exceptions occurred in the persistence layer, check {nameof(InnerExceptions)} property.",
				innerExceptions)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AggregateHibernateException"/> class.
		/// </summary>
		/// <param name="innerExceptions">The exceptions to aggregate.</param>
		public AggregateHibernateException(params Exception[] innerExceptions) :
			this(innerExceptions?.ToList())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerExceptions">The exceptions to aggregate.</param>
		public AggregateHibernateException(string message, IEnumerable<Exception> innerExceptions) :
			this(message, innerExceptions?.ToList())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerExceptions">The exceptions to aggregate.</param>
		public AggregateHibernateException(string message, params Exception[] innerExceptions) :
			this(message, innerExceptions?.ToList())
		{
		}

		private AggregateHibernateException(string message, IList<Exception> innerExceptions) :
			base(
				message,
				innerExceptions?.FirstOrDefault())
		{
			if (innerExceptions == null)
				throw new ArgumentNullException(nameof(innerExceptions));
			if (innerExceptions.Count == 0)
				throw new ArgumentException("Exceptions list to aggregate is empty", nameof(innerExceptions));
			if (innerExceptions.Any(e => e == null))
				throw new ArgumentException("Exceptions list to aggregate contains a null exception", nameof(innerExceptions));
			InnerExceptions = new ReadOnlyCollection<Exception>(innerExceptions);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AggregateHibernateException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected AggregateHibernateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			InnerExceptions = (ReadOnlyCollection<Exception>) info.GetValue("InnerExceptions", typeof(ReadOnlyCollection<Exception>));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("InnerExceptions", InnerExceptions);
		}

		/// <summary>
		/// Return a string representation of the aggregate exception.
		/// </summary>
		/// <returns>A string representation with inner exceptions.</returns>
		public override string ToString()
		{
			var str = new StringBuilder(base.ToString());
			for (var i = 0; i < InnerExceptions.Count; i++)
			{
				str
					.AppendLine()
					.Append("---> (Inner exception #").Append(i).Append(") ")
					.Append(InnerExceptions[i]).AppendLine("<---");
			}
			return str.ToString();
		}
	}
}
