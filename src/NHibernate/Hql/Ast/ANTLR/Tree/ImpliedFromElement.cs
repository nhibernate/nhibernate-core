using System;
using System.Text;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class ImpliedFromElement : FromElement
	{
	    private bool _impliedInFromClause;
        private bool _inProjectionList;

		public ImpliedFromElement(IToken token) : base(token)
		{
		}

		public override bool IsImplied
		{
			get { return true; }
		}

        public override bool IsImpliedInFromClause
        {
            get { return _impliedInFromClause; }
        }

        public override void SetImpliedInFromClause(bool flag)
        {
            _impliedInFromClause = flag;
        }

        public override bool IncludeSubclasses
        {
            get
            {
                // Never include subclasses for implied from elements.
                return false;
            }
            set
            {
                base.IncludeSubclasses = value;
            }
        }

        public override bool InProjectionList
        {
            get
            {
                return _inProjectionList && IsFromOrJoinFragment;
            }
            set
            {
                _inProjectionList = value;
            }
        }

        public override string GetDisplayText()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("ImpliedFromElement{");
            AppendDisplayText(buf);
            buf.Append("}");
            return buf.ToString();
        }
	}
}
