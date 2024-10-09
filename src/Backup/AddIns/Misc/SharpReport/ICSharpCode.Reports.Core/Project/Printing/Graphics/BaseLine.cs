// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.Drawing;
using System.Drawing.Drawing2D;	
/// <summary>
/// This class handle the basic's of drawing all kind of Lines,
/// and we can create a Pen from this properties
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 09.10.2005 18:37:51
/// </remarks>
namespace ICSharpCode.Reports.Core
{
	
	public class BaseLine
	{
		DashStyle dashStyle;
		Color color;
		float thickness;
		private LineCap startLineCap;
		private LineCap endLineCap;
		private DashCap dashLineCap;
		
		#region Constructor
		
		public BaseLine(Color color, DashStyle dashStyle, float thickness):this(color,dashStyle,thickness,
		                                                                        LineCap.Flat,
		                                                                        LineCap.Flat,
		                                                                        DashCap.Flat)
		{
		}
		
		public BaseLine(Color color, DashStyle dashStyle,float thickness, LineCap startLineCap, LineCap endLineCap, DashCap dashLineCap)
		{
			if (color == Color.White) {
				this.color = Color.Black;
			}
			
			this.color = color;
			this.dashStyle = dashStyle;
			this.thickness = thickness;
			this.startLineCap = startLineCap;
			this.endLineCap = endLineCap;
			this.dashLineCap = dashLineCap;
		}
		
		
		#endregion
		
		
		#region Pen
		
		public Pen CreatePen()
		{
//			return this.CreatePen(72f);
			return this.CreatePen(this.thickness);
		}
		
		
		public Pen CreatePen(float resolution)
		{
			Pen pen;
			
			if (resolution == 0f)
			{
//				pen = new Pen(this.color, resolution / 72f);
				pen = new Pen(this.color, 1);
			}
			else
			{
//				pen = new Pen(this.color, (this.thickness * resolution) / 72f);
				pen = new Pen(this.color, resolution);
			}
			pen.DashStyle = this.DashStyle;
			pen.SetLineCap(this.startLineCap,this.endLineCap,this.DashLineCap);
			return pen;
		}

		#endregion

		
		public Color Color {
			get {
				return color;
			}
		}
		
		
		public DashStyle DashStyle {
			get {
				return dashStyle;
			}
		}
		
		
		public float Thickness {
			get {
				return thickness;
			}
		}
		
		
		public LineCap StartLineCap {
			get { return startLineCap; }
		}
		
		
		public LineCap EndLineCap {
			get { return endLineCap; }
		}
		
		
		public DashCap DashLineCap {
			get { return dashLineCap; }
		}
	}
}
