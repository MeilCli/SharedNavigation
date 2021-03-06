using System;
using System.Threading.Tasks;

namespace SharedNavigation.Sample.NETCore
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Navigation!!");
            var view = new View();
            var viewModel = new ViewModel();
            view.RegisterNavigationByAutoGenerated(viewModel, null);
            await viewModel.PushCommand.NavigateAsync(viewModel);
            await viewModel.PopCommand.NavigateAsync(viewModel);
        }
    }
}
