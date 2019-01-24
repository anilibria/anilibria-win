using System;
using System.Windows.Input;
using System.Reflection;

namespace Anilibria.MVVM {

	/// <summary>
	/// UI generic command.
	/// </summary>
	public class UIGenericCommand<T> : ICommand {

		private readonly WeakAction<T> m_CommandAction;

		private readonly WeakFunc<T , bool> m_CanExecute;

		public event EventHandler CanExecuteChanged;

		public UIGenericCommand ( Action<T> commandAction , Func<T , bool> canExecute = null ) {
			if ( commandAction == null ) throw new ArgumentNullException ( nameof ( commandAction ) );

			m_CommandAction = new WeakAction<T> ( commandAction );

			if ( canExecute != null ) m_CanExecute = new WeakFunc<T , bool> ( canExecute );
		}

		public void RaiseCanExecuteChanged () => CanExecuteChanged?.Invoke ( this , EventArgs.Empty );

		public bool CanExecute ( object parameter ) {
			if ( m_CanExecute == null ) return true;

			if ( m_CanExecute.IsStatic || m_CanExecute.IsAlive ) {
				if ( parameter == null && typeof ( T ).GetTypeInfo ().IsValueType ) return m_CanExecute.Execute ( default ( T ) );
				if ( parameter == null || parameter is T ) return ( m_CanExecute.Execute ( (T) parameter ) );
			}

			return false;
		}

		public virtual void Execute ( object parameter ) {
			var isCanExecute = CanExecute ( parameter ) && m_CommandAction != null && ( m_CommandAction.IsStatic || m_CommandAction.IsAlive );
			if ( !isCanExecute ) return;

			if ( parameter == null ) {
				if ( typeof ( T ).GetTypeInfo ().IsValueType ) {
					m_CommandAction.Execute ( default ( T ) );
				}
				else {
					m_CommandAction.Execute ( (T) parameter );
				}
			}
			else {
				m_CommandAction.Execute ( (T) parameter );
			}
		}

	}

}