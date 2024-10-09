﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TestClassWithOneMethodTestFixture
	{
		TestProject testProject;
		TestClass testClass;
		TestMethod testMethod;
		bool resultChangedCalled;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			resultChangedCalled = false;
			IProject project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);

			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			MockClass mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
			mockClass.Namespace = "RootNamespace.Tests";
			mockClass.ProjectContent = projectContent;
			mockClass.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(mockClass);
			
			// Add a method to the test class
			MockMethod mockMethod = new MockMethod("TestMethod");
			mockMethod.DeclaringType = mockClass;
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			mockClass.Methods.Add(mockMethod);
			
			testProject = new TestProject(project, projectContent);
			testClass = testProject.TestClasses[0];
			testMethod = testClass.TestMethods[0];
		}
		
		[Test]
		public void OneMethod()
		{
			Assert.AreEqual(1, testClass.TestMethods.Count);
		}
		
		[Test]
		public void TestMethodResult()
		{
			Assert.AreEqual(TestResultType.None, testMethod.Result);
		}
		
		[Test]
		public void TestFailed()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod");
			result.IsFailure = true;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Failure, testMethod.Result);
		}
		
		[Test]
		public void TestClassFailed()
		{
			TestFailed();
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
		
		[Test]
		public void TestClassIgnored()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod");
			result.IsIgnored = true;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Ignored, testClass.Result);
		}
		
		[Test]
		public void TestResultChanged()
		{
			try {
				testMethod.ResultChanged += ResultChanged;
				TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod");
				result.IsFailure = true;
				testProject.UpdateTestResult(result);
			} finally {
				testMethod.ResultChanged -= ResultChanged;
			}
			
			Assert.IsTrue(resultChangedCalled);
		}
		
		[Test]
		public void TestResultChangedOnClass()
		{
			try {
				testClass.ResultChanged += ResultChanged;
				TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod");
				result.IsFailure = true;
				testProject.UpdateTestResult(result);
			} finally {
				testClass.ResultChanged -= ResultChanged;
			}
			
			Assert.IsTrue(resultChangedCalled);
		}
		
		[Test]
		public void UpdateProjectWithTestResultHavingClassNameOnly()
		{
			TestResult result = new TestResult("ClassNameOnly");
			result.IsFailure = true;
			
			testProject.UpdateTestResult(result);
		}
		
		[Test]
		public void UpdateProjectWithTestResultWithUnknownClassName()
		{
			TestResult result = new TestResult("RootNamespace.Tests.UnknownClassName.TestMethod");
			result.IsFailure = true;
			
			testProject.UpdateTestResult(result);
		}
		
		[Test]
		public void UpdateProjectWithTestResultWithUnknownMethodName()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.UnknownMethodName");
			result.IsFailure = true;
			
			testProject.UpdateTestResult(result);
		}
		
		[Test]
		public void FindTestMethod()
		{
			Assert.AreSame(testMethod, testClass.TestMethods["TestMethod"]);
		}
		
		[Test]
		public void AddNewClassNodeWhenTestClassPassed()
		{
			testClass.Result = TestResultType.Success;
			TestClassTreeNode node = new TestClassTreeNode(testProject, testClass);
			Assert.AreEqual(TestTreeViewImageListIndex.TestPassed, (TestTreeViewImageListIndex)node.ImageIndex);
		}
		
		[Test]
		public void AddNewMethodNodeWhenTestPassed()
		{
			testMethod.Result = TestResultType.Success;
			TestMethodTreeNode node = new TestMethodTreeNode(testProject, testMethod);
			Assert.AreEqual(TestTreeViewImageListIndex.TestPassed, (TestTreeViewImageListIndex)node.ImageIndex);
		}
		
		/// <summary>
		/// Tests that a method is removed from the TestClass
		/// based on the parse info. Also checks that the test methods are
		/// checked based on the CompoundClass via IClass.GetCompoundClass. 
		/// </summary>
		[Test]
		public void MethodRemovedInParserInfo()
		{
			// Create old compilation unit.
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			oldUnit.Classes.Add(testClass.Class);
			
			// Create new compilation unit.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			newUnit.Classes.Add(testClass.Class);

			// Add a new method to a new compound class.
			MockClass compoundClass = new MockClass("RootNamespace.MyTestFixture");
			compoundClass.ProjectContent = projectContent;
			compoundClass.Attributes.Add(new MockAttribute("TestFixture"));
			MockClass mockClass = (MockClass)testClass.Class;
			mockClass.SetCompoundClass(compoundClass);
			
			// Monitor test methods removed.
			List<TestMethod> methodsRemoved = new List<TestMethod>();
			testClass.TestMethods.TestMethodRemoved += delegate(Object source, TestMethodEventArgs e)
				{ methodsRemoved.Add(e.TestMethod); };

			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.IsFalse(testClass.TestMethods.Contains("TestMethod"));
			Assert.AreEqual(1, methodsRemoved.Count);
			Assert.AreSame(testMethod.Method, methodsRemoved[0].Method);
		}
				
		void ResultChanged(object source, EventArgs e)
		{
			resultChangedCalled = true;
		}
	}
}
