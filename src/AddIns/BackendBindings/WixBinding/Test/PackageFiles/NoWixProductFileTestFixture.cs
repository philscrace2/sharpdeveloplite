﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Tests that the package editor will display an error if no Wix file can be found
	/// containing the product information.
	/// </summary>
	[TestFixture]
	public class NoWixProductFileFixture : PackageFilesTestFixtureBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
		}
		
		[Test]
		public void NoDirectoriesAdded()
		{
			Assert.AreEqual(0, view.DirectoriesAdded.Count);
		}
		
		[Test]
		public void NoRootDirectoryFound()
		{
			Assert.IsTrue(view.IsNoRootDirectoryFoundMessageDisplayed);
		}
		
		[Test]
		public void ContextMenuIsDisabled()
		{
			Assert.IsFalse(view.ContextMenuEnabled);
		}
		
		protected override string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment></Fragment>\r\n" +
				"</Wix>";
		}
	}
}
