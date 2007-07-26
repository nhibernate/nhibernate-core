using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NHibernate.Tool.HbmXsd
{
	public class ImproveTypeNamesCommand
	{
		private readonly CodeNamespace code;
		private readonly Dictionary<string, string> typeNameChanges = new Dictionary<string, string>();

		public ImproveTypeNamesCommand(CodeNamespace code)
		{
			this.code = code;
		}

		public void Execute()
		{
			ChangeTypeNames();
			UpdateTypeReferences();
		}

		protected virtual string CustomizeBetterTypeName(string name)
		{
			return name;
		}

		protected virtual string HandleSpecialCaseTypeNames(string originalName)
		{
			return null;
		}

		protected static string CamelCase(string name)
		{
			string[] parts = name.Split('-');

			for (int i = 0; i < parts.Length; i++)
				parts[i] = parts[i].Substring(0, 1).ToUpper() + parts[i].Substring(1);

			return string.Join("", parts);
		}

		private void ChangeTypeNames()
		{
			foreach (CodeTypeDeclaration type in code.Types)
			{
				string rootElementName = GetRootElementName(type);
				string originalName = type.Name;

				string betterTypeName = HandleSpecialCaseTypeNames(originalName)
					?? GetBetterTypeName(originalName, rootElementName);

				type.Name = CustomizeBetterTypeName(betterTypeName);
				typeNameChanges[originalName] = type.Name;
			}
		}

		private void UpdateTypeReferences()
		{
			foreach (CodeTypeDeclaration type in code.Types)
				foreach (CodeTypeMember member in type.Members)
				{
					CodeMemberField field = member as CodeMemberField;
					CodeConstructor constructor = member as CodeConstructor;

					if (field != null)
						UpdateFieldTypeReferences(field);
					else if (constructor != null)
						UpdateConstructorTypeReferences(constructor);
				}
		}

		private void UpdateConstructorTypeReferences(CodeConstructor constructor)
		{
			foreach (CodeStatement statement in constructor.Statements)
			{
				CodeAssignStatement assignment = (CodeAssignStatement) statement;

				CodeFieldReferenceExpression right = assignment.Right as CodeFieldReferenceExpression;

				if (right != null)
					UpdateTypeReference(((CodeTypeReferenceExpression) right.TargetObject).Type);
			}
		}

		private void UpdateFieldTypeReferences(CodeMemberField field)
		{
			if (field.Type.ArrayElementType != null)
				UpdateTypeReference(field.Type.ArrayElementType);
			else
				UpdateTypeReference(field.Type);

			foreach (CodeAttributeDeclaration attribute in field.CustomAttributes)
				if (attribute.Name == typeof (XmlElementAttribute).FullName)
				{
					if (attribute.Arguments.Count == 2)
						UpdateTypeReference(((CodeTypeOfExpression) attribute.Arguments[1].Value).Type);
				}
				else if (attribute.Name == typeof (DefaultValueAttribute).FullName)
				{
					CodeFieldReferenceExpression reference = attribute.Arguments[0].Value as CodeFieldReferenceExpression;

					if (reference != null)
						UpdateTypeReference(((CodeTypeReferenceExpression) reference.TargetObject).Type);
				}
		}

		private void UpdateTypeReference(CodeTypeReference type)
		{
			if (typeNameChanges.ContainsKey(type.BaseType))
				type.BaseType = typeNameChanges[type.BaseType];
		}

		private static string GetRootElementName(CodeTypeMember type)
		{
			foreach (CodeAttributeDeclaration attribute in type.CustomAttributes)
				if (attribute.Name == typeof (XmlRootAttribute).FullName)
					foreach (CodeAttributeArgument argument in attribute.Arguments)
						if (argument.Name == "")
							return ((CodePrimitiveExpression) argument.Value).Value.ToString();

			return null;
		}

		private static string GetBetterTypeName(string originalName, string rootElementName)
		{
			if (rootElementName == null)
				return CamelCase(originalName);

			bool rootMatchesMatchesOriginal =
				rootElementName.Replace("-", "").Equals(originalName, StringComparison.InvariantCultureIgnoreCase);

			if (!rootMatchesMatchesOriginal)
				throw new InvalidOperationException(
					string.Format("Unhandled special case: '{0}' and '{1}' do not match.", originalName, rootElementName));

			return CamelCase(rootElementName);
		}
	}
}