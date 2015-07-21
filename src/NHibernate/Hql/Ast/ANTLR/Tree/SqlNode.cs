using System;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// A base AST node for the intermediate tree.
	/// </summary>
	[CLSCompliant(false)]
	public class SqlNode : ASTNode
	{
		/**
		 * The original text for the node, mostly for debugging.
		 */
		private String _originalText;

		/**
		 * The data type of this node.  Null for 'no type'.
		 */
		private IType _dataType;

		public SqlNode(IToken token) : base(token)
		{
			_originalText = token.Text;
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;

				if (!string.IsNullOrEmpty(value) && _originalText == null)
				{
					_originalText = value;
				}
			}
		}

		/// <summary>
		/// Retrieve the text to be used for rendering this particular node.
		/// </summary>
		/// <param name="sessionFactory">The session factory</param>
		/// <returns>The text to use for rendering</returns>
		public virtual SqlString RenderText(ISessionFactoryImplementor sessionFactory)
		{
			// The basic implementation is to simply use the node's text
			return new SqlString(Text);
		}

		public string OriginalText
		{
			get { return _originalText; }
		}

		public virtual IType DataType
		{
			get { return _dataType; }
			set { _dataType = value; }
		}
	}
}
