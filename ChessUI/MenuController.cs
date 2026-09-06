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
        public void ShowUserPrompt()
        {
            ChessClient client = new ChessClient();

            UserPromptMenu prompt = new UserPromptMenu(client.UserName, client.LocalIp);
            host.Content = prompt;

            prompt.Confirmed += () =>
            {
                AppState.UserConfirmed = true;
                // step 2 of the startup sequence
                ShowUserMenu(client.LocalIp);  
            };
        }

        public void ShowUserMenu(string ipAddress)
        {
            UserMenu menu = new UserMenu(ipAddress);
            host.Content = menu;

            menu.Submitted += (userName, ip) =>
            {
                ChessClient.DebugPrintSubmitted(userName, ip);
                Close();
            };
        }
    }
}
