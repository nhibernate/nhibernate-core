using System;
using System.Text;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class ComponentJoin : FromElement
	{
		private readonly string columns;
		private readonly string componentPath;
		private readonly string componentProperty;
		private readonly ComponentType componentType;

		public ComponentJoin(FromClause fromClause, FromElement origin, string alias, string componentPath, ComponentType componentType)
			: base(fromClause, origin, alias)
		{
			this.componentPath = componentPath;
			this.componentType = componentType;
			componentProperty = StringHelper.Unqualify(componentPath);
			fromClause.AddJoinByPathMap(componentPath, this);
			InitializeComponentJoin(new ComponentFromElementType(this));

			string[] cols = origin.GetPropertyMapping("").ToColumns(TableAlias, componentProperty);
			columns = string.Join(", ", cols);
		}

		public string ComponentPath
		{
			get { return componentPath; }
		}

		public ComponentType ComponentType
		{
			get { return componentType; }
		}

		public string ComponentProperty
		{
			get { return componentProperty; }
		}

		public override IType DataType
		{
			get { return ComponentType; }
			set { base.DataType = value; }
		}

		public override string GetIdentityColumn()
		{
			return columns;
		}

		public override string GetDisplayText()
		{
			return "ComponentJoin{path=" + ComponentPath + ", type=" + componentType.ReturnedClass + "}";
		}

		#region Nested type: ComponentFromElementType

		public class ComponentFromElementType : FromElementType
		{
			private readonly ComponentJoin fromElement;
			private readonly IPropertyMapping propertyMapping;

			public ComponentFromElementType(ComponentJoin fromElement)
				: base(fromElement)
			{
				this.fromElement = fromElement;
				propertyMapping = new ComponentPropertyMapping(this);
			}

			public ComponentJoin FromElement
			{
				get { return fromElement; }
			}

			public override IType DataType
			{
				get { return fromElement.ComponentType; }
			}

			public override IQueryableCollection QueryableCollection
			{
				get { return null; }
				set { base.QueryableCollection = value; }
			}

			public override IPropertyMapping GetPropertyMapping(string propertyName)
			{
				return propertyMapping;
			}

			public override IType GetPropertyType(string propertyName, string propertyPath)
			{
				int index = fromElement.ComponentType.GetPropertyIndex(propertyName);
				return fromElement.ComponentType.Subtypes[index];
			}

			public override string RenderScalarIdentifierSelect(int i)
			{
				String[] cols = GetBasePropertyMapping().ToColumns(fromElement.TableAlias, fromElement.ComponentProperty);
				var buf = new StringBuilder();
				// For property references generate <tablealias>.<columnname> as <projectionalias>
				for (int j = 0; j < cols.Length; j++)
				{
					string column = cols[j];
					if (j > 0)
					{
						buf.Append(", ");
					}
					buf.Append(column).Append(" as ").Append(NameGenerator.ScalarName(i, j));
				}
				return buf.ToString();
			}

			protected IPropertyMapping GetBasePropertyMapping()
			{
				return fromElement.Origin.GetPropertyMapping("");
			}

			#region Nested type: ComponentPropertyMapping

			private class ComponentPropertyMapping : IPropertyMapping
			{
				private readonly ComponentFromElementType fromElementType;

				public ComponentPropertyMapping(ComponentFromElementType fromElementType)
				{
					this.fromElementType = fromElementType;
				}

				#region IPropertyMapping Members

				public IType Type
				{
					get { return fromElementType.FromElement.ComponentType; }
				}

				public IType ToType(string propertyName)
				{
					return fromElementType.GetBasePropertyMapping().ToType(GetPropertyPath(propertyName));
				}

				public bool TryToType(string propertyName, out IType type)
				{
					return fromElementType.GetBasePropertyMapping().TryToType(GetPropertyPath(propertyName), out type);
				}

				public string[] ToColumns(string alias, string propertyName)
				{
					return fromElementType.GetBasePropertyMapping().ToColumns(alias, GetPropertyPath(propertyName));
				}

				public string[] ToColumns(string propertyName)
				{
					return fromElementType.GetBasePropertyMapping().ToColumns(GetPropertyPath(propertyName));
				}

				#endregion

				private string GetPropertyPath(string propertyName)
				{
					return fromElementType.FromElement.ComponentPath + '.' + propertyName;
				}
			}

			#endregion
		}

		#endregion
	}
}
