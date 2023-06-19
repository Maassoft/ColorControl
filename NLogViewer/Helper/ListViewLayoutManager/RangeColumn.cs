using System;
using System.Windows;
using System.Windows.Controls;

namespace DJ.Helper.ListViewLayoutManager
{

	
	public sealed class RangeColumn : LayoutColumn
	{

		
		public static readonly DependencyProperty MinWidthProperty = 
			DependencyProperty.RegisterAttached(
				"MinWidth",
				typeof( double ),
				typeof( RangeColumn ) );

		
		public static readonly DependencyProperty MaxWidthProperty =
			DependencyProperty.RegisterAttached(
				"MaxWidth",
				typeof( double ),
				typeof( RangeColumn ) );

		
		public static readonly DependencyProperty IsFillColumnProperty =
			DependencyProperty.RegisterAttached(
				"IsFillColumn",
				typeof( bool ),
				typeof( RangeColumn ) );

		
		private RangeColumn()
		{
		} // RangeColumn

		
		public static double GetMinWidth( DependencyObject obj )
		{
			return (double)obj.GetValue( MinWidthProperty );
		} // GetMinWidth

		
		public static void SetMinWidth( DependencyObject obj, double minWidth )
		{
			obj.SetValue( MinWidthProperty, minWidth );
		} // SetMinWidth

		
		public static double GetMaxWidth( DependencyObject obj )
		{
			return (double)obj.GetValue( MaxWidthProperty );
		} // GetMaxWidth

		
		public static void SetMaxWidth( DependencyObject obj, double maxWidth )
		{
			obj.SetValue( MaxWidthProperty, maxWidth );
		} // SetMaxWidth

		
		public static bool GetIsFillColumn( DependencyObject obj )
		{
			return (bool)obj.GetValue( IsFillColumnProperty );
		} // GetIsFillColumn

		
		public static void SetIsFillColumn( DependencyObject obj, bool isFillColumn )
		{
			obj.SetValue( IsFillColumnProperty, isFillColumn );
		} // SetIsFillColumn

		
		public static bool IsRangeColumn( GridViewColumn column )
		{
			if ( column == null )
			{
				return false;
			}
			return
				HasPropertyValue( column, MinWidthProperty ) ||
				HasPropertyValue( column, MaxWidthProperty ) ||
				HasPropertyValue( column, IsFillColumnProperty );
		} // IsRangeColumn

		
		public static double? GetRangeMinWidth( GridViewColumn column )
		{
			return GetColumnWidth( column, MinWidthProperty );
		} // GetRangeMinWidth

		
		public static double? GetRangeMaxWidth( GridViewColumn column )
		{
			return GetColumnWidth( column, MaxWidthProperty );
		} // GetRangeMaxWidth

		
		public static bool? GetRangeIsFillColumn( GridViewColumn column )
		{
			if ( column == null )
			{
				throw new ArgumentNullException( nameof(column) );
			}
			object value = column.ReadLocalValue( IsFillColumnProperty );
			if ( value != null && value.GetType() == IsFillColumnProperty.PropertyType )
			{
				return (bool)value;
			}

			return null;
		} // GetRangeIsFillColumn

		
		public static GridViewColumn ApplyWidth( GridViewColumn gridViewColumn, double minWidth, 
			double width, double maxWidth )
		{
			return ApplyWidth( gridViewColumn, minWidth, width, maxWidth, false );
		} // ApplyWidth

		
		public static GridViewColumn ApplyWidth( GridViewColumn gridViewColumn, double minWidth, 
			double width, double maxWidth, bool isFillColumn )
		{
			SetMinWidth( gridViewColumn, minWidth );
			gridViewColumn.Width = width;
			SetMaxWidth( gridViewColumn, maxWidth );
			SetIsFillColumn( gridViewColumn, isFillColumn );
			return gridViewColumn;
		} // ApplyWidth

	} // class RangeColumn

} // namespace Itenso.Windows.Controls.ListViewLayout
// -- EOF -------------------------------------------------------------------
