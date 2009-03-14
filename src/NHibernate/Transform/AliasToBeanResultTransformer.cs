using System;
using System.Collections;
using NHibernate.Properties;

namespace NHibernate.Transform
{
	/// <summary>
	/// Result transformer that allows to transform a result to 
	/// a user specified class which will be populated via setter  
	/// methods or fields matching the alias names. 
	/// </summary>
	/// <example>
	/// <code>
	/// IList resultWithAliasedBean = s.CreateCriteria(typeof(Enrollment))
	/// 			.CreateAlias("Student", "st")
	/// 			.CreateAlias("Course", "co")
	/// 			.SetProjection( Projections.ProjectionList()
	/// 					.Add( Projections.Property("co.Description"), "CourseDescription" )
	/// 			)
	/// 			.SetResultTransformer( new AliasToBeanResultTransformer(typeof(StudentDTO)) )
	/// 			.List();
	/// 
	/// StudentDTO dto = (StudentDTO)resultWithAliasedBean[0];
	/// </code>
	/// </example>
	[Serializable]
	public class AliasToBeanResultTransformer : IResultTransformer
	{
		private readonly System.Type resultClass;
		private ISetter[] setters;
		private IPropertyAccessor propertyAccessor;

		public AliasToBeanResultTransformer(System.Type resultClass)
		{
			if (resultClass == null)
				throw new ArgumentNullException("resultClass");
			this.resultClass = resultClass;
			propertyAccessor = new ChainedPropertyAccessor(
				new IPropertyAccessor[]
					{
						// TODO H3:	PropertyAccessorFactory.GetPropertyAccessor(resultClass, null),
						PropertyAccessorFactory.GetPropertyAccessor(null),
						PropertyAccessorFactory.GetPropertyAccessor("field")
					});
		}

		public object TransformTuple(object[] tuple, String[] aliases)
		{
			object result;

			try
			{
				if (setters == null)
				{
					setters = new ISetter[aliases.Length];
					for (int i = 0; i < aliases.Length; i++)
					{
						string alias = aliases[i];
						if (alias != null)
						{
							setters[i] = propertyAccessor.GetSetter(resultClass, alias);
						}
					}
				}
				result = Activator.CreateInstance(resultClass, true);

				for (int i = 0; i < aliases.Length; i++)
				{
					if (setters[i] != null)
					{
						setters[i].Set(result, tuple[i]);
					}
				}
			}
			catch (InstantiationException e)
			{
				throw new HibernateException("Could not instantiate result class: " + resultClass.FullName, e);
			}
			catch (MethodAccessException e)
			{
				throw new HibernateException("Could not instantiate result class: " + resultClass.FullName, e);
			}

			return result;
		}

		public IList TransformList(IList collection)
		{
			return collection;
		}
	}
}