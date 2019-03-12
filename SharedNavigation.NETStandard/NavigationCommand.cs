using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharedNavigation.NETStandard
{
    public class NavigationCommand<T> : INavigationCommand<T> where T : class, INavigationViewModel, new()
    {
        public INavigationAction NavigationAction { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanNavigate(T viewModel)
        {
            return NavigationAction?.CanNavigate(viewModel) ?? false;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanNavigate(parameter as T);
        }

        public Task NavigateAsync(T viewModel)
        {
            if (NavigationAction is null)
            {
                throw new InvalidOperationException($"Not injected {nameof(NavigationAction)}");
            }
            return NavigationAction.NavigateAsync(viewModel);
        }

        void ICommand.Execute(object parameter)
        {
            NavigateAsync(parameter as T);
        }
    }
}
