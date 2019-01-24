using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Anilibria.MVVM {

	/// <summary>
	/// A base class for viewmodel classes.
	/// </summary>
	public class ViewModel : INotifyPropertyChanged {

		private readonly List<UICommand> m_Commands = new List<UICommand> ();

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Verify property name.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		[Conditional ( "DEBUG" )]
		[DebuggerStepThrough]
		public void VerifyPropertyName ( string propertyName ) {
			var myType = GetType ();

			if ( !string.IsNullOrEmpty ( propertyName ) && myType.GetTypeInfo ().GetDeclaredProperty ( propertyName ) == null ) {
				throw new ArgumentException ( "Property not found" , propertyName );
			}
		}

		/// <summary>
		/// Raise property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		public virtual void RaisePropertyChanged ( string propertyName ) {
			VerifyPropertyName ( propertyName );

			PropertyChanged?.Invoke ( this , new PropertyChangedEventArgs ( propertyName ) );
		}

		/// <summary>
		/// Raise property changed.
		/// </summary>
		/// <param name="propertyExpression">Property expression.</param>
		public virtual void RaisePropertyChanged<T> ( Expression<Func<T>> propertyExpression ) {
			var handler = PropertyChanged;

			if ( handler != null ) {
				var propertyName = GetPropertyName ( propertyExpression );

				if ( !string.IsNullOrEmpty ( propertyName ) ) {
					RaisePropertyChanged ( propertyName );
				}
			}
		}

		/// <summary>
		/// Get property name.
		/// </summary>
		/// <param name="propertyExpression">Property expression.</param>
		protected static string GetPropertyName<T> ( Expression<Func<T>> propertyExpression ) {
			if ( propertyExpression == null ) throw new ArgumentNullException ( "propertyExpression" );

			var body = propertyExpression.Body as MemberExpression;
			if ( body == null ) throw new ArgumentException ( "Invalid argument" , "propertyExpression" );

			var property = body.Member as PropertyInfo;
			if ( property == null ) throw new ArgumentException ( "Argument is not a property" , "propertyExpression" );

			return property.Name;
		}

		/// <summary>
		/// Raise property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <param name="oldValue">Old value.</param>
		/// <param name="newValue">New value.</param>
		public virtual void RaisePropertyChanged<T> ( [CallerMemberName] string propertyName = null , T oldValue = default ( T ) , T newValue = default ( T ) ) {
			if ( string.IsNullOrEmpty ( propertyName ) ) throw new ArgumentException ( "This method cannot be called with an empty string" , "propertyName" );

			RaisePropertyChanged ( propertyName );
		}

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="field">Field.</param>
		/// <param name="newValue">New value.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>Value changed after set operation.</returns>
		protected bool Set<T> ( ref T field , T newValue = default ( T ) , [CallerMemberName] string propertyName = null ) {
			if ( EqualityComparer<T>.Default.Equals ( field , newValue ) ) return false;

			var oldValue = field;
			field = newValue;

			RaisePropertyChanged ( propertyName , oldValue , field );

			return true;
		}

		/// <summary>
		/// Create relay command.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <returns></returns>
		protected ICommand CreateCommand ( Action action , bool isKeep = true ) {

			var command = new UICommand ( action );

			if ( isKeep ) m_Commands.Add ( command );

			return command;
		}

		/// <summary>
		/// Create relay command and canExecute function.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="canExecute">Can execute function.</param>
		/// <returns>Command.</returns>
		protected ICommand CreateCommand ( Action action , Func<bool> canExecute , bool isKeep = true ) {
			var command = new UICommand ( action , canExecute );

			if ( isKeep ) m_Commands.Add ( command );

			return command;
		}

		/// <summary>
		/// Create relay command with parameter.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <returns></returns>
		protected ICommand CreateCommand<T> ( Action<T> action ) {

			return new UIGenericCommand<T> ( action );
		}

		/// <summary>
		/// Create relay command with parameter and canExecute function.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="canExecute">Can execute function.</param>
		/// <returns>Command.</returns>
		protected ICommand CreateCommand<T> ( Action<T> action , Func<T , bool> canExecute ) {

			return new UIGenericCommand<T> ( action , canExecute );
		}

		/// <summary>
		/// Check can execute to all keep commands.
		/// </summary>
		public void RaiseCommands () {
			foreach ( var command in m_Commands ) command.RaiseCanExecuteChanged ();
		}

		/// <summary>
		/// Raise can execute changed.
		/// </summary>
		/// <param name="command">Command.</param>
		protected void RaiseCanExecuteChanged ( ICommand command ) {
			( command as UICommand ).RaiseCanExecuteChanged ();
		}

		/// <summary>
		/// Raise can execute changed.
		/// </summary>
		/// <param name="command">Command.</param>
		protected void RaiseCanExecuteChanged<T> ( ICommand command ) {
			( command as UIGenericCommand<T> ).RaiseCanExecuteChanged ();
		}

	}

}
