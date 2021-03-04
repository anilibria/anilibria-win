using Anilibria.Controls.PresentationClasses;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Anilibria.Controls {

	public sealed partial class AlphabetUserControl : UserControl {

		private string m_Alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

		public ICommand Checked {
			get => (ICommand) GetValue ( CheckedProperty );
			set => SetValue ( CheckedProperty , value );
		}

		public static readonly DependencyProperty CheckedProperty =
			DependencyProperty.Register (
				"Checked" ,
				typeof ( ICommand ) ,
				typeof ( AlphabetUserControl ) ,
				new PropertyMetadata ( false )
		);

		public ICommand UnChecked {
			get => (ICommand) GetValue ( UnCheckedProperty );
			set => SetValue ( UnCheckedProperty , value );
		}

		public static readonly DependencyProperty UnCheckedProperty =
			DependencyProperty.Register (
				"UnChecked" ,
				typeof ( ICommand ) ,
				typeof ( AlphabetUserControl ) ,
				new PropertyMetadata ( false )
		);

		public ICommand ClickOutside {
			get => (ICommand) GetValue ( ClickOutsideProperty );
			set => SetValue ( ClickOutsideProperty , value );
		}

		public static readonly DependencyProperty ClickOutsideProperty =
			DependencyProperty.Register (
				"ClickOutside" ,
				typeof ( ICommand ) ,
				typeof ( AlphabetUserControl ) ,
				new PropertyMetadata ( false )
		);

		public AlphabetUserControl () {
			InitializeComponent ();

			var characters = m_Alphabet.Select (
				a => new AlphabetModel {
					Title = a.ToString ()
				}
			).ToList ();

			ItemContainer.ItemsSource = new ObservableCollection<AlphabetModel> ( characters );
		}

		private void Grid_PointerPressed ( object sender , PointerRoutedEventArgs e ) {
			ClickOutside.Execute ( null );
		}

		private void ToggleButton_Checked ( object sender , RoutedEventArgs e ) {
			var button = sender as ToggleButton;
			if ( button == null ) return;

			var textBlock = button.Content as TextBlock;
			if ( textBlock == null ) return;

			if ( Checked == null ) return;

			Checked.Execute ( textBlock.Text );
		}

		private void ToggleButton_Unchecked ( object sender , RoutedEventArgs e ) {
			var button = sender as ToggleButton;
			if ( button == null ) return;

			var textBlock = button.Content as TextBlock;
			if ( textBlock == null ) return;

			UnChecked.Execute ( textBlock.Text );
		}

	}

}
