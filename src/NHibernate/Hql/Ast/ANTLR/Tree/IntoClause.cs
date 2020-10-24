using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using NHibernate.Persister.Entity;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents an entity referenced in the INTO clause of an HQL
	/// INSERT statement.
	/// 
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class IntoClause : HqlSqlWalkerNode, IDisplayableNode
	{
		private IQueryable _persister;
		private String _columnSpec = string.Empty;
		private IType[] _types;

		private bool _discriminated;
		private bool _explicitIdInsertion;
		private bool _explicitVersionInsertion;

		public IntoClause(IToken token)
			: base(token)
		{
		}

		public void Initialize(IQueryable persister)
		{
			if (persister.IsAbstract)
			{
				throw new QueryException("cannot insert into abstract class (no table)");
			}

			_persister = persister;
			InitializeColumns();

			if (Walker.SessionFactoryHelper.HasPhysicalDiscriminatorColumn(persister))
			{
				_discriminated = true;
				_columnSpec += ", " + persister.DiscriminatorColumnName;
			}

			ResetText();
		}

		private void ResetText()
		{
			Text = "into " + TableName + " ( " + _columnSpec + " )";
		}

		private string TableName
		{
			get { return _persister.GetSubclassTableName(0); }
		}

		public IQueryable Queryable
		{
			get { return _persister; }
		}

		private string EntityName
		{
			get { return _persister.EntityName; }
		}

		public bool IsDiscriminated
		{
			get { return _discriminated; }
		}

		public bool IsExplicitIdInsertion
		{
			get { return _explicitIdInsertion; }
		}

		public bool IsExplicitVersionInsertion
		{
			get { return _explicitVersionInsertion; }
		}

		public void PrependIdColumnSpec()
		{
			_columnSpec = _persister.IdentifierColumnNames[0] + ", " + _columnSpec;
			ResetText();
		}

		public void PrependVersionColumnSpec()
		{
			_columnSpec = _persister.GetPropertyColumnNames(_persister.VersionProperty)[0] + ", " + _columnSpec;
			ResetText();
		}

		public void ValidateTypes(SelectClause selectClause)
		{
			IType[] selectTypes = selectClause.QueryReturnTypes;

			if (selectTypes.Length != _types.Length)
			{
				throw new QueryException("number of select types did not match those for insert");
			}

			for (int i = 0; i < _types.Length; i++)
			{
                if (selectTypes[i] == null)
                    selectTypes[i] = _types[i];
                else
                {
                    if (!AreCompatible(_types[i], selectTypes[i]))
                    {
                        throw new QueryException(
                                "insertion type [" + _types[i] + "] and selection type [" +
                                selectTypes[i] + "] at position " + i + " are not compatible"
                        );
                    }
                }
			}

			// otherwise, everything ok.
		}

		/// <summary>
		/// Returns additional display text for the AST node.
		/// </summary>
		/// <returns>The additional display text.</returns>
		public string GetDisplayText()
		{
			var buf = new StringBuilder();
			buf.Append("IntoClause{");
			buf.Append("entityName=").Append(EntityName);
			buf.Append(",tableName=").Append(TableName);
			buf.Append(",columns={").Append(_columnSpec).Append("}");
			buf.Append("}");
			return buf.ToString();
		}

		private void InitializeColumns()
		{
			IASTNode propertySpec = GetFirstChild();
			var ts = new List<IType>();
			VisitPropertySpecNodes(propertySpec.GetFirstChild(), ts);
			_types = ts.ToArray();
			_columnSpec = _columnSpec.Substring(0, _columnSpec.Length - 2);
		}

		private void VisitPropertySpecNodes(IASTNode propertyNode, ICollection<IType> types)
		{
			if (propertyNode == null)
			{
				return;
			}

			// TODO : we really need to be able to deal with component paths here also;
			// this is difficult because the hql-sql grammar expects all those node types
			// to be FromReferenceNodes.  One potential fix here would be to convert the
			// IntoClause to just use a FromClause/FromElement combo (as a child of the
			// InsertStatement) and move all this logic into the InsertStatement.  That's
			// probably the easiest approach (read: least amount of changes to the grammar
			// and code), but just doesn't feel right as then an insert would contain
			// 2 from-clauses
			string name = propertyNode.Text;

			if (IsSuperclassProperty(name))
			{
				throw new QueryException("INSERT statements cannot refer to superclass/joined properties [" + name + "]");
			}

			if (name == _persister.IdentifierPropertyName)
			{
				_explicitIdInsertion = true;
			}

			if (_persister.IsVersioned)
			{
				if (name == _persister.PropertyNames[_persister.VersionProperty])
				{
					_explicitVersionInsertion = true;
				}
			}

			string[] columnNames = _persister.ToColumns(name);
			RenderColumns(columnNames);
			types.Add(_persister.ToType(name));

			// visit width-first, then depth
			VisitPropertySpecNodes(propertyNode.NextSibling, types);
			VisitPropertySpecNodes(propertyNode.GetFirstChild(), types);
		}

		private void RenderColumns(string[] columnNames)
		{
			for (int i = 0; i < columnNames.Length; i++)
			{
				_columnSpec += columnNames[i] + ", ";
			}
		}

		private bool IsSuperclassProperty(string propertyName)
		{
			// really there are two situations where it should be ok to allow the insertion
			// into properties defined on a superclass:
			//      1) union-subclass with an abstract root entity
			//      2) discrim-subclass
			//
			// #1 is handled already because of the fact that
			// UnionSubclassPersister alreay always returns 0
			// for this call...
			//
			// we may want to disallow it for discrim-subclass just for
			// consistency-sake (currently does not work anyway)...
			return _persister.GetSubclassPropertyTableNumber(propertyName) != 0;
		}

		/// <summary>
		/// Determine whether the two types are "assignment compatible".
		/// </summary>
		/// <param name="target">The type defined in the into-clause.</param>
		/// <param name="source">The type defined in the select clause.</param>
		/// <returns>True if they are assignment compatible.</returns>
		private bool AreCompatible(IType target, IType source)
		{
			if (target.Equals(source))
			{
				// if the types report logical equivalence, return true...
				return true;
			}

			// otherwise, perform a "deep equivalence" check...

			if (!target.ReturnedClass.IsAssignableFrom(source.ReturnedClass))
			{
				return false;
			}

			SqlType[] targetDatatypes = target.SqlTypes(SessionFactoryHelper.Factory);
			SqlType[] sourceDatatypes = source.SqlTypes(SessionFactoryHelper.Factory);

			if (targetDatatypes.Length != sourceDatatypes.Length)
			{
				return false;
			}

			for (int i = 0; i < targetDatatypes.Length; i++)
			{
				if (!AreSqlTypesCompatible(targetDatatypes[i], sourceDatatypes[i]))
				{
					return false;
				}
			}

			return true;
		}

		private static bool AreSqlTypesCompatible(SqlType target, SqlType source)
		{
			// TODO NH: May be are not equals but are compatible
			return target.Equals(source);
		}
	}
}