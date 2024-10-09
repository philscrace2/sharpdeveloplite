﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class SetupDialogListPad : AbstractPadContent
	{
		SetupDialogListView setupDialogListView;
		bool disposed;
		static SetupDialogListPad instance;
		
		public SetupDialogListPad()
		{
			instance = this;
			
			setupDialogListView = new SetupDialogListView();
			setupDialogListView.ContextMenuStrip = MenuService.CreateContextMenu(setupDialogListView, "/SharpDevelop/Pads/WixSetupDialogListPad/ContextMenu");
			setupDialogListView.ItemActivate += SetupDialogListViewItemActivate;
			setupDialogListView.Enter += SetupDialogListViewEnter;
			
			// Show dialogs in currently open wix project.
			ShowDialogList();
			
			ProjectService.CurrentProjectChanged += CurrentProjectChanged;
		}
		
		static public SetupDialogListPad Instance {
			get {
				return instance;
			}
		}
	
		public override Control Control {
			get {
				return setupDialogListView;
			}
		}
		
		public override void Dispose()
		{
			if (!disposed) {
				disposed = true;
				setupDialogListView.Dispose();
				setupDialogListView = null;
				ProjectService.CurrentProjectChanged -= CurrentProjectChanged;
			}
		}
		
		/// <summary>
		/// Opens the selected dialog and displays it in the designer.
		/// </summary>
		public void OpenSelectedDialog()
		{
			SetupDialogListViewItem selectedDialog = SelectedDialog;
			if (selectedDialog != null) {
				SetupDialogErrorListViewItem errorItem = selectedDialog as SetupDialogErrorListViewItem;
				if (errorItem == null) {
					OpenDialog(selectedDialog.FileName, selectedDialog.Id);
				} else {
					FileService.JumpToFilePosition(errorItem.FileName, errorItem.Line, errorItem.Column);
				}
			}
		}
		
		/// <summary>
		/// Gets the selected dialog list view item.
		/// </summary>
		public SetupDialogListViewItem SelectedDialog {
			get {
				if (setupDialogListView.SelectedItems.Count > 0) {
					return (SetupDialogListViewItem)(setupDialogListView.SelectedItems[0]);
				}
				return null;
			}
		}

		/// <summary>
		/// Adds all the dialog ids from all the files in the project to the list view.
		/// </summary>
		/// <remarks>
		/// If an error occurs an error item is added to the list. The error
		/// list is only cleared the first time an error occurs
		/// since there may be multiple errors, one in each Wix file.
		/// Also we do not clear the error list unless we have an error
		/// so any previously added errors from a build, for example, are not 
		/// cleared unless we have to.
		/// </remarks>
		void ShowDialogList()
		{
			// Make sure we do not leave any errors in the error list from a previous call.
			if (setupDialogListView.HasErrors) {
				WixBindingService.ClearErrorList();
			}

			setupDialogListView.Items.Clear();
			
			WixProject openWixProject = ProjectService.CurrentProject as WixProject;
			if (openWixProject != null) {
				bool clearedErrorList = false;
				foreach (FileProjectItem wixFile in openWixProject.WixFiles) {
					if (File.Exists(wixFile.FileName)) {
						try {
							AddDialogListItems(wixFile.FileName);
						} catch (XmlException ex) {
							// Clear the error list the first time only.
							if (!clearedErrorList) {
								clearedErrorList = true;
								WixBindingService.ClearErrorList();
							}
							setupDialogListView.AddError(wixFile.FileName, ex);
							WixBindingService.AddErrorToErrorList(wixFile.FileName, ex);
						}
					}
				}
				if (clearedErrorList) {
					WixBindingService.ShowErrorList();
				}
			}
		}
		
		/// <summary>
		/// Adds dialog ids to the list.
		/// </summary>
		void AddDialogListItems(string fileName)
		{
			WorkbenchTextFileReader workbenchTextFileReader = new WorkbenchTextFileReader();
			using (TextReader reader = workbenchTextFileReader.Create(fileName)) {
				setupDialogListView.AddDialogs(fileName, WixDocument.GetDialogIds(reader));
			}
		}
		
		void CurrentProjectChanged(object source, ProjectEventArgs e)
		{
			ShowDialogList();
		}

		void SetupDialogListViewItemActivate(object source, EventArgs e)
		{
			OpenSelectedDialog();
		}
		
		/// <summary>
		/// When the setup dialog list gets focus update the list of dialogs since
		/// the Wix document may have changed.
		/// </summary>
		void SetupDialogListViewEnter(object source, EventArgs e)
		{
			UpdateDialogList();
		}
		
		/// <summary>
		/// Opens the specified dialog id into the designer.
		/// </summary>
		static void OpenDialog(string fileName, string dialogId)
		{
			// Open the Wix file.
			IViewContent viewContent = FileService.OpenFile(fileName);
			
			// Show the designer.
			WixDialogDesigner designer = WixDialogDesigner.GetDesigner(viewContent);
			if (designer != null) {
				designer.OpenDialog(dialogId);
			} else {
				LoggingService.Debug("Could not open Wix dialog designer for: " + fileName);
			}
		}
		
		/// <summary>
		/// Updates the list if the Wix documents can be read otherwise the list
		/// items are unchanged for that document.
		/// </summary>
		void UpdateDialogList()
		{
			try {
				setupDialogListView.BeginUpdate();
				// TODO: Intelligent updating.
				ShowDialogList();
			} finally {
				setupDialogListView.EndUpdate();
			}
		}
	}
}
