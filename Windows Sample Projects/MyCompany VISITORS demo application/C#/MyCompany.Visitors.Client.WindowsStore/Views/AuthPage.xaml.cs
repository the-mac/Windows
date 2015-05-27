﻿namespace MyCompany.Visitors.Client.WindowsStore.Views
{
    using MyCompany.Visitors.Client.WindowsStore.ViewModel;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuthPage : Page
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AuthPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var vm = (this.DataContext as VMAuthentication);
            vm.AuthenticateUser();
        }
    }
}
