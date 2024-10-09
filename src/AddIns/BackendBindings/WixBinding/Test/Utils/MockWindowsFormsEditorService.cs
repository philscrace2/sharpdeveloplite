﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport;
using ICSharpCode.WixBinding;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WixBinding.Tests.Utils
{
	public class MockWindowsFormsEditorService : IWindowsFormsEditorService
	{
		Type controlType;
		string newValue;
		bool newValueSet;
		
		public MockWindowsFormsEditorService()
		{
		}
		
		public void CloseDropDown()
		{
		}
		
		public void DropDownControl(Control control)
		{
			controlType = control.GetType();
			
			if (newValueSet) {
				GuidEditorListBox guidListBox = control as GuidEditorListBox;
				DropDownEditorListBox dropDownListBox = control as DropDownEditorListBox;
				if (guidListBox != null) {
					guidListBox.Guid = newValue;
				} else if (dropDownListBox != null) {
					dropDownListBox.Value = newValue;
				}
			}
		}
		
		public DialogResult ShowDialog(Form dialog)
		{
			return DialogResult.None;
		}
		
		public Type GetDropDownControlTypeUsed()
		{
			return controlType;
		}
		
		public void SetNewValue(string value)
		{
			newValue = value;
			newValueSet = true;
		}
	}
}
