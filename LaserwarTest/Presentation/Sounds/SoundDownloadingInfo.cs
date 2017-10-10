using LaserwarTest.Commons.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation.Sounds
{
    /// <summary>
    /// Предоставляет доступ к информации о загрузке файла
    /// </summary>
    public class SoundDownloadingInfo : ObservableObject
    {
    }

    public enum DownloadSoundState
    {
        Download,
        Downloading,
        Downloaded
    }
}
