// Copyright (c) 2022 MOCCA Software GmbH. All rights reserved.
// 
// All rights are reserved. Reproduction or transmission in whole or in part,
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// File Name: QueryLoaderFactory.cs
// Created: 12/12/2022
// Author:  Michael Kaufmann (mika)

using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Loader.Hql
{
	/// <summary>
	/// Creates query loaders.
	/// </summary>
	public class QueryLoaderFactory: IQueryLoaderFactory
	{
		/// <summary>
		/// Creates a query loader.
		/// </summary>
		/// <param name="queryTranslator"></param>
		/// <param name="factory"></param>
		/// <param name="selectClause"></param>
		/// <returns></returns>
		public IQueryLoader Create(QueryTranslatorImpl queryTranslator, ISessionFactoryImplementor factory, SelectClause selectClause)
		{
			return new QueryLoader(queryTranslator, factory, selectClause);
		}
	}
}
