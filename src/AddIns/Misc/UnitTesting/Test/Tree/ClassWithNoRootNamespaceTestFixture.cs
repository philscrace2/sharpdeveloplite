﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	/// <summary>
	/// Adds a class with no root namespace to the test tree.
	/// </summary>
	[TestFixture]
	public class ClassWithNoRootNamespaceTestFixture
	{
		Solution solution;
		ExtTreeNode rootNode;
		TreeNodeCollection nodes;
		DummyParserServiceTestTreeView dummyTreeView;
		TestTreeView treeView;
		MSBuildBasedProject project;
		MockClass testClass;
		ExtTreeNode testFixtureNode;
		TreeNodeCollection rootChildNodes;
		MockProjectContent projectContent;
		
		[SetUp]
		public void SetUp()
		{
			// Create solution.
			solution = new Solution();
			
			// Create a project to display in the test tree view.
			project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			// Add a test class with a TestFixture attributes.
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			testClass = new MockClass("MyTestFixture");
			testClass.Attributes.Add(new MockAttribute("TestFixture"));
			testClass.ProjectContent = projectContent;
			projectContent.Classes.Add(testClass);
						
			// Init mock project content to be returned.
			dummyTreeView = new DummyParserServiceTestTreeView();
			dummyTreeView.ProjectContentForProject = projectContent;
			
			// Load the projects into the test tree view.
			treeView = dummyTreeView as TestTreeView;
			solution.Folders.Add(project);
			treeView.AddSolution(solution);
			nodes = treeView.Nodes;
			rootNode = (ExtTreeNode)treeView.Nodes[0];
			
			// Expand the root node so any child nodes are
			// lazily created.
			rootNode.Expanding();
			rootChildNodes = rootNode.Nodes;
			testFixtureNode = (ExtTreeNode)rootNode.Nodes[0];
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeView != null) {
				treeView.Dispose();
			}
		}
		
		[Test]
		public void OneRootChildNode()
		{
			Assert.AreEqual(1, rootChildNodes.Count);
		}
		
		[Test]
		public void TestFixtureNodeText()
		{
			Assert.AreEqual("MyTestFixture", testFixtureNode.Text);
		}
		
		[Test]
		public void TestFixtureNodeIsTestClassNode()
		{
			Assert.IsInstanceOf(typeof(TestClassTreeNode), testFixtureNode);
		}
		
		/// <summary>
		/// Make sure the test tree view clears the existing nodes
		/// when the solution is added to the tree. Only one
		/// solution can be displayed in the tree.
		/// </summary>
		[Test]
		public void AddSolution()
		{
			treeView.AddSolution(solution);
			
			Assert.AreEqual(1, treeView.Nodes.Count);
		}
		
		[Test]
		public void TestClassFails()
		{
			treeView.SelectedNode = testFixtureNode;
			TestClass testClass = treeView.SelectedTestProject.TestClasses["MyTestFixture"];
			testClass.Result = TestResultType.Failure;
			
			Assert.AreEqual(TestTreeViewImageListIndex.TestFailed, (TestTreeViewImageListIndex)testFixtureNode.ImageIndex);
			Assert.AreEqual(TestTreeViewImageListIndex.TestFailed, (TestTreeViewImageListIndex)rootNode.ImageIndex);
		}
		
		[Test]
		public void AddNewClass()
		{			
			TestProjectTreeNode projectNode = (TestProjectTreeNode)rootNode;
			
			MockClass mockClass = new MockClass("MyNewTestFixture");
			mockClass.Attributes.Add(new MockAttribute("TestFixture"));
			mockClass.ProjectContent = projectContent;
			TestClass newTestClass = new TestClass(mockClass);
			projectNode.TestProject.TestClasses.Add(newTestClass);
			
			ExtTreeNode newTestClassNode = null;
			foreach (ExtTreeNode node in rootNode.Nodes) {
				if (node.Text == "MyNewTestFixture") {
					newTestClassNode = node;
					break;
				}
			}
			newTestClass.Result = TestResultType.Failure;
					
			// New test class node should be added to the root node.
			Assert.IsNotNull(newTestClassNode);
			Assert.AreEqual(2, rootNode.Nodes.Count);
			
			// Make sure the project node image index is affected by the
			// new test class added.
			Assert.AreEqual(TestTreeViewImageListIndex.TestFailed, (TestTreeViewImageListIndex)rootNode.SelectedImageIndex);
		}
		
		/// <summary>
		/// Tests that the test class with no root namespace
		/// is removed from the project tree node.
		/// </summary>
		[Test]
		public void RemoveClass()
		{
			// Locate the class we are going to remove.
			TestProjectTreeNode projectNode = (TestProjectTreeNode)rootNode;
			TestClassTreeNode testClassNode = (TestClassTreeNode)projectNode.Nodes[0];
			testClassNode.Expanding();
			TestClass testClass = projectNode.TestProject.TestClasses["MyTestFixture"];
			projectNode.TestProject.TestClasses.Remove(testClass);
			
			ExtTreeNode testClassNodeAfterRemove = null;
			foreach (ExtTreeNode node in rootNode.Nodes) {
				if (node.Text == "MyTestFixture") {
					testClassNodeAfterRemove = node;
					break;
				}
			}
		
			Assert.IsNull(testClassNodeAfterRemove);
			Assert.AreEqual(0, projectNode.Nodes.Count);
			Assert.IsTrue(testClassNode.IsDisposed);
			
			// Make sure the TestClassTreeNode.Dispose removes all event
			// handlers.
			// It uses the events: 
			// TestClass.ResultChanged
			// TestClassCollection.TestMethodAdded
			// TestClassCollection.TestMethodRemoved
	
			// Make sure the test class result is not a failure already.
			testClass.Result = TestResultType.None;		
			testClass.Result = TestResultType.Failure;
			Assert.AreEqual(TestTreeViewImageListIndex.TestNotRun, 
				(TestTreeViewImageListIndex)testClassNode.ImageIndex,
				"Disposed TestClassTreeNode affected by test class result change.");
		
			// Add a new test method to the test class 
			// and make sure the disposed class node does
			// not add a new child node.
			Assert.AreEqual(0, testClassNode.Nodes.Count);
			MockMethod mockMethod = new MockMethod("Method");
			TestMethod testMethod = new TestMethod(mockMethod);
			testClass.TestMethods.Add(testMethod);
			
			Assert.AreEqual(0, testClassNode.Nodes.Count);
		}
		
		/// <summary>
		/// Tests that the TestProjectTreeNode handles the case where 
		/// some of its children are not class nodes when removing a class.
		/// </summary>
		[Test]
		public void RemoveClassWithProjectNodeHavingNonClassNodeChildren()
		{
			// Locate the class we are going to remove.
			TestProjectTreeNode projectNode = (TestProjectTreeNode)rootNode;
			projectNode.Nodes.Insert(0, new ExtTreeNode());
			TestClass testClass = projectNode.TestProject.TestClasses["MyTestFixture"];
			projectNode.TestProject.TestClasses.Remove(testClass);
			
			ExtTreeNode testClassNode = null;
			foreach (ExtTreeNode node in rootNode.Nodes) {
				if (node.Text == "MyTestFixture") {
					testClassNode = node;
					break;
				}
			}
		
			Assert.IsNull(testClassNode);
			Assert.AreEqual(1, projectNode.Nodes.Count);	
		}
		
		/// <summary>
		/// Tests that the project node and the test fixture node
		/// are both disposed when the project node is disposed.
		/// </summary>
		[Test]
		public void DisposeProjectNode()
		{
			rootNode.Dispose();
			rootNode.Remove();
			
			Assert.IsTrue(rootNode.IsDisposed);
			Assert.IsTrue(testFixtureNode.IsDisposed);
		}
		
		/// <summary>
		/// Tests that the TestProjectTreeNode.TestClassCollection.TestClassRemoved
		/// event handler is removed when the TestProjectTreeNode
		/// is disposed.
		/// </summary>
		[Test]
		public void ChangeTestClassResultAfterDisposingProjectNode()
		{
			TestProjectTreeNode projectNode = (TestProjectTreeNode)rootNode;
			projectNode.Dispose();
			
			// Make sure the project node image index is  
			// unaffected when the test class result is changed.
			Assert.AreEqual(TestTreeViewImageListIndex.TestNotRun, (TestTreeViewImageListIndex)projectNode.ImageIndex);
			TestClass testClass = projectNode.TestProject.TestClasses["MyTestFixture"];
			testClass.Result = TestResultType.Failure;
			Assert.AreEqual(TestTreeViewImageListIndex.TestNotRun, (TestTreeViewImageListIndex)projectNode.ImageIndex);
		}
		
		/// <summary>
		/// Tests that the TestProjectTreeNode.TestClassCollection.TestClassRemoved
		/// event handler is removed when the TestProjectTreeNode
		/// is disposed.
		/// </summary>
		[Test]
		public void AddTestClassResultAfterDisposingTestsProjectNode()
		{
			TestProjectTreeNode projectNode = (TestProjectTreeNode)rootNode;
			projectNode.Dispose();
			
			// Make sure the project node child nodes are  
			// unaffected when the test class is removed.
			Assert.AreEqual(1, projectNode.Nodes.Count);
			TestClass testClass = projectNode.TestProject.TestClasses["MyTestFixture"];
			projectNode.TestProject.TestClasses.Remove(testClass);
			Assert.AreEqual(1, projectNode.Nodes.Count);
		}
		
		/// <summary>
		/// Tests that the TestProjectTreeNode.TestClassCollection.TestClassAdded
		/// event handler is removed when the TestProjectTreeNode
		/// is disposed.
		/// </summary>
		[Test]
		public void RemoveTestClassResultAfterDisposingTestsProjectNode()
		{
			TestProjectTreeNode projectNode = (TestProjectTreeNode)rootNode;
			projectNode.Dispose();
			
			// Make sure the project node child nodes are  
			// unaffected when the test class is removed.
			Assert.AreEqual(1, projectNode.Nodes.Count);
			MockClass mockClass = new MockClass("MyNewTestClass");
			TestClass testClass = new TestClass(mockClass);
			projectNode.TestProject.TestClasses.Add(testClass);
			Assert.AreEqual(1, projectNode.Nodes.Count);
		}
	}
}
