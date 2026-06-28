using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Earthquake_WPFApp {
    class MainViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        public MenuCommand MenuCommand { get; private set; }

        public MainViewModel() {
            MenuCommand = new MenuCommand(this);
        }

    }
}
