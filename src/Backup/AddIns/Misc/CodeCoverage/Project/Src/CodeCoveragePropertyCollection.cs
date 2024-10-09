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
	public class CodeCoveragePropertyCollection
	{
		List<CodeCoverageProperty> properties = new List<CodeCoverageProperty>();
		
		public CodeCoveragePropertyCollection()
		{
		}
		
		public int Count {
			get { return properties.Count; }
		}
		
		public CodeCoverageProperty this[int index] {
			get { return properties[index]; }
		}
		
		/// <summary>
		/// Adds a getter or setter to the properties collection.
		/// </summary>
		public void Add(CodeCoverageMethod method)
		{
			bool added = false;
			string propertyName = CodeCoverageProperty.GetPropertyName(method);
			foreach (CodeCoverageProperty property in properties) {
				if (propertyName == property.Name) {
					property.AddMethod(method);
					added = true;				
				}
			}
			
			if (!added) {
				properties.Add(new CodeCoverageProperty(method));
			}
		}

		public IEnumerator<CodeCoverageProperty> GetEnumerator()
		{
			return properties.GetEnumerator();
		}
	}
}
