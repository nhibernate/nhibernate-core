using System;
using System.Text;
using Antlr.Runtime;


namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public abstract class FromReferenceNode : AbstractSelectExpression, IResolvableNode, IDisplayableNode, IPathNode
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(FromReferenceNode));

		public const int RootLevel = 0;
		private FromElement _fromElement;
		private bool _resolved;

		protected FromReferenceNode(IToken token) : base(token)
		{
		}

		public override bool IsReturnableEntity
		{
			get { return !IsScalar && _fromElement.IsEntity; }
		}
		
		public override FromElement FromElement
		{
			get { return _fromElement; }
			set { _fromElement = value; }
		}

		public bool IsResolved
		{
			get { return _resolved; }
			set
			{
				_resolved = true;
				if (Log.IsDebugEnabled)
				{
					Log.Debug("Resolved :  " + Path + " -> " + Text);
				}
			}
		}

		public string GetDisplayText()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("{").Append((_fromElement == null) ? "no fromElement" : _fromElement.GetDisplayText());
			buf.Append("}");
			return buf.ToString();
		}

		public abstract void Resolve(bool generateJoin, bool implicitJoin, string classAlias, IASTNode parent);

		public void Resolve(bool generateJoin, bool implicitJoin, string classAlias)
		{
			Resolve(generateJoin, implicitJoin, classAlias, null);
		}

		public void Resolve(bool generateJoin, bool implicitJoin)
		{
			Resolve(generateJoin, implicitJoin, null);
		}

		public virtual void ResolveInFunctionCall(bool generateJoin, bool implicitJoin)
		{
			Resolve(generateJoin, implicitJoin, null);
		}

		public abstract void ResolveIndex(IASTNode parent);

		public virtual string Path
		{
			get { return OriginalText; }
		}

		public void RecursiveResolve(int level, bool impliedAtRoot, string classAlias, IASTNode parent)
		{
			IASTNode lhs = GetFirstChild();
			int nextLevel = level + 1;
			if ( lhs != null ) 
			{
				FromReferenceNode n = ( FromReferenceNode ) lhs;
				n.RecursiveResolve( nextLevel, impliedAtRoot, null, this );
			}

			ResolveFirstChild();
			bool impliedJoin = !(level == RootLevel && !impliedAtRoot);

			Resolve( true, impliedJoin, classAlias, parent );
		}

		/// <summary>
		/// Sub-classes can override this method if they produce implied joins (e.g. DotNode).
		/// </summary>
		/// <returns>an implied join created by this from reference.</returns>
		public virtual FromElement GetImpliedJoin()
		{
			return null;
		}

		public virtual void PrepareForDot(string propertyName)
		{
		}

		public virtual void ResolveFirstChild()
		{
		}
	}
}
