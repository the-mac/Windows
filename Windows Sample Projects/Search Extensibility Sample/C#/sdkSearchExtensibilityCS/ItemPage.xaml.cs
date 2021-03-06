/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using System.Windows.Media.Imaging;

namespace sdkSearchExtensibilityCS
{
    public partial class ItemPage : PhoneApplicationPage
    {
        // Constructor
        public ItemPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data.
            DataContext = App.ItemViewModel;
            this.Loaded += new RoutedEventHandler(ItemPage_Loaded);
        }

        // Load data from the ItemViewModel.
        private void ItemPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Temporary text for the TextBlock that will show the name of the product if available.
            string productName = "Product not found";

            // Icon to indicate if the product is recalled.
            BitmapImage iconImage = new BitmapImage();

            // Specifies that the icon should not be cached or created in the background.
            iconImage.CreateOptions = BitmapCreateOptions.None;

            // Text to describe if the product is recalled.
            string caption;


            // Loop through the parameters in the Search Extras deep link URI.
            foreach (string strKey in this.NavigationContext.QueryString.Keys)
            {
                // Look for the “ProductName” parameter.
                if (strKey == "ProductName")
                {
                    // Extract the product name. 
                    productName = this.NavigationContext.QueryString[strKey];

                    // Calls helper method to determine if product is recalled.
                    if (IsRecalled(productName))
                    {
                        // Set icon and caption to indicate that product is recalled.
                        iconImage.UriSource = new Uri("X.png", UriKind.Relative);
                        caption = "This product was recently recalled! Proceed with caution!!";
                    }
                    else
                    {
                        // Set icon and caption to indicate that the product is not recalled. 
                        iconImage.UriSource = new Uri("Check.png", UriKind.Relative);
                        caption = "No recalls found! You can buy this product with confidence :)";

                    }

                    // Set ViewModel icon and caption to indicate if product is recalled.
                    App.ItemViewModel.IconSource = iconImage;
                    App.ItemViewModel.Caption = caption;
                }
            }


            // Bind the product name to the LaunchingParams property.
            App.ItemViewModel.LaunchingParams = productName;
        }


        // Placeholder method for determining if a product is recalled (results are randomly selected).
        private bool IsRecalled(string productName)
        {
            // A pseudo-random number generator, based on the length of the product name.
            Random rand = new Random(productName.Length);

            /* 
             * Integer generated by the pseudo-random number generator, 
             * bit-shifted by the bitwise operator '&' with the integer 1, 
             * to determine if the number is even or odd.
             */
            int test = rand.Next() & 1;


            // If the random number is odd, product is recalled.
            if (test == 1)
            {
                return true;
            }
            // If the random number is even, product is not recalled.
            else
            {
                return false;
            }
        }

        // OnNavigatedTo implementation.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

        }
    }
}
