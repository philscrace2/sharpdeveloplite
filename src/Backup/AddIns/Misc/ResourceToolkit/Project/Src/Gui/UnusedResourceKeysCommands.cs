// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace Hornung.ResourceToolkit.Gui
{
	/// <summary>
	/// Hides or shows the ICSharpCode.Core host resources in an UnusedResourceKeysViewContent.
	/// </summary>
	public class UnusedResourceKeysHideICSharpCodeCoreHostResourcesCommand : AbstractCheckableMenuCommand, IFilter<ResourceItem>
	{
		string icSharpCodeCoreHostResourceFileName;
		
		public override void Run()
		{
			base.Run();
			
			IFilterHost<ResourceItem> host = ((ToolBarCheckBox)this.Owner).Caller as IFilterHost<ResourceItem>;
			if (host != null) {
				if (this.IsChecked) {
					this.icSharpCodeCoreHostResourceFileName = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceSet(null).FileName;
					host.RegisterFilter(this);
				} else {
					host.UnregisterFilter(this);
				}
			}
		}
		
		/// <summary>
		/// Determines if the specified item matches the current filter criteria.
		/// </summary>
		/// <param name="item">The item to test.</param>
		/// <returns><c>true</c>, if the specified item matches the current filter criteria, otherwise <c>false</c>.</returns>
		public bool IsMatch(ResourceItem item)
		{
			return !FileUtility.IsEqualFileName(item.FileName, this.icSharpCodeCoreHostResourceFileName);
		}
	}
}
