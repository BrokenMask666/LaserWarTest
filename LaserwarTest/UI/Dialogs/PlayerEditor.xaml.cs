using LaserwarTest.Core.UI.Popups;
using LaserwarTest.Data.DB;
using LaserwarTest.Data.DB.Entities;
using LaserwarTest.Presentation.Games;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaserwarTest.UI.Dialogs
{
    public sealed partial class PlayerEditor : UserControl, IPopupUIElement
    {
        public event EventHandler CanClose;
        public event EventHandler<Player> PlayerSaved;

        Player Player { get; }

        public PlayerEditor(int playerID)
        {
            InitializeComponent();
            using (var db = DBManager.GetLocalDB().Connection.Open())
                Player = new Player(db.Get<PlayerEntity>(playerID));
        }

        private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CanClose?.Invoke(this, EventArgs.Empty);
        }

        private void SaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Player.Save();
            CanClose?.Invoke(this, EventArgs.Empty);
            PlayerSaved?.Invoke(this, Player);
        }
    }
}
