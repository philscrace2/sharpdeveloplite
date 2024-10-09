// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Dom;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Determines equality of DOM members by region and fully qualified name.
	/// </summary>
	public class MemberEqualityComparer : IEqualityComparer<IMember>
	{
		public bool Equals(IMember x, IMember y)
		{
			if (x == null || y == null) {
				return false;
			}
			if (x.Region != y.Region) {
				return false;
			}
			IComparer<string> nameComparer;
			if (x.DeclaringType != null &&
			    x.DeclaringType.ProjectContent != null &&
			    x.DeclaringType.ProjectContent.Language != null) {
				nameComparer = x.DeclaringType.ProjectContent.Language.NameComparer;
			} else {
				nameComparer = StringComparer.InvariantCulture;
			}
			return nameComparer.Compare(x.FullyQualifiedName, y.FullyQualifiedName) == 0;
		}
		
		public int GetHashCode(IMember obj)
		{
			if (obj == null) {
				return 0;
			}
			return obj.Region.GetHashCode() ^
				obj.FullyQualifiedName.GetHashCode();
		}
	}
}
