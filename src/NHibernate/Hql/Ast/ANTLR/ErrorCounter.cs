using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using log4net;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// An error handler that counts parsing errors and warnings.
	/// </summary>
	[CLSCompliant(false)]
	internal class ErrorCounter : IParseErrorHandler 
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ErrorCounter));
		private static readonly ILog hqlLog = LogManager.GetLogger("NHibernate.Hql.Parser");

		private readonly List<string> _errorList = new List<string>();
		private readonly List<string> _warningList = new List<string>();
		private readonly List<RecognitionException> _recognitionExceptions = new List<RecognitionException>();

		public void ReportError(RecognitionException e)
		{
			ReportError( e.ToString() );
			_recognitionExceptions.Add( e );
			if ( log.IsDebugEnabled ) {
				log.Debug( e.ToString(), e );
			}
		}

		public void ReportError(string message) 
		{
			hqlLog.Error( message );
			_errorList.Add( message );
		}

		public int GetErrorCount() 
		{
			return _errorList.Count;
		}

		public void ReportWarning(string message) 
		{
			hqlLog.Debug( message );
			_warningList.Add( message );
		}

		private string GetErrorString() 
		{
			bool first = true;
			StringBuilder buf = new StringBuilder();
			foreach (string error in _errorList) 
			{
				buf.Append(error);

				if (!first) buf.Append('\n');

				first = false;

			}
			return buf.ToString();
		}

		public void ThrowQueryException()
		{
			if ( GetErrorCount() > 0 ) 
			{
				if ( _recognitionExceptions.Count > 0 ) 
				{
					throw QuerySyntaxException.Convert(_recognitionExceptions[0] );
				}
				else 
				{
					throw new QueryException( GetErrorString() );
				}
			}
			else 
			{
				// all clear
				if ( log.IsDebugEnabled ) 
				{
					log.Debug( "throwQueryException() : no errors" );
				}
			}
		}
	}

}
