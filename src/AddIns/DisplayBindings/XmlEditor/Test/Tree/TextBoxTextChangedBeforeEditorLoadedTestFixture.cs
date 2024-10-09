// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Fixes a null exception that could occur if the TextBoxTextChanged event is fired before the
	/// XmlTreeEditor is created.
	/// </summary>
	[TestFixture]
	public class TextBoxTextChangedBeforeEditorLoadedTestFixture
	{
		[Test]
		public void FireTextBoxTextChangedEventBeforeXmlTreeEditorCreated()
		{
			DerivedXmlTreeViewContainerControl treeViewContainer = new DerivedXmlTreeViewContainerControl();
			treeViewContainer.CallTextBoxTextChanged();
		}
	}
}
