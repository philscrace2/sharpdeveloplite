﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Base class for all Wix tree nodes.
	/// </summary>
	public class WixTreeNode : ExtTreeNode, IOwnerState
	{
		XmlElement element;
		ExtTreeNode dummyChildNode = null;
		
		public WixTreeNode(XmlElement element)
		{
			this.element = element;
			sortOrder = 10;
			
			if (element.HasChildNodes) {
				dummyChildNode = new ExtTreeNode();
				dummyChildNode.AddTo(this);
			}
		}
		
		public Enum InternalState {
			get {
				WixPackageFilesTreeView treeView = WixPackageFilesTreeView;
				if (treeView != null) {
					return treeView.InternalState;
				}
				return WixPackageFilesTreeView.WixPackageFilesTreeViewState.None;
			}
		}
		
		/// <summary>
		/// Gets whether this tree node has been initialized. If it has been
		/// initialized then all the child nodes have been added to this node.
		/// </summary>
		public bool IsInitialized {
			get {
				return isInitialized;
			}
		}
		
		/// <summary>
		/// Can delete all Wix tree nodes.
		/// </summary>
		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			RemoveElementCommand command = new RemoveElementCommand();
			command.Run();
		}
		
		/// <summary>
		/// Gets the XmlElement associated with this tree node.
		/// </summary>
		public XmlElement XmlElement {
			get {
				return element;
			}
		}
		
		public WixPackageFilesTreeView WixPackageFilesTreeView {
			get {
				return (WixPackageFilesTreeView)TreeView;
			}
		}
		
		/// <summary>
		/// Adds child nodes to this tree node.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			
			if (dummyChildNode != null) {
				Nodes.Remove(dummyChildNode);
				dummyChildNode = null;
			}
			
			WixTreeNodeBuilder.AddNodes(this, element.ChildNodes);
		}
	}
}
