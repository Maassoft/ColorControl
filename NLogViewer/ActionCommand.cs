using System;
using System.Windows.Input;

namespace DJ
{
    public sealed class ActionCommand : ICommand
    {
        private readonly Action _Action;
        private readonly Action<object> _ObjectAction;

        public ActionCommand(Action action)
        {
            _Action = action;
        }

        public ActionCommand(Action<object> objectAction)
        {
            _ObjectAction = objectAction;
        }

        private event EventHandler CanExecuteChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CanExecuteChanged += value;
            remove => CanExecuteChanged -= value;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_ObjectAction != null)
                _ObjectAction(parameter);
            else
                _Action();
        }
    }
}