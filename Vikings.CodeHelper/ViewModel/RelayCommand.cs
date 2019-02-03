using System;
using System.Windows.Input;

namespace Vikings.CodeHelper.ViewModel
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action executeAction) : base(f => executeAction()) { }
    }

    public class RelayCommand<T> : ICommand
    {
        readonly Action<T> ExecuteAction;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<T> executeAction) => ExecuteAction = executeAction;

        bool isEnabled = true;
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    OnCanExecuteChanged();
                }
            }
        }

        static object TryCast(object value, Type targetType)
        {
            Type type = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (type.IsEnum && value is string)
            {
                value = Enum.Parse(type, (string)value, false);
            }
            else if (value is IConvertible && !targetType.IsAssignableFrom(value.GetType()))
            {
                value = Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (value == null && targetType.IsValueType)
            {
                value = Activator.CreateInstance(targetType);
            }
            return value;
        }

        static T GetGenericParameter(object parameter)
        {
            parameter = TryCast(parameter, typeof(T));
            if (parameter == null || parameter is T) return (T)parameter;
            return default(T);
        }

        protected virtual void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object parameter) => IsEnabled;

        public void Execute(object parameter) => ExecuteAction(GetGenericParameter(parameter));
    }
}
