// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
	
	
	/// <summary>
	/// Handles BeforPrint and AfterPrint Events
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 22.11.2004 13:06:44
	/// </remarks>
	
namespace ICSharpCode.Reports.Core {
	
	public class SectionEventArgs : EventArgs
	{
		BaseSection section;
		
		public SectionEventArgs(BaseSection section){
			this.section = section;
		}
		
		public BaseSection Section {
			get {
				return section;
			}
		}
		
	}
		
		
	///<summary>
	/// This event is fired just bevore an Item is printed
	/// Use this event for formatting etc.
	/// </summary>	
	
	public class BeforePrintEventArgs : System.EventArgs {
		public BeforePrintEventArgs (){
				
		}
	}
	
	/// <summary>
	/// This Event is fiered after an Item is printed
	/// </summary>
	public class AfterPrintEventArgs : System.EventArgs {
		PointF locationAfterPrint;
			
		
		public AfterPrintEventArgs(PointF locationAfterPrint){
				
				this.locationAfterPrint = locationAfterPrint;
		}
		
		public PointF LocationAfterPrint {
			get {
				return locationAfterPrint;
			}
		}
		
	}
}
