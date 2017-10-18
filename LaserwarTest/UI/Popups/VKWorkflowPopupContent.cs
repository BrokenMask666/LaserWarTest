using LaserwarTest.Core.UI.Popups;
using LaserwarTest.Core.UI.Popups.Animations;
using LaserwarTest.UI.Dialogs;
using LaserwarTest.UI.Popups.Animations;
using System;

namespace LaserwarTest.UI.Popups
{
    public class VKWorkflowPopupContent : PopupContent
    {
        VKWorkflowDialog Dialog => Content as VKWorkflowDialog;

        public VKWorkflowPopupContent(Type redirectPageType, IPopupContentAnimation openAnimation = null, IPopupContentAnimation closeAnimation = null) : base(
            new VKWorkflowDialog(redirectPageType) { Width = 760, Height = 605 },
            openAnimation ?? new ScalePopupOpenAnimation(),
            closeAnimation ?? new ScalePopupCloseAnimation())
        {
        }
    }
}
