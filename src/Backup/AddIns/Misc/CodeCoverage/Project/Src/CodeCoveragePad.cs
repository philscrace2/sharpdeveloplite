// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoveragePad : AbstractPadContent
	{
		static CodeCoveragePad instance;
		bool disposed;
		CodeCoverageControl codeCoverageControl;
		
		public CodeCoveragePad()
		{
			instance = this;
			
			codeCoverageControl = new CodeCoverageControl();
			codeCoverageControl.UpdateToolbar();
					
			ProjectService.SolutionClosed += SolutionClosed;
			ProjectService.SolutionLoaded += SolutionLoaded;
			
			ShowSourceCodePanel = CodeCoverageOptions.ShowSourceCodePanel;
			ShowVisitCountPanel = CodeCoverageOptions.ShowVisitCountPanel;
		}
		
		public static CodeCoveragePad Instance {
			get {
				return instance;
			}
		}

		public override Control Control {
			get {
				return codeCoverageControl;
			}
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			if (!disposed) {
				disposed = true;
				ProjectService.SolutionClosed -= SolutionClosed;
				ProjectService.SolutionLoaded -= SolutionLoaded;
				codeCoverageControl.Dispose();
			}
		}
		
		public void UpdateToolbar()
		{
			codeCoverageControl.UpdateToolbar();
		}
		
		public void ShowResults(CodeCoverageResults results)
		{
			if (results != null) {
				codeCoverageControl.AddModules(results.Modules);
			}
		}
		
		public void ClearCodeCoverageResults()
		{
			codeCoverageControl.Clear();
		}
		
		public bool ShowSourceCodePanel {
			get {
				return codeCoverageControl.ShowSourceCodePanel;
			}
			set {
				codeCoverageControl.ShowSourceCodePanel = value;
			}
		}
		
		public bool ShowVisitCountPanel {
			get {
				return codeCoverageControl.ShowVisitCountPanel;
			}
			set {
				codeCoverageControl.ShowVisitCountPanel = value;
			}
		}
		
		void SolutionLoaded(object sender, EventArgs e)
		{
			codeCoverageControl.UpdateToolbar();
		}
		
		void SolutionClosed(object sender, EventArgs e)
		{
			ClearCodeCoverageResults();
			codeCoverageControl.UpdateToolbar();
			ClearCodeCoverageResults();
		}
	}
}
