// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

/// <summary>
/// This Class throws the Standart SharpReport Error
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 15.01.2005 09:39:32
/// </remarks>
/// 
namespace ICSharpCode.Reports.Core {
	[Serializable()]
	public class ReportException : Exception {
		
		
		string errorMessage = String.Empty;
		public ReportException():base(){
			
		}
		public ReportException(string errorMessage) :base (errorMessage){
			this.errorMessage = errorMessage;
		}
		public ReportException(string errorMessage,
		                            Exception exception):base (errorMessage,exception){
			
		}
		
		protected ReportException(SerializationInfo info,
		                               StreamingContext context) : base(info, context){
			// Implement type-specific serialization constructor logic.
		}
		
		public string ErrorMessage {
			get {
				return errorMessage;
			}
		}
		
		[SecurityPermissionAttribute(SecurityAction.Demand,
		                             SerializationFormatter = true)]

		public override void GetObjectData(SerializationInfo info, StreamingContext context){
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue("errorMessage", this.errorMessage);
			base.GetObjectData(info, context);
		}
		
	}
}
