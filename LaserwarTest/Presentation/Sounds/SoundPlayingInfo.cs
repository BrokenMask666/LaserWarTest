using LaserwarTest.Commons.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation.Sounds
{
    /// <summary>
    /// Предоставляет доступ к информации о проигрывании файла
    /// </summary>
    public class SoundPlayingInfo : ObservableObject
    {
    }

    public enum PlaySoundState
    {
        Stopped,
        Playing,
        Disabled,
    }
}
