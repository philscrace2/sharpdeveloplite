﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using WixBinding;

namespace WixBinding.Tests.DialogXmlGeneration
{
	/// <summary>
	/// Tests that the controls added onto the GroupBox get added to the dialog xml.
	/// </summary>
	[TestFixture]
	public class GroupBoxContainingControlsTestFixture
	{
		XmlElement dialogElement;
		XmlElement textBoxElement;
		XmlElement groupBoxElement;
		XmlElement childGroupBoxElement;
		XmlElement buttonElement;
		XmlElement radioButtonGroupElement;
		XmlElement childButtonElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());

			WixDialog wixDialog = doc.GetDialog("WelcomeDialog");
			using (Form dialog = wixDialog.CreateDialog()) {
				GroupBox groupBox = new GroupBox();
				groupBox.Name = "NewGroupBox";
				groupBox.Top = 10;
				groupBox.Left = 20;
				dialog.Controls.Add(groupBox);
				
				TextBox textBox = new TextBox();
				textBox.Name = "NewTextBox";
				groupBox.Controls.Add(textBox);
				
				Button button = new Button();
				button.Name = "NewButton";
				groupBox.Controls.Add(button);
				
				GroupBox childGroupBox = new GroupBox();
				childGroupBox.Top = 10;
				childGroupBox.Left = 20;
				childGroupBox.Name = "NewChildGroupBox";
				groupBox.Controls.Add(childGroupBox);
				
				RadioButtonGroupBox radioButtonGroupBox = new RadioButtonGroupBox();
				radioButtonGroupBox.Name = "NewRadioButtonGroupBox";
				groupBox.Controls.Add(radioButtonGroupBox);
				
				Button childButton = new Button();
				childButton.Name = "NewChildButton";
				childGroupBox.Controls.Add(childButton);
				
				dialogElement = wixDialog.UpdateDialogElement(dialog);
				groupBoxElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='NewGroupBox']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));				
				textBoxElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='NewTextBox']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				childGroupBoxElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='NewChildGroupBox']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));				
				buttonElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='NewButton']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				radioButtonGroupElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='NewRadioButtonGroupBox']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				childButtonElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='NewChildButton']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
			}
		}
		
		[Test]
		public void TextBoxX()
		{
			int expectedX = Convert.ToInt32(20 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), textBoxElement.GetAttribute("X"));
		}
		
		[Test]
		public void TextBoxY()
		{
			int expectedY = Convert.ToInt32(10 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), textBoxElement.GetAttribute("Y"));			
		}
		   
		[Test]
		public void GroupBoxX()
		{
			int expectedX = Convert.ToInt32(20 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), groupBoxElement.GetAttribute("X"));
		}
		
		[Test]
		public void GroupBoxY()
		{
			int expectedY = Convert.ToInt32(10 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), groupBoxElement.GetAttribute("Y"));			
		}		
		
		[Test]
		public void ButtonX()
		{
			int expectedX = Convert.ToInt32(20 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), buttonElement.GetAttribute("X"));
		}
		
		[Test]
		public void ButtonY()
		{
			int expectedY = Convert.ToInt32(10 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), buttonElement.GetAttribute("Y"));			
		}		
		
		[Test]
		public void ChildGroupBoxX()
		{
			int expectedX = Convert.ToInt32(40 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), childGroupBoxElement.GetAttribute("X"));
		}
		
		[Test]
		public void ChildGroupBoxY()
		{
			int expectedY = Convert.ToInt32(20 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), childGroupBoxElement.GetAttribute("Y"));			
		}	
		
		[Test]
		public void RadioButtonGroupX()
		{
			int expectedX = Convert.ToInt32(20 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), radioButtonGroupElement.GetAttribute("X"));
		}
		
		[Test]
		public void RadioButtonGroupY()
		{
			int expectedY = Convert.ToInt32(10 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), radioButtonGroupElement.GetAttribute("Y"));			
		}	
		
		[Test]
		public void ChildButtonX()
		{
			int expectedX = Convert.ToInt32(40 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), childButtonElement.GetAttribute("X"));
		}
		
		[Test]
		public void ChildButtonY()
		{
			int expectedY = Convert.ToInt32(20 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), childButtonElement.GetAttribute("Y"));			
		}	
			
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370' Title='Welcome Dialog Title'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
