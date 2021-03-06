using SharedNavigation.NETStandard;
using System;
using System.Threading.Tasks;

namespace SharedNavigation.Sample.NETCore
{
    internal class View : INavigationView<ViewModel>
    {
        [Navigate(nameof(ViewModel.PopCommand))]
        public static async Task PopAsync(ViewModel viewModel)
        {
            Console.Out.WriteLine("PopAsync");
            await Task.CompletedTask;
        }

        [Navigate(nameof(ViewModel.PushCommand))]
        public async Task PushAsync(ViewModel viewModel)
        {
            Console.Out.WriteLine("PushAsync");
            await Task.CompletedTask;
        }

        public void RegisterNavigationByAutoGenerated(ViewModel viewModel, INavigationAction defaultNavigationAction)
        {
        }
    }
}
