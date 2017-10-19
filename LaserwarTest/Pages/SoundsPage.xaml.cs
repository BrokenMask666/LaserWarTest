using LaserwarTest.Presentation.Sounds;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SoundsPage : Page
    {
        VMSounds VMSounds { get; } = new VMSounds();

        public SoundsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await VMSounds.Load();

            if (VMSounds.Items.Count == 0)
                VisualStateManager.GoToState(this, nameof(NoDataState), false);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VMSounds.Dispose();
        }
    }
}
