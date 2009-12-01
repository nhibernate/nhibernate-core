using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Hql.Util;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Generates translators which uses the Antlr-based parser to perform
	/// the translation.
	/// 
	/// Author: Gavin King
	/// Ported by: Steve Strong
	/// </summary>
	public class ASTQueryTranslatorFactory : IQueryTranslatorFactory2
	{
        public IQueryTranslator[] CreateQueryTranslators(string queryString, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory)
		{
            var isFilter = collectionRole != null;
            var parser = new HqlParseEngine(queryString, isFilter, factory);
            parser.Parse();

            HqlParseEngine[] polymorphicParsers = AstPolymorphicProcessor.Process(parser, factory);

            var translators = polymorphicParsers
                            .Select(hql => new QueryTranslatorImpl(queryString, hql, filters, factory))
                            .ToArray();

            foreach (var translator in translators)
            {
                if (collectionRole == null)
                {
                    translator.Compile(factory.Settings.QuerySubstitutions, shallow);
                }
                else
                {
                    translator.Compile(collectionRole, factory.Settings.QuerySubstitutions, shallow);
                }
            }

            return translators;
		}

        public IQueryTranslator[] CreateQueryTranslators(string queryIdentifier, IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory)
        {
            var isFilter = collectionRole != null;
            var parser = new HqlParseEngine(queryExpression.Translate(factory), factory);

            HqlParseEngine[] polymorphicParsers = AstPolymorphicProcessor.Process(parser, factory);

            var translators = polymorphicParsers
                            .Select(hql => new QueryTranslatorImpl(queryIdentifier, hql, filters, factory))
                            .ToArray();

            foreach (var translator in translators)
            {
                if (collectionRole == null)
                {
                    translator.Compile(factory.Settings.QuerySubstitutions, shallow);
                }
                else
                {
                    translator.Compile(collectionRole, factory.Settings.QuerySubstitutions, shallow);
                }
            }

            return translators;
        }
	}

    public class AstPolymorphicProcessor
    {
        public static HqlParseEngine[] Process(HqlParseEngine parser, ISessionFactoryImplementor factory)
        {
            // Find all the polymorphic "froms"
            var fromDetector = new FromDetector(factory);
            var polymorphic = new NodeTraverser(fromDetector);
            polymorphic.TraverseDepthFirst(parser.Ast);

            if (fromDetector.Map.Count > 0)
            {
                var parsers = DuplicateTree(parser.Ast, fromDetector.Map);

                return parsers.Select(p => new HqlParseEngine(p, factory)).ToArray();
            }
            else
            {
                return new [] { parser };
            }
        }

        private static IEnumerable<IASTNode> DuplicateTree(IASTNode ast, IEnumerable<KeyValuePair<IASTNode, IASTNode[]>> nodeMapping)
        {
            var replacements = ExpandDictionaryArrays(nodeMapping);

            var dups = new IASTNode[replacements.Count()];

            for (var i = 0; i < replacements.Count(); i++)
            {
                dups[i] = DuplicateTree(ast, replacements[i]);
            }

            return dups;
        }

        private static IASTNode DuplicateTree(IASTNode ast, Dictionary<IASTNode, IASTNode> nodeMapping)
        {
            IASTNode candidate;

            if (nodeMapping.TryGetValue(ast, out candidate))
            {
                return candidate;
            }

            var dup = ast.DupNode();

            foreach (var child in ast)
            {
                dup.AddChild(DuplicateTree(child, nodeMapping));
            }

            return dup;
        }

        static IList<Dictionary<IASTNode, IASTNode>> ExpandDictionaryArrays(IEnumerable<KeyValuePair<IASTNode, IASTNode[]>> input)
        {
            return (from list in ExpandDictionaryArraysInner(input)
                    select list.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)).ToList();
        }

        static IEnumerable<IEnumerable<KeyValuePair<IASTNode, IASTNode>>> ExpandDictionaryArraysInner(IEnumerable<KeyValuePair<IASTNode, IASTNode[]>> input)
        {
            var output = new List<IEnumerable<KeyValuePair<IASTNode, IASTNode>>>();

            foreach (var value in input.First().Value)
            {
                var inner = new List<KeyValuePair<IASTNode, IASTNode>>
                                {new KeyValuePair<IASTNode, IASTNode>(input.First().Key, value)};

                if (input.Count() > 1)
                {
                    output.AddRange(ExpandDictionaryArraysInner(input.Skip(1)).Select(c => c.Union(inner)));
                }
                else
                {
                    output.Add(inner);
                }
            }

            return output;
        }

    }


    internal class FromDetector : IVisitationStrategy
    {
        private readonly ISessionFactoryImplementor _sfi;
        private readonly Dictionary<IASTNode, IASTNode[]> _map = new Dictionary<IASTNode, IASTNode[]>();

        public FromDetector(ISessionFactoryImplementor sfi)
        {
            _sfi = sfi;
        }

        public IDictionary<IASTNode, IASTNode[]> Map
        {
            get { return _map; }
        }

        public void Visit(IASTNode node)
        {
            if (node.Type == HqlSqlWalker.FROM && node.ChildCount > 0)
            {
                foreach (var child in node)
                {
                    string className = null;
                    IASTNode identifer = null;

                    if (child.Type == HqlSqlWalker.RANGE)
                    {
                        identifer = child.GetChild(0);

                        if (identifer.Type == HqlSqlWalker.IDENT)
                        {
                            className = identifer.Text;
                        }
                        else if (identifer.Type == HqlSqlWalker.DOT)
                        {
                            className = BuildPath(identifer);
                        }
                        else
                        {
                            // TODO
                            throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        // TODO - stuff for joins?
                    }

                    if (className != null)
                    {
                        System.Type classType = (new SessionFactoryHelper(_sfi)).GetImportedClass(className);

                        if (classType != null)
                        {
                            string[] implementors = _sfi.GetImplementors(classType.FullName);

                            if (implementors != null)
                            {
                                if (implementors.Length == 1 &&
                                    ((implementors[0] == className) || (implementors[0] == classType.FullName)))
                                {
                                    // No need to change things
                                    return;
                                }

                                Map.Add(identifer,
                                        implementors.Select(implementor => MakeIdent(identifer, implementor)).ToArray());
                            }
                        }
                    }
                }
            }
        }

        private IASTNode MakeIdent(IASTNode source, string text)
        {
            var ident = source.DupNode();
            ident.Type = HqlSqlWalker.IDENT;
            ident.Text = text;
            return ident;
        }

        private static string BuildPath(IASTNode node)
        {
            var sb = new StringBuilder();
            BuildPath(node, sb);
            return sb.ToString();
        }

        private static void BuildPath(IASTNode node, StringBuilder sb)
        {
            if (node.Type == HqlSqlWalker.DOT)
            {
                BuildPath(node.GetChild(0), sb);

                sb.Append('.');
                sb.Append(node.GetChild(1).Text);
            }
            else
            {
                sb.Append(node.Text);
            }
        }
    }

}
