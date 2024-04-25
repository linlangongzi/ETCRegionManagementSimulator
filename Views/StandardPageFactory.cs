using ETCRegionManagementSimulator.Interfaces;
using ETCRegionManagementSimulator.Models;
using ETCRegionManagementSimulator.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI.Xaml.Controls;

namespace ETCRegionManagementSimulator.Views
{
    public class StandardPageFactory : IStandardPageFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public StandardPageFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Page CreateStandardPage(object parameter)
        {
            if (parameter is ClientView clientView)
            {
                Type pageType = DeterminePageType(clientView);
                Page page = (Page)_serviceProvider.GetRequiredService(pageType);

                if (page.DataContext is ClientViewModel viewModel)
                {
                    viewModel.ClientView = clientView; // Ensure ViewModel is properly initialized
                }
                else
                {
                    page.DataContext = new ClientViewModel(clientView);
                }

                return page;
            }
            // Return a default empty page instead if parameter is something else
            return new StandardClientPage();
        }

        private Type DeterminePageType(ClientView clientView)
        {
            if (clientView.Type != ClientView.ClientViewType.Standard)
            {
                return typeof(SpecialClientPage);
            }
            else
            {
                return typeof(StandardClientPage);
            }
        }
    }
}
