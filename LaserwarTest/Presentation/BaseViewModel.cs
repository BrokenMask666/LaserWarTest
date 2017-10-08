using LaserwarTest.Commons.Observables;
using LaserwarTest.Pages;
using LaserwarTest.UI.Popups;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation
{
    /// <summary>
    /// Базовая модель представления
    /// </summary>
    public class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Инициирует появление экрана загрузки.
        /// Использует небольшое ожидание, чтобы UI успел отреагировать для более плавного отклика
        /// </summary>
        /// <param name="delay">Длительность задержки в миллисекундах</param>
        /// <returns></returns>
        protected async Task Loading(int delay = 60)
        {
            AppShell.GetCurrent().SetLoading();
            if (delay > 0) await Task.Delay(delay);
        }

        protected void Loaded() => AppShell.GetCurrent().SetLoaded();

        protected void ShowError(string message, bool modal = false) =>
            new ErrorPopupContent(message).Open(modal);

        protected void ShowError(string title, string message, bool modal = false) =>
            new ErrorPopupContent(title, message).Open(modal);
    }
}
