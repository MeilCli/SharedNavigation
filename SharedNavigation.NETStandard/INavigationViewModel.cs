using System;
using System.Collections.Generic;
using System.Text;

namespace SharedNavigation.NETStandard
{
    public interface INavigationViewModel
    {
        void RestoreState(byte[] state);

        byte[] SaveState();
    }
}
