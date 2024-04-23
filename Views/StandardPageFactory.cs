using ETCRegionManagementSimulator.Models;
using ETCRegionManagementSimulator.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ETCRegionManagementSimulator.Views
{

    public class StandardPageFactory
    {

        private readonly IServiceProvider _serviceProvider;

        public StandardPageFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// TODO: Create other kinds of Pages Here

        /// Only create a StandardClientPage for demostraction
        public StandardClientPage CreateStandardPage(ClientView clientView)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var page = scope.ServiceProvider.GetRequiredService<StandardClientPage>();
                (page.DataContext as ClientViewModel).ClientView = clientView;
                return page;
            }
        }
    }
}
