using TPCWare.CarLocator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TPCWare.CarLocator.ViewModels
{
    public class BaseViewModel: ObservableObject
    {
        bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value, onChanged: () => OnPropertyChanged(nameof(IsNotBusy)));
        }

        public bool IsNotBusy => !IsBusy;

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        internal event Func<string, Task> DoDisplayAlert;

        public Task DisplayAlertAsync(string message)
        {
            return DoDisplayAlert?.Invoke(message) ?? Task.CompletedTask;
        }

        public BaseViewModel()
        {
        }

    }
}
