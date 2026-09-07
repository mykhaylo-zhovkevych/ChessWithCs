using System;
using System.Windows;
using System.Windows.Controls;
using ChessLogic;
using ChessLogic.Enum;

namespace ChessUI
{
    /// <summary>
    /// Owns the single overlay slot (the MenuContainer ContentControl) and knows
    /// how to show every menu and wire up its callback. This keeps MainWindow
    /// focused on the board and the game rules.
    /// </summary>
    public class MenuController
    {
        private readonly ContentControl host;
        private readonly Action onRestart;

        public MenuController(ContentControl host, Action onRestart)
        {
            this.host = host;
            this.onRestart = onRestart;
        }

        public bool IsOpen => host.Content != null;

        public void Close() => host.Content = null;

        public void ShowPromotion(Player player, Action<PieceType> onPicked)
        {
            PromotionMenu menu = new PromotionMenu(player);
            host.Content = menu;

            menu.PieceSelected += type =>
            {
                Close();
                onPicked(type);
            };
        }

        public void ShowGameOver(GameState gameState)
        {
            GameOverMenu menu = new GameOverMenu(gameState);
            host.Content = menu;

            menu.OptionSelected += option =>
            {
                if (option == Option.Exit)
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    Close();
                    onRestart();
                }
            };
        }

        public void ShowPause()
        {
            PauseMenu menu = new PauseMenu();
            host.Content = menu;

            menu.OptionSelected += option =>
            {
                Close();

                if (option == Option.Restart)
                {
                    onRestart();
                }
            };
        }
  
        public void ShowUserPrompt(string userName, string localIp, int port, string defaultJoinHost, Action onHost, Action<string> onJoin)
        {
            UserPromptMenu prompt = new UserPromptMenu(userName, localIp, port);
            host.Content = prompt;

            prompt.HostSelected += () =>
            {
                AppState.UserConfirmed = true;
                Close();
                onHost();
            };

            prompt.JoinSelected += () =>
            {
                AppState.UserConfirmed = true;
                ShowUserMenu(defaultJoinHost, onJoin);
            };
        }

        public void ShowUserMenu(string defaultJoinHost, Action<string> onJoin)
        {
            UserMenu menu = new UserMenu(defaultJoinHost);
            host.Content = menu;

            menu.Submitted += ip =>
            {
                Close();
                onJoin(ip);
            };
        }
    }
}
