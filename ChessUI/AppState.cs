namespace ChessUI
{
    /// <summary>
    /// One shared bag of app-wide state.
    /// </summary>
    public static class AppState
    {
        // default value: the user has not confirmed yet
        private static bool userConfirmed = false;

        /// <summary>
        /// Starts out false; set to true once the user clicks YES on the
        /// startup prompt (see <see cref="MenuController.ShowUserPrompt"/>).
        /// </summary>
        public static bool UserConfirmed
        {
            get => userConfirmed;
            set => userConfirmed = value;
        }
    }
}
