using System;
using System.Collections.Generic;
using System.Linq;
using Programe.Gui;
using Programe.Gui.Widgets;

namespace Programe
{
    static class Interface
    {
        private static GuiSystem gui;

        private static Window loginWindow;
        private static TextBox loginUsername;
        private static TextBox loginPassword;
        private static Label loginStatus;

        private static Window registerWindow;
        private static TextBox registerUsername;
        private static TextBox registerPassword1;
        private static TextBox registerPassword2;
        private static Label registerStatus;

        private static ListBox statusWindowList;
        
        public static void Start(GuiSystem guiSystem)
        {
            gui = guiSystem;

            var desktop = new Container(gui.SurfaceWidth, gui.SurfaceHeight - 1);
            desktop.Left = 0;
            desktop.Top = 1;
            gui.Add(desktop);

            var statusWindow = new Window(10, 10, 70, 20, "Status Messages");
            statusWindow.Visible = false;
            desktop.Add(statusWindow);

            statusWindowList = new ListBox(1, 0, 66, 18);
            statusWindowList.SelectEnabled = false;
            statusWindow.Add(statusWindowList);

            #region Login Window
            loginWindow = new Window(10, 10, 35, 11, "Login");
            loginWindow.Visible = false;
            desktop.Add(loginWindow);

            var loginUsernameLabel = new Label(1, 1, 31, 1, "Username:");
            loginWindow.Add(loginUsernameLabel);

            loginUsername = new TextBox(1, 2, 31);
            loginWindow.Add(loginUsername);

            var loginPasswordLabel = new Label(1, 4, 31, 1, "Password:");
            loginWindow.Add(loginPasswordLabel);

            loginPassword = new TextBox(1, 5, 31);
            loginPassword.PasswordCharacter = (char)3;
            loginWindow.Add(loginPassword);

            var loginButton = new Button(24, 7, 8, "Login");
            loginWindow.Add(loginButton);

            loginStatus = new Label(1, 7, 22, 1, "Not connected");
            loginWindow.Add(loginStatus);
            #endregion

            #region Register Window
            registerWindow = new Window(10, 10, 35, 14, "Register");
            registerWindow.Visible = false;
            desktop.Add(registerWindow);

            var registerUsernameLabel = new Label(1, 1, 31, 1, "Username:");
            registerWindow.Add(registerUsernameLabel);

            registerUsername = new TextBox(1, 2, 31);
            registerWindow.Add(registerUsername);

            var registerPassword1Label = new Label(1, 4, 31, 1, "Password:");
            registerWindow.Add(registerPassword1Label);

            registerPassword1 = new TextBox(1, 5, 31);
            registerPassword1.PasswordCharacter = (char)3;
            registerWindow.Add(registerPassword1);

            var registerPassword2Label = new Label(1, 7, 31, 1, "Confirm Password:");
            registerWindow.Add(registerPassword2Label);

            registerPassword2 = new TextBox(1, 8, 31);
            registerPassword2.PasswordCharacter = (char)3;
            registerWindow.Add(registerPassword2);

            var registerButton = new Button(21, 10, 11, "Register");
            registerWindow.Add(registerButton);

            registerStatus = new Label(1, 10, 19, 1, "Not connected");
            registerWindow.Add(registerStatus);
            #endregion

            #region Menu
            var menu = new MenuBar();

            var account = new MenuItem("Account");
            var login = new MenuItem("Login");
            login.Clicked += () =>
            {
                ResetAccountWindows();
                loginWindow.Visible = true;
                loginWindow.Focus();
            };

            var register = new MenuItem("Register");
            register.Clicked += () =>
            {
                ResetAccountWindows();
                registerWindow.Visible = true;
                registerWindow.Focus();
            };

            account.Items.Add(login);
            account.Items.Add(register);
            menu.Items.Add(account);

            var view = new MenuItem("View");
            var status = new MenuItem("Status Messages");
            status.Clicked += () =>
            {
                statusWindow.Visible = true;
                statusWindow.Focus();
            };

            view.Items.Add(status);
            menu.Items.Add(view);
            #endregion

            gui.Add(menu);
        }

        public static void Connected()
        {
            loginStatus.Caption = "";
            registerStatus.Caption = "";
        }

        public static void Disconnected()
        {
            loginStatus.Caption = "Not connected";
            registerStatus.Caption = "Not connected";
        }

        public static void AddStatusMessage(string message)
        {
            if (statusWindowList.Items.Count > 150)
                statusWindowList.Items.RemoveAt(0);
            statusWindowList.Items.Add(new ListBoxItem(message));
        }

        private static void ResetAccountWindows()
        {
            loginUsername.Value = "";
            loginPassword.Value = "";
            loginWindow.Visible = false;
            CenterWindow(loginWindow);

            registerUsername.Value = "";
            registerPassword1.Value = "";
            registerPassword2.Value = "";
            registerWindow.Visible = false;
            CenterWindow(registerWindow);
        }

        private static void CenterWindow(Window window)
        {
            var halfWidth = (int)gui.SurfaceWidth / 2;
            var halfHeight = (int)gui.SurfaceHeight / 2;
            window.Left = (int)(halfWidth - (window.Width / 2));
            window.Top = (int)(halfHeight - (window.Height / 2));
        }
    }
}
