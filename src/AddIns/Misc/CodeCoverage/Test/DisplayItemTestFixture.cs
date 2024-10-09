// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Drawing;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class DisplayItemTestFixture
	{
		CodeCoverageDisplayItem displayItem;
		string itemName = "Code Covered";
		string backColorPropertyName = "BackColor";
		Color backColor = Color.Lime;
		string foreColorPropertyName = "ForeColor";
		Color foreColor = Color.Blue;
		
		[SetUp]
		public void Init()
		{
			displayItem = new CodeCoverageDisplayItem(itemName, backColorPropertyName, backColor, foreColorPropertyName, foreColor);
		}
		
		[Test]
		public void DisplayItemToString()
		{
			Assert.AreEqual(itemName, displayItem.ToString());
		}
		
		[Test]
		public void HasChanged()
		{
			Assert.IsFalse(displayItem.HasChanged);
		}
		
		[Test]
		public void BackColor()
		{
			Assert.AreEqual(backColor, displayItem.BackColor);
		}
		
		[Test]
		public void BackColorPropertyName()
		{
			Assert.AreEqual(backColorPropertyName, displayItem.BackColorPropertyName);
		}
		
		[Test]
		public void ForeColor()
		{
			Assert.AreEqual(foreColor, displayItem.ForeColor);
		}
		
		[Test]
		public void ForeColorPropertyName()
		{
			Assert.AreEqual(foreColorPropertyName, displayItem.ForeColorPropertyName);
		}
		
		[Test]
		public void ChangeBackColor()
		{
			displayItem.BackColor = Color.Red;
			Assert.IsTrue(displayItem.HasChanged);
		}
		
		[Test]
		public void ChangeForeColor()
		{
			displayItem.ForeColor = Color.Yellow;
			Assert.IsTrue(displayItem.HasChanged);
		}
	}
}
