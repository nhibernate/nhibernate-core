using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Test
{
	public static class ExpressionEqualityChecker
	{
		public static bool ExpressionEquals(this Expression source,Expression toBeCompared)
		{
			if(object.Equals(source,toBeCompared))
				return true;
			else if ((source != null && toBeCompared == null) || (source == null && toBeCompared != null))
			{
				return false;
			}
			else if (source.NodeType == toBeCompared.NodeType && source.Type==toBeCompared.Type)
			{
					System.Type sourceType = source.GetType();
					var properties = sourceType.GetProperties();
					bool value = true;

					for (int i = 0; i < properties.Length;i++ )
					{
						var info = properties[i];
						var leftValue = info.GetValue(source, null);
						var rightValue = info.GetValue(toBeCompared, null);
						if (leftValue is Expression)
						{
							value &= ExpressionEquals((Expression)leftValue, (Expression)rightValue);
						}
						else
							value &= object.Equals(leftValue,rightValue);
					}
					return value;
			}
			else 
				return false;
		}
	}
}
