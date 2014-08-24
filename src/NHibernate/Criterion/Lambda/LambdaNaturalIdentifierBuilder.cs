
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class LambdaNaturalIdentifierBuilder
	{
		private NaturalIdentifier naturalIdentifier;
		private string propertyName;

		public LambdaNaturalIdentifierBuilder(NaturalIdentifier naturalIdentifier, string propertyName)
		{
			this.naturalIdentifier = naturalIdentifier;
			this.propertyName = propertyName;
		}

		public NaturalIdentifier Is(object value)
		{
			return naturalIdentifier.Set(propertyName, value);
		}

	}

}
