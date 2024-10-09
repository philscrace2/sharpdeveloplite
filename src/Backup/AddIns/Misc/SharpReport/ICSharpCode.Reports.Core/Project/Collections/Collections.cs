// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core{
	
	public class CollectionChangedEventArgs<T> : EventArgs
	{
		T item;
		
		public T Item {
			get {
				return item;
			}
		}
		
		public CollectionChangedEventArgs(T item)
		{
			this.item = item;
		}
	}
	///<summary>
	/// Comparer to Sort the <see cref="ReportItemCollection"></see>
	/// by System.Drawing.Location.Y, so we have <see cref="BaseReportItem"></see> in the Order we use them
	/// (Line by Line)
	/// </summary>
	internal class LocationSorter : IComparer<BaseReportItem>  {
		public int Compare(BaseReportItem x, BaseReportItem y){
			if (x == null){
				if (y == null){
					return 0;
				}
				return -1;
			}
			if (y == null){
				return 1;
			}
			
			if (x.Location.Y == y.Location.Y){
				return x.Location.X - y.Location.X;
			}
			return x.Location.Y - y.Location.Y;
		}
	}
	

	///<summary>
	///  A collection that holds <see cref='IItemRenderer'/> objects.
	///</summary>
	public class ReportItemCollection : Collection<BaseReportItem>
	{
		public event EventHandler<CollectionChangedEventArgs<BaseReportItem>> Added;
		public event EventHandler<CollectionChangedEventArgs<BaseReportItem>> Removed;
		
		// Trick to get the inner list as List<T> (InnerList always has that type because we only use
		// the parameterless constructor on Collection<T>)
		
		private List<BaseReportItem> InnerList {
			get { return (List<BaseReportItem>)base.Items; }
		}
		
		public void Sort(IComparer<BaseReportItem> comparer)
		{
			InnerList.Sort(comparer);
		}
		
		public void ForEach (Action <BaseReportItem> action)
		{
			this.InnerList.ForEach (action);
		}
		
		
		/*
		public ExporterCollection ConvertAll<BaseExportColumn>(Converter<BaseReportItem, ICSharpCode.Reports.Core.Exporter.BaseExportColumn> converter)
		{
			List<ICSharpCode.Reports.Core.Exporter.BaseExportColumn> l  = InnerList.ConvertAll(converter);
			ExporterCollection e = new ExporterCollection();
			e.AddRange(l);
			return e;
		}
		*/
		
		public void AddRange(IEnumerable<BaseReportItem> items)
		{
			foreach (BaseReportItem item in items) Add(item);
		}
		
		
		public void SortByLocation () {
			if (this.Count > 1) {
				this.Sort(new LocationSorter());
			}
		}
		
		
		public bool Exist (string itemName)
		{
			if (String.IsNullOrEmpty(itemName)) {
				throw new ArgumentNullException("itemName");
			}
			
			if (InnerFind(itemName) == null) {
				return false;
			}
			else {
				return true;
			}	
		}
		
	
		private BaseReportItem InnerFind (string name)
		{
			var query = from bt in base.Items where bt.Name == name select bt;
			if (query.Count() >0) {
				return query.FirstOrDefault();
			} else {
				return null;
			}
		}
		
		
		public BaseReportItem Find (string itemName)
		{
			if (String.IsNullOrEmpty(itemName)) {
				throw new ArgumentNullException("itemName");
			}
			return this.InnerFind (itemName);
		}
		
		
		public BaseReportItem FindHighestElement()
		{
			if (this.InnerList.Count == 0) {
				return null;
			}
			BaseReportItem heighest = this.InnerList[0];
			foreach (BaseReportItem item in this.InnerList)
			{
				if (item.Size.Height > heighest.Size.Height) {
					heighest = item;
				}
			}
			return heighest;
		}
		
		
		protected override void InsertItem(int index, BaseReportItem item)
		{
			base.InsertItem(index, item);
			this.OnAdded (item);
		}
		
		
		protected override void RemoveItem(int index)
		{
			BaseReportItem item = this[index];
			base.RemoveItem(index);
			this.OnRemoved(item);
		}
		
		
		void OnAdded(BaseReportItem item){
			if (Added != null)
				Added(this, new CollectionChangedEventArgs<BaseReportItem>(item));
		}
		
		
		void OnRemoved( BaseReportItem item){
			if (Removed != null)
				Removed(this, new CollectionChangedEventArgs<BaseReportItem>(item));
		}
	}

	/// <summary>
	/// This class holds all the available Sections of an Report
	/// </summary>
	[Serializable()]
	public sealed class ReportSectionCollection: Collection<BaseSection>
	{
	}
	
	
	
	[Serializable()]
	public class AvailableFieldsCollection: Collection<AbstractColumn>{
		
		public AvailableFieldsCollection(){
		}
		
		public AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			var query = from bt in this where bt.ColumnName == columnName select bt;
			if (query.Count() >0) {
				return query.FirstOrDefault();
			} else {
				return null;
			}
		}
	}
	[Serializable()]
	public class SortColumnCollection: Collection<SortColumn>{
		
		public SortColumnCollection()
		{
		}
		
		public AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			var query = from bt in this where bt.ColumnName == columnName select bt;
			if (query.Count() >0) {
				return query.FirstOrDefault();
			} else {
				return null;
			}
		}
	
		
		public void AddRange (IEnumerable<SortColumn> items)
		{
			foreach (SortColumn item in items){
				this.Add(item);
			}
		}
		
		
		/// <summary>
		/// The Culture is used for direct String Comparison
		/// </summary>
		
		public static CultureInfo Culture
		{
			get { return CultureInfo.CurrentCulture;}
		}
	}
	
	
	
	[Serializable()]
	public class ColumnCollection: Collection<AbstractColumn>{
		
		public ColumnCollection()
		{
		}
		
		public AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			var query = from bt in this where bt.ColumnName == columnName select bt;
			if (query.Count() >0) {
				return query.FirstOrDefault();
			} else {
				return null;
			}
		}
	
		
		public void AddRange (IEnumerable<AbstractColumn> items)
		{
			foreach (AbstractColumn item in items){
				this.Add(item);
			}
		}
		
		
		/// <summary>
		/// The Culture is used for direct String Comparison
		/// </summary>
		
		public static CultureInfo Culture
		{
			get { return CultureInfo.CurrentCulture;}
		}
	}
	

	
	public class ParameterCollection: Collection<BasicParameter>{
		
		public ParameterCollection()
		{			
		}
		
		
		public BasicParameter Find (string parameterName)
		{
			if (String.IsNullOrEmpty(parameterName)) {
				throw new ArgumentNullException("parameterName");
			}
			var query = from bt in this where bt.ParameterName == parameterName select bt;
			if (query.Count() >0) {
				return query.FirstOrDefault();
			} else {
				return null;
			}
		}
		
		
		public  System.Collections.Hashtable CreateHash ()
		{
			System.Collections.Hashtable ht = new System.Collections.Hashtable();
			foreach(BasicParameter bt in this){
					ht.Add(bt.ParameterName,bt.ParameterValue);
			}
			return ht;
		}
		
		
		public System.Collections.Generic.List<SqlParameter> ExtractSqlParameters ()
		{
			System.Collections.Generic.List<SqlParameter> sql = new List<SqlParameter>();
			sql = (from t in this where t is SqlParameter select (SqlParameter)t).ToList<SqlParameter>();
			return sql;
		}
		
		
		public static CultureInfo Culture
		{
			get { return System.Globalization.CultureInfo.CurrentCulture; }
		}
		
		
		public void AddRange (IEnumerable<BasicParameter> items)
		{
			foreach (BasicParameter item in items){
				this.Add(item);
			}
		}
	}
	
	#region ExporterCollection
	public class ExporterCollection : Collection<BaseExportColumn>
	{
		
		public void AddRange (IEnumerable <BaseExportColumn> items){
			foreach (var item in items) {
				IExportContainer container = item as IExportContainer;
				if (container != null) {
					AddRange(container.Items);
				}
				this.Add (item);
			}
		}
	}
	
	
	public class PagesCollection  :Collection<ExporterPage>
	{
		
	}
	
	
	#endregion
	
	public class  CurrentItemsCollection:Collection<CurrentItem>
	{
		public CurrentItem Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			var query = from bt in this where bt.ColumnName.ToLower() == columnName.ToLower(CultureInfo.InvariantCulture) select bt;
			if (query.Count() >0) {
				return query.FirstOrDefault();
			} else {
				return null;
			}
		}
	}
}
