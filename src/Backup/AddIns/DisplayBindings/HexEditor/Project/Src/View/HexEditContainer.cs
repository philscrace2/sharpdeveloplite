// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using HexEditor.Util;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;

namespace HexEditor.View
{
	public partial class HexEditContainer : UserControl
	{
		public bool HasSomethingSelected {
			get { return hexEditControl.HasSomethingSelected; }
		}
		
		public bool EditorFocused {
			get { return hexEditControl.Focused; }
		}
		
		public bool CanUndo {
			get { return hexEditControl.CanUndo; }
		}
		
		public bool CanRedo {
			get { return hexEditControl.CanRedo; }
		}
		
		public HexEditContainer()
		{
			InitializeComponent();
			
			tbSizeToFit.Text = StringParser.Parse(tbSizeToFit.Text);
			
			ToolStripControlHost bytesPerLine = new ToolStripControlHost(tSTBCharsPerLine);
			this.toolStrip1.Items.Insert(1, bytesPerLine);
			
			ToolStripControlHost viewMode = new ToolStripControlHost(tCBViewMode);
			this.toolStrip1.Items.Insert(3, viewMode);
			
			tSTBCharsPerLine.Text = hexEditControl.BytesPerLine.ToString();
			this.hexEditControl.ContextMenuStrip = MenuService.CreateContextMenu(this.hexEditControl, "/AddIns/HexEditor/Editor/ContextMenu");
			tCBViewMode.SelectedIndex = 0;
						
			tSTBCharsPerLine.Value = Settings.BytesPerLine;
			tCBViewMode.SelectedItem = Settings.ViewMode.ToString();
			hexEditControl.ViewMode = Settings.ViewMode;
			hexEditControl.BytesPerLine = Settings.BytesPerLine;
			tbSizeToFit.Checked = hexEditControl.FitToWindowWidth = Settings.FitToWidth;
			tSTBCharsPerLine.Enabled = !Settings.FitToWidth;
		}

		void TbSizeToFitClick(object sender, EventArgs e)
		{
			tSTBCharsPerLine.Enabled = !tbSizeToFit.Checked;
			hexEditControl.FitToWindowWidth = tbSizeToFit.Checked;
			tSTBCharsPerLine.Text = hexEditControl.BytesPerLine.ToString();
		}

		void TCBViewModeSelectedIndexChanged(object sender, EventArgs e)
		{
			switch (tCBViewMode.SelectedIndex)
			{
				case 0:
					hexEditControl.ViewMode = ViewMode.Hexadecimal;
					break;
				case 1:
					hexEditControl.ViewMode = ViewMode.Octal;
					break;
				case 2:
					hexEditControl.ViewMode = ViewMode.Decimal;
					break;
			}
		}

		void HexEditContainerGotFocus(object sender, EventArgs e)
		{
			hexEditControl.Focus();
		}
		
		public void LoadFile(OpenedFile file, Stream stream)
		{
			hexEditControl.LoadFile(file, stream);
		}
		
		public void SaveFile(OpenedFile file, Stream stream)
		{
			hexEditControl.SaveFile(file, stream);
		}
		
		public string Cut()
		{
			string text = hexEditControl.Copy();
			hexEditControl.Delete();
			return text;
		}
		
		public string Copy()
		{
			return hexEditControl.Copy();
		}
		
		public void Paste(string text)
		{
			hexEditControl.Paste(text);
		}
		
		public void Delete()
		{
			hexEditControl.Delete();
		}
		
		public void SelectAll()
		{
			hexEditControl.SelectAll();
		}
		
		public void Undo()
		{
			hexEditControl.Undo();
		}
		
		public void Redo()
		{
			hexEditControl.Redo();
		}

		void tSTBCharsPerLine_TextChanged(object sender, EventArgs e)
		{
			int value = 0;
			if (int.TryParse(tSTBCharsPerLine.Text, out value)) {
				hexEditControl.BytesPerLine = value;
				tSTBCharsPerLine.Text = hexEditControl.BytesPerLine.ToString();
			}
		}

		void HexEditContainer_Resize(object sender, EventArgs e)
		{
			if (this.tbSizeToFit.Checked) tSTBCharsPerLine.Text = hexEditControl.BytesPerLine.ToString();
		}
	}
}
