using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedNavigation.NETStandard
{
    public interface INavigationAction
    {
        bool CanNavigate<T>(T viewModel) where T : INavigationViewModel, new();

        Task NavigateAsync<T>(T viewModel) where T : INavigationViewModel, new();
    }
}
