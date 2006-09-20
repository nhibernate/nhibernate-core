using System;
using System.Collections;
using NHibernate.Property;

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
			propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor(null);
			// TODO H3:
			//= new ChainedPropertyAccessor(
			//    new IPropertyAccessor[]
			//    {
			//        PropertyAccessorFactory.GetPropertyAccessor(resultClass, null),
			//        PropertyAccessorFactory.GetPropertyAccessor("field")
			//    });
		}

		public Object TransformTuple(Object[] tuple, String[] aliases)
		{
			Object result;

			try
			{
				if (setters == null)
				{
					setters = new ISetter[aliases.Length];
					for (int i = 0; i < aliases.Length; i++)
					{
						String alias = aliases[i];
						if (alias != null)
						{
							setters[i] = propertyAccessor.GetSetter(resultClass, alias);
						}
					}
				}
				result = Activator.CreateInstance(resultClass);

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
