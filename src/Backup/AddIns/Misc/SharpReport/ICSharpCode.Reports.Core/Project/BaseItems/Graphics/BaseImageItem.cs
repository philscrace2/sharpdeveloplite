// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

/// <summary>
/// Handles the drawing of Images like Bitmap's and so on
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 04.10.2005 11:17:29
/// </remarks>
using System;
using System.Drawing;
using System.Globalization;
using System.IO;

using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core {
	
	public class BaseImageItem : BaseGraphicItem,IDataRenderer ,IExportColumnBuilder{
		
		/// <summary>
		/// Default constructor 
		/// </summary>
		
		private string imageFileName;
		private Image image;
		private bool scaleImageToSize;
		private GlobalEnums.ImageSource imageSource;
		private string columnName;
		private string baseTableName;
		private string reportFileName;
		private string relativeFileName;
		
		public BaseImageItem():base() {
		}
		
		#region IExportColumnBuilder  implementation
		
		public BaseExportColumn CreateExportColumn()
		{
			BaseStyleDecorator st = this.CreateItemStyle();
			ExportImage item = new ExportImage(st);
			
			if (this.Image == null) {
				item.FileName = this.imageFileName;
				this.Image = this.LoadImage();
			}
			
			item.Image = this.Image;
			item.ScaleImageToSize = this.scaleImageToSize;
			return item;
		}
		
		
		private BaseStyleDecorator CreateItemStyle ()
		{
			BaseStyleDecorator style = new BaseStyleDecorator();
			style.BackColor = this.BackColor;
			style.ForeColor = this.ForeColor;
			style.Location = this.Location;
			style.Size = this.Size;
			style.DrawBorder = this.DrawBorder;
			return style;
		}
		#endregion
		
		
		private Image LoadImage ()
		{
			try {
				Image im;
				string absFileName = this.AbsoluteFileName;
				System.Diagnostics.Trace.WriteLine(String.Format("<CORE.LoadImage > {0}",absFileName));
				if (!String.IsNullOrEmpty(absFileName) && File.Exists(absFileName)) {
					im = Image.FromFile (this.AbsoluteFileName);
				} else {
					im = BaseImageItem.ErrorBitmap(base.Size);
				}
				if (im == null) {
					string str = String.Format(CultureInfo.InvariantCulture,
					                           "Unable to Load {0}",imageFileName);
					throw new ReportException(str);
				}
				return im;
			} catch (System.OutOfMemoryException) {
				throw;
			} catch (System.IO.FileNotFoundException) {
				throw;
			}
		}
		
		/// <summary>
		/// ToolboxIcon for ReportRectangle
		/// </summary>
		/// <returns>Bitmap</returns>
		
		private static Bitmap ErrorBitmap(Size size)
		{
			Bitmap b = new Bitmap (size.Width,size.Height);
			using (Graphics g = Graphics.FromImage (b)){
				g.DrawRectangle (new Pen(Color.Black, 1),
				                 1,1,size.Width -2,size.Height -2);
				g.DrawString("Image",new Font("Microsoft Sans Serif",
				                              8),
				             new SolidBrush(Color.Gray),
				             new RectangleF(1,1,size.Width,size.Height) );
				
			}
			return b;
		}
	
		#region overrides
		
		public override void Render(ReportPageEventArgs rpea) 
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			base.Render(rpea);
			Graphics g = rpea.PrintPageEventArgs.Graphics;
			
			if (this.Image == null){
				this.Image = ErrorBitmap(new Size (base.DrawingRectangle.Width,
				                                   base.DrawingRectangle.Height) );
			}
			
			if (this.scaleImageToSize) {
				g.DrawImageUnscaled(this.Image,this.Location.X,this.Location.Y);
				rpea.LocationAfterDraw = new Point (this.Location.X + this.Image.Width,
				                                    this.Location.Y + this.Image.Height);
				
			} else {
				RectangleF rect =  base.DrawingRectangle;
				g.DrawImage(this.Image,rect);
				rpea.LocationAfterDraw = new Point (this.Location.X + (int)rect.Width,
				                                    this.Location.Y + (int)rect.Height);
			}
		}
		
		#endregion
		
		
		#region properties
		
		public string ColumnName
		{
			get { return columnName; }
			set {
				columnName = value;
				this.imageSource = GlobalEnums.ImageSource.Database;
			}
		}
		
		
		public string BaseTableName 
		{
			get { return baseTableName; }
			set {
				baseTableName = value;
				this.imageSource = GlobalEnums.ImageSource.Database;
			}
		}

		
		public string MappingName
		{
			get {
				return baseTableName + "." + columnName;
			} 
		}
		
		
		public virtual string ImageFileName 
		{
			get {
				return imageFileName;
			}
			set {
				imageFileName = value;
				this.imageSource = GlobalEnums.ImageSource.File;
				System.Diagnostics.Trace.WriteLine(String.Format("<CORE.BaseImage> ImageFilename {0}",Path.GetFullPath(this.imageFileName)));
			}
		}
		
		
		
		public string AbsoluteFileName
		{
			get {
				if (!string.IsNullOrEmpty(relativeFileName)) {
					string testFileName = FileUtility.NormalizePath(Path.Combine(Path.GetDirectoryName(this.reportFileName),this.relativeFileName));
						
					if (File.Exists(testFileName)){
						Console.WriteLine("Image found with Relative Filename");
						Console.WriteLine("Report Filename {0}",this.reportFileName);
						Console.WriteLine("Relative Filename {0}",this.relativeFileName);
						Console.WriteLine("Image Filename {0}",this.ImageFileName);
						return testFileName;
					
					} else {
						Console.WriteLine("AbsoluteFileName can't load image");
						Console.WriteLine("Report Filename {0}",this.reportFileName);
						Console.WriteLine("Relative Filename {0}",this.relativeFileName);
						Console.WriteLine("Image Filename {0}",this.ImageFileName);
					}
				}
			
				return this.ImageFileName;
			}
		}
		
		
		public string RelativeFileName 
		{
			get { return relativeFileName; }
			set { relativeFileName = value; }
		}
		
		

		public string ReportFileName 
		{
			get { return reportFileName; }
			set { 
				reportFileName = value; 
				System.Diagnostics.Trace.WriteLine("");
				System.Diagnostics.Trace.WriteLine(String.Format("<CORE.BaseImage> ReportFilename {0}",Path.GetFullPath(this.reportFileName)));
			}
		}
		
		
		/// <summary>
		/// The Image
		/// </summary>
		
//		[XmlIgnoreAttribute]
		public  virtual Image Image
		{
			get {
				if (this.image != null) {
					return image;
				} else {
					if (!String.IsNullOrEmpty(imageFileName)) {
						this.image = this.LoadImage();
						return this.image;
					}
				}
				return null;
			}
			set {
				this.imageFileName = String.Empty;
				this.image = value;
				this.imageSource = GlobalEnums.ImageSource.External;
			}
		}
		
		/// <summary>
		/// Where did the image come from
		/// </summary>
		/// 
		public GlobalEnums.ImageSource ImageSource 
		{
			get {return imageSource;}
			set {this.imageSource = value;}
		}
		
		///<summary>
		/// enlarge / Shrink the Controls Size
		/// </summary>
		public bool ScaleImageToSize
		{
			get {
				return scaleImageToSize;
			}
			set {
				scaleImageToSize = value;
			}
		}
		
		
		public override Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		

		public override System.Drawing.Drawing2D.DashStyle DashStyle {
			get { return base.DashStyle; }
			set { base.DashStyle = value; }
		}
		
		
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		
		
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		
		
		public override int Thickness {
			get { return base.Thickness; }
			set { base.Thickness = value; }
		}
		
		#endregion
		
	}
}
