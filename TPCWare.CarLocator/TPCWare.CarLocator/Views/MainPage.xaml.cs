using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPCWare.CarLocator.Models;
using TPCWare.CarLocator.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TPCWare.CarLocator.Views
{
    public partial class MainPage : BasePage
    {
        private MainViewModel vm;

        public MainPage()
        {
            InitializeComponent();
            vm = BindingContext as MainViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (vm != null)
            {
                await vm.UpdateMapAsync(MyMap);
            }
        }

        private async void SetPosition_Clicked(object sender, EventArgs e)
        {
            if (vm != null)
            {
                await vm.SetNewCarLocationAsync(MyMap);     
            }
        }
        
    }
}
