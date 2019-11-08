using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Controls {

	/// <summary>
	/// <see cref="ListView"/> with IsMultipleSelect and MultipleSelectedItem.
	/// </summary>
	public class BindableListView : ListView {

		public BindableListView () {
			Loaded += MultiSelectableGridControl_Loaded;
		}

		private void MultiSelectableGridControl_Loaded ( object sender , RoutedEventArgs e ) {
			Loaded -= MultiSelectableGridControl_Loaded;

			SelectionChanged += MultiSelectableSelectionChanged;
		}

		private void MultiSelectableSelectionChanged ( object sender , SelectionChangedEventArgs args ) {
			if ( MultipleSelectedItems == null ) return;

			foreach ( var addedItem in args.AddedItems ) MultipleSelectedItems.Add ( addedItem );
			foreach ( var removeItem in args.RemovedItems ) MultipleSelectedItems.Remove ( removeItem );
		}

		public IList MultipleSelectedItems
		{
			get => (IList) GetValue ( MultipleSelectedItemsProperty );
			set => SetValue ( MultipleSelectedItemsProperty , value );
		}

		public static readonly DependencyProperty MultipleSelectedItemsProperty =
			DependencyProperty.Register (
				"MultipleSelectedItems" ,
				typeof ( IList ) ,
				typeof ( BindableListView ) ,
				new PropertyMetadata ( null , MultipleSelectedItemsChangedHandler )
		);

		private static void MultipleSelectedItemsChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is BindableListView grid ) ) return;

			if ( !( e.NewValue is IList list ) || list.Count == 0 ) {
				grid.SelectedItem = null;
				return;
			}
		}

		public bool IsMultipleSelect
		{
			get => (bool) GetValue ( IsMultipleSelectProperty );
			set => SetValue ( IsMultipleSelectProperty , value );
		}

		/// <summary>
		/// Register is multiple select depedency property.
		/// </summary>
		public static readonly DependencyProperty IsMultipleSelectProperty =
			DependencyProperty.Register (
				"IsMultipleSelect" ,
				typeof ( bool ) ,
				typeof ( BindableListView ) ,
				new PropertyMetadata ( false , IsMultiSelectChanged )
		);

		private static void IsMultiSelectChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			BindableListView grid = d as BindableListView;
			if ( grid == null ) return;

			var value = (bool) e.NewValue;
			grid.SelectionMode = value ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Single;
			grid.SelectedIndex = -1;
		}


	}

}
