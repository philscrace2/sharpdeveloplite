// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageNamespaceTreeNode : CodeCoverageMethodsTreeNode
	{
		string namespacePrefix = String.Empty;
				
		public CodeCoverageNamespaceTreeNode(string name, List<CodeCoverageMethod> methods) : this(String.Empty, name, methods)
		{
		}
		
		public CodeCoverageNamespaceTreeNode(string namespacePrefix, string name, List<CodeCoverageMethod> methods) : base(name, methods, CodeCoverageImageListIndex.Namespace)
		{
			sortOrder = 1;
			this.namespacePrefix = namespacePrefix;
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();
			
			// Add namespace nodes.
			string fullNamespace = CodeCoverageMethod.GetFullNamespace(namespacePrefix, Name);
			foreach (string namespaceName in CodeCoverageMethod.GetChildNamespaces(Methods, fullNamespace)) {
				string childFullNamespace = CodeCoverageMethod.GetFullNamespace(fullNamespace, namespaceName);
				CodeCoverageNamespaceTreeNode node = new CodeCoverageNamespaceTreeNode(fullNamespace, namespaceName, CodeCoverageMethod.GetAllMethods(Methods, childFullNamespace));
				node.AddTo(this);
			}
			
			// Add class nodes for this namespace.
			foreach (string className in CodeCoverageMethod.GetClassNames(Methods, fullNamespace)) {
				CodeCoverageClassTreeNode classNode = new CodeCoverageClassTreeNode(className, CodeCoverageMethod.GetMethods(Methods, fullNamespace, className));
				classNode.AddTo(this);
			}
			
			// Sort nodes added.
			SortChildNodes();
		}
	}
}
