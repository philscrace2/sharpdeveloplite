﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace WixBinding.Tests.Diff
{
	/// <summary>
	/// Tests that certain files and directories are ignored if they 
	/// match an entry in the WixPackageFilesDiff.ExcludedFileNames.
	/// </summary>
	[TestFixture]
	public class ExcludedFilesTestFixture : IDirectoryReader
	{
		WixPackageFilesDiffResult[] diffResults;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
			doc.LoadXml(GetWixXml());
			WixPackageFilesDiff diff = new WixPackageFilesDiff(this);
			diff.ExcludedFileNames.Add("*.pdb");
			diffResults = diff.Compare(doc.RootDirectory);
		}
		
		[Test]
		public void OneDiffResultFound()
		{
			Assert.AreEqual(1, diffResults.Length);
		}
		
		[Test]
		public void DiffResultFileName()
		{
			Assert.AreEqual(@"C:\Projects\Setup\bin\newfile.txt", diffResults[0].FileName);
		}
		
		[Test]
		public void DiffResultType()
		{
			Assert.AreEqual(WixPackageFilesDiffResultType.NewFile, diffResults[0].DiffType);
		}

		public string[] GetFiles(string path)
		{
			return new string[] {"license.rtf", "newfile.txt", "myapp.pdb"};
		}
		
		public string[] GetDirectories(string path)
		{
			return new string[0];
		}
		
		public bool DirectoryExists(string path)
		{
			return true;
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='Test' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t<Directory Id='TARGETDIR' SourceName='SourceDir'>\r\n" +
				"\t\t\t<Directory Id='ProgramFilesFolder' Name='PFiles'>\r\n" +
				"\t\t\t\t<Directory Id='INSTALLDIR' Name='YourApp' LongName='Your Application'>\r\n" +
				"\t\t\t\t\t<Component Id='MyComponent' DiskId='1'>\r\n" +
				"\t\t\t\t\t\t<File Id='LicenseFile' Name='license.rtf' Source='bin\\license.rtf' />\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
