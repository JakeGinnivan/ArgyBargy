using System;

namespace ArgyBargy
{
    [Flags]
    internal enum ApplicationState
    {
        Available = 0,
        Busy = 1,
        DialogueShowing = 2
    }
}