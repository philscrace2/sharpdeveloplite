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
/// Throwen when no valid SharpReportCore is available or when SharpReportCore == null
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 05.07.2005 22:32:47
/// </remarks>

	
namespace ICSharpCode.Reports.Core {	
	[Serializable()]
	public class MissingDataSourceException : System.Exception {
		
		string errorMessage = String.Empty;
		
		public MissingDataSourceException():base() {
			
		}
		
		public MissingDataSourceException(string errorMessage):base(errorMessage ){
			this.errorMessage = errorMessage;
		}
		
		public MissingDataSourceException(string errorMessage,
		                            Exception exception):base (errorMessage,exception){
			
		}
		
		protected MissingDataSourceException(SerializationInfo info, 
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
