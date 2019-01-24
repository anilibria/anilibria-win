using System;
using System.Windows.Input;

namespace Anilibria.MVVM {

	/// <summary>
	/// UI command.
	/// </summary>
	public class UICommand : ICommand {

		private readonly WeakAction m_CommandAction;

		private readonly WeakFunc<bool> m_CanExecute;

		public event EventHandler CanExecuteChanged;

		public UICommand ( Action commandAction , Func<bool> canExecute = null ) {
			if ( commandAction == null ) throw new ArgumentNullException ( nameof ( commandAction ) );

			m_CommandAction = new WeakAction ( commandAction );

			if ( canExecute != null ) m_CanExecute = new WeakFunc<bool> ( canExecute );
		}

		public void RaiseCanExecuteChanged () => CanExecuteChanged?.Invoke ( this , EventArgs.Empty );

		public bool CanExecute ( object parameter ) => m_CanExecute == null || ( m_CanExecute.IsStatic || m_CanExecute.IsAlive ) && m_CanExecute.Execute ();

		public virtual void Execute ( object parameter ) {
			if ( CanExecute ( parameter ) && m_CommandAction != null && ( m_CommandAction.IsStatic || m_CommandAction.IsAlive ) ) m_CommandAction.Execute ();
		}

	}

}