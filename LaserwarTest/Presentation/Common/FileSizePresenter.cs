using LaserwarTest.Commons.Observables;

namespace LaserwarTest.Presentation.Common
{
    public class FileSizePresenter : ObservablePresenter<int>
    {
        const string Bytes = "";
        const string KBytes = "Kb";
        const string MBytes = "Mb";
        const string GBytes = "Gb";
        const string TBytes = "Tb";

        public FileSizePresenter(int sizeInBytes) : base(sizeInBytes, $"{sizeInBytes} {Bytes}")
        {
            string[] sizes = { Bytes, KBytes, MBytes, GBytes, TBytes };

            int order = 0;
            while (sizeInBytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                sizeInBytes = sizeInBytes / 1024;
            }

            DisplayText = $"{sizeInBytes} {sizes[order]}";
        }
    }
}
