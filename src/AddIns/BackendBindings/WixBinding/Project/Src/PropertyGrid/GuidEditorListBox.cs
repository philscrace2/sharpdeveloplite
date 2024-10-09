﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A list box which has one entry called "New Guid".
	/// </summary>
	public class GuidEditorListBox : ListBox
	{
		string guid = String.Empty;
		IWindowsFormsEditorService editorService;
		
		public GuidEditorListBox(IWindowsFormsEditorService editorService)
		{
			this.editorService = editorService;
			Items.Add("New Guid");
			Size = new Size(Width, ItemHeight);
			BorderStyle = BorderStyle.None;
		}
		
		public string Guid {
			get {
				return guid;
			}
			set {
				guid = value;
			}
		}
		
		protected override bool ShowFocusCues {
			get {
				return false;
			}
		}
		
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			int index = IndexFromPoint(e.Location);
			if (index != -1) {
				CreateNewGuid();
				editorService.CloseDropDown();
			}
		}
		
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.KeyData == Keys.Return) {
				if (SelectedIndex != -1) {
					CreateNewGuid();
				}
				editorService.CloseDropDown();
			}
		}
		
		void CreateNewGuid()
		{
			guid = System.Guid.NewGuid().ToString().ToUpperInvariant();
		}
	}
}
