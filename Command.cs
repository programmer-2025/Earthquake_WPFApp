using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Earthquake_WPFApp {
    internal class Command : ICommand {

        private readonly Action action;

        public Command(Action action) {
            this.action = action;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) {
            return true;
        }

        public void Execute(object? parameter) {
            action();
        }

    }
}
