using System;

namespace Managers
{
    public class SomeButtonClickedManager
    {
        public event EventHandler PlayButtonClicked, OnlineButtonClicked, QuitButtonClicked, StartButtonClicked, MenuButtonClicked;

        public void OnSomeButtonClicked(string name)
        {
            switch (name)
            {
                case "Play":
                    PlayButtonClicked?.Invoke(this, null);
                    break;
                case "Online":
                    OnlineButtonClicked?.Invoke(this, null);
                    break;
                case "Quit":
                    QuitButtonClicked?.Invoke(this, null);
                    break;
                case "Start":
                    StartButtonClicked?.Invoke(this, null);
                    break;
                case "Menu":
                    MenuButtonClicked?.Invoke(this, null);
                    break;
            }
        }
    }
}