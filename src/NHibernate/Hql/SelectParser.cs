using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using NHibernate;
using NHibernate.Util;
using NHibernate.Type;

namespace NHibernate.Hql {
	
	/// <summary>
	/// Parsers the select clause of a hibernate query, looking
	/// for a table (well, really class) alias.
	/// </summary>
	public class SelectParser : IParser {
		private static StringCollection aggregateFunctions = new StringCollection();
		private static StringCollection countArguments = new StringCollection();
		
		static SelectParser() {
			aggregateFunctions.Add("count");
			aggregateFunctions.Add("avg");
			aggregateFunctions.Add("max");
			aggregateFunctions.Add("min");
			aggregateFunctions.Add("sum");
				
			countArguments.Add("distinct");
			countArguments.Add("all");
			countArguments.Add("*");
		}
		
		private bool ready;
		private bool aggregate;
		private bool count;
		private bool avg;
		private bool first;
		private bool afterNew;
		private System.Type holderClass;

		private SelectPathExpressionParser pathExpressionParser = new SelectPathExpressionParser();
		private PathExpressionParser aggregatePathExpressionParser = new PathExpressionParser();		
		
		public void Token(string token, QueryTranslator q) {
			
			string lctoken = token.ToLower();
			
			if (first) {
				first = false;
				if (lctoken.Equals("distinct")) {
					q.Distinct = true;
					return ;
				} else if (lctoken.Equals("all")) {
					q.Distinct = false;
					return ;
				}
			}
			
			if (afterNew) {
				afterNew=false;
				holderClass = q.GetImportedClass(token);
				if (holderClass==null) throw new QueryException("class not found: " + token);
				q.HolderClass = holderClass;
			} else if (token.Equals(StringHelper.Comma)) {
				if (ready) throw new QueryException("alias or expression expected in SELECT");
				q.AppendScalarSelectToken(StringHelper.CommaSpace);
				ready = true;
			} else if ( "new".Equals(lctoken) ) {
				afterNew=true;
				ready=false;
			} else if (StringHelper.OpenParen.Equals(token)) {
				if (!aggregate && holderClass!=null && !ready) {
					//opening paren in new Foo ( ... )
					ready=true;
				} else if (aggregate) {
					q.AppendScalarSelectToken(token);
				} else {
					throw new QueryException("aggregate function expected before ( in SELECT");
				}
				ready = true;
			} else if (StringHelper.ClosedParen.Equals(token)) {
				if (holderClass!=null && !ready) {
					//
				} else if (aggregate && ready) {
					q.AppendScalarSelectToken(token);
				} else {
					throw new QueryException("( expected before ) in select");
				}
				count = false;
				aggregate = false;
				ready = false;
			} else if (countArguments.Contains(lctoken)) {
				if (!ready || !aggregate) throw new QueryException(token + " only allowed inside aggregate function in SELECT");
				q.AppendScalarSelectToken(token);
			} else if (aggregateFunctions.Contains(lctoken)) {
				if (!ready) throw new QueryException(", expected before aggregate function in SELECT: " + token);
				if (lctoken.Equals("count")) {
					q.AddSelectScalar(NHibernate.Int32);//must be handled differently 'cos of count(*)
					count = true;
				}
				else if( lctoken.Equals("avg") ) {
					avg = true;
				}
				aggregate = true;
				ready = false;
				q.AppendScalarSelectToken(token);
			} else if (aggregate) {
				if (!ready) throw new QueryException("( expected after aggregate function in SELECT");
				ParserHelper.Parse(aggregatePathExpressionParser, q.Unalias(token), ParserHelper.PathSeparators, q);
				
				if (aggregatePathExpressionParser.IsCollectionValued) {
					q.AddCollection(
						aggregatePathExpressionParser.CollectionName, 
						aggregatePathExpressionParser.CollectionRole);
				}
				q.AppendScalarSelectToken(aggregatePathExpressionParser.WhereColumn);
				if (!count) q.AddSelectScalar( AggregateType(aggregatePathExpressionParser.WhereColumnType, q) );
				aggregatePathExpressionParser.AddAssociation(q);
			} else {
				if (!ready) throw new QueryException(", expected in SELECT");
				
				ParserHelper.Parse(pathExpressionParser, token, ParserHelper.PathSeparators, q);
				if (pathExpressionParser.IsCollectionValued) {
					q.AddCollection(
						pathExpressionParser.CollectionName, 
						pathExpressionParser.CollectionRole);
				} else if (pathExpressionParser.WhereColumnType.IsEntityType) {
					q.AddSelectClass(pathExpressionParser.SelectName);
				}
				q.AppendScalarSelectTokens(pathExpressionParser.WhereColumns);
				q.AddSelectScalar(pathExpressionParser.WhereColumnType);
				pathExpressionParser.AddAssociation(q);
				
				ready = false;
			}
		}

		public IType AggregateType(IType type, QueryTranslator q) { 
			if (count) { 
				throw new AssertionFailure("count(*) must be handled differently"); 
			} 
			else if (avg) { 
				SqlTypes.SqlType[] sqlTypes;
				
				try { 
					sqlTypes = type.SqlTypes(q.factory);
				} 
				catch (MappingException me) { 
					throw new QueryException(me); 
				} 

				if (sqlTypes.Length!=1) throw new QueryException("multi-column type in avg()"); 
				DbType sqlDbType = sqlTypes[0].DbType; 
				if ( sqlDbType==DbType.Int32 || sqlDbType==DbType.Int64 || sqlDbType==DbType.Int16 ) { 
					return NHibernate.Single; 
				} 
				else { 
					return type; 
				} 
			    
			} 
			else { 
				return type; 
			} 
		} 

		public void Start(QueryTranslator q) {
			ready = true;
			first = true;
			aggregate = false;
			count = false;
			avg = false;
			afterNew = false;
			holderClass = null;
		}
		
		public void End(QueryTranslator q) {
		}
	}
}