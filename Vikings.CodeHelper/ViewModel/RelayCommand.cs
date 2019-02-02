using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Vikings.CodeHelper.ViewModel
{
    public class RelayCommand : ICommand
    {
        readonly Action ExecuteAction;

        public RelayCommand(Action executeAction) => ExecuteAction = executeAction;

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => ExecuteAction();
    }
}
