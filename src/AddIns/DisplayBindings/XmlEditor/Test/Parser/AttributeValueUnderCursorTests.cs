﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.Parser
{
	[TestFixture]
	public class AttributeValueUnderCursorTests
	{
		[Test]
		public void SuccessTest1()
		{
			string text = "<a foo='abc'";
			Assert.AreEqual("abc", XmlParser.GetAttributeValueAtIndex(text, text.Length - 1));
		}
		
		[Test]
		public void SuccessTest2()
		{
			string text = "<a foo=\"abc\"";
			Assert.AreEqual("abc", XmlParser.GetAttributeValueAtIndex(text, text.Length - 1));
		}
		
		[Test]
		public void SuccessTest3()
		{
			string text = "<a foo='abc'";
			Assert.AreEqual("abc", XmlParser.GetAttributeValueAtIndex(text, text.Length - 2));
		}
		
		[Test]
		public void SuccessTest4()
		{
			string text = "<a foo='abc'";
			Assert.AreEqual("abc", XmlParser.GetAttributeValueAtIndex(text, text.IndexOf("abc")));
		}
		
		[Test]
		public void SuccessTest5()
		{
			string text = "<a foo=''";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeValueAtIndex(text, text.Length - 1));
		}
		
		[Test]
		public void SuccessTest6()
		{
			string text = "<a foo='a'";
			Assert.AreEqual("a", XmlParser.GetAttributeValueAtIndex(text, text.Length - 1));
		}
		
		[Test]
		public void SuccessTest7()
		{
			string text = "<a foo='a\"b\"c'";
			Assert.AreEqual("a\"b\"c", XmlParser.GetAttributeValueAtIndex(text, text.Length - 1));
		}
		
		[Test]
		public void FailureTest1()
		{
			string text = "<a foo='a";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeValueAtIndex(text, text.Length - 1));
		}

		[Test]
		public void MarkupExtensionValueTest()
		{
			string xaml = "<Test val1=\"{Binding Value}\" />";
			int offset = "<Test val1=\"{Bin".Length;
					
			Assert.AreEqual("{Binding Value}", XmlParser.GetAttributeValueAtIndex(xaml, offset));
		}
		
		[Test]
		public void LeftCurlyBracketIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar('{'));
		}
		
		[Test]
		public void RightCurlyBracketIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar('}'));
		}
		
		[Test]
		public void SpaceCharIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar(' '));
		}
		
		[Test]
		public void ColonCharIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar(':'));
		}
		
		[Test]
		public void ForwardSlashCharIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar('/'));
		}
		
		[Test]
		public void UnderscoreCharIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar('_'));
		}
		
		[Test]
		public void DotCharIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar('.'));
		}

		[Test]
		public void DashCharIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar('-'));
		}
		
		[Test]
		public void HashCharIsValidAttributeValueChar()
		{
			Assert.IsTrue(XmlParser.IsAttributeValueChar('#'));
		}
		
		[Test]
		public void LeftAngleBracketIsNotValidAttributeValueChar()
		{
			Assert.IsFalse(XmlParser.IsAttributeValueChar('<'));
		}
		
		[Test]
		public void RightAngleBracketIsNotValidAttributeValueChar()
		{
			Assert.IsFalse(XmlParser.IsAttributeValueChar('>'));
		}
	}
}
