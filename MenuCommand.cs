using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace Earthquake_WPFApp {
    internal class MenuCommand {

        private readonly MainViewModel mainViewModel_;
        public ICommand ExitCommand { get; private set; }
        public ICommand ChangeBackgroundCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }

        public MenuCommand(MainViewModel mainViewModel) {
            this.mainViewModel_ = mainViewModel;

            ExitCommand = new Command(RunExitCommand);
            ChangeBackgroundCommand = new Command(RunChangeBackgroundCommand);
            AboutCommand = new Command(RunAboutCommand);
        }

        public void RunExitCommand() {
            Application.Current.Shutdown(0);
        }

        public void RunChangeBackgroundCommand() {
            var dialog = new ColorDialog();
            dialog.AllowFullOpen = true;
            dialog.AnyColor = true;
            dialog.FullOpen = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                Color color = dialog.Color;
            }
        }

        public void RunAboutCommand() {
            var menu = new AppAboutWindow();
            menu.ShowDialog();
        }
    }
}
