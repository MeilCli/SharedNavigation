using SharedNavigation.NETStandard;

namespace SharedNavigation.Sample.NETCore
{
    internal class ViewModel : INavigationViewModel
    {
        [InjectNavigation]
        public INavigationCommand<ViewModel> PushCommand { get; set; } = new NavigationCommand<ViewModel>();

        [InjectNavigation]
        public INavigationCommand<ViewModel> PopCommand { get; set; } = new NavigationCommand<ViewModel>();

        public void RestoreState(byte[] state)
        {
        }

        public byte[] SaveState()
        {
            return new byte[0];
        }
    }
}
