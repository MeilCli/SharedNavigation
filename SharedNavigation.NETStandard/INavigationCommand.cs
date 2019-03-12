using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharedNavigation.NETStandard
{
    public interface INavigationCommand<T> : ICommand where T : class, INavigationViewModel, new()
    {
        INavigationAction NavigationAction { get; set; }

        bool CanNavigate(T viewModel);

        Task NavigateAsync(T viewModel);
    }
}
