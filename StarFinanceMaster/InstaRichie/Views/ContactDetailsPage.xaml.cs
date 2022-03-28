using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetailsPage : Page
    {
        SQLiteConnection conn;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        // Populates listview with current data
        public void Results()
        {
            conn.CreateTable<ContactDetails>();
            var query = conn.Table<ContactDetails>();
            ContactDetailsListView.ItemsSource = query.ToList();
        }

        public ContactDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void AddContactDetails_Button_Click(object sender, RoutedEventArgs e)
        {
            // Attempts to insert a ContactDetails object using the data in the textbox fields
            try
            {
                StringBuilder errors = new StringBuilder();
                bool invalid = false;
                int i; // Needed for the TryParse method

                // FirstName
                if (FirstName_TextBox.Text == "")
                {
                    errors.Append("First name cannot be left blank.\n");
                    invalid = true;
                }

                // LastName
                if (LastName_TextBox.Text == "")
                {
                    errors.Append("Last name cannot be left blank.\n");
                    invalid = true;
                }

                // CompanyName
                if (CompanyName_TextBox.Text == "")
                {
                    errors.Append("Company name cannot be left blank.\n");
                    invalid = true;
                }

                // MobilePhone
                if (MobilePhone_TextBox.Text == "")
                {
                    errors.Append("Mobile phone number cannot be left blank.\n");
                    invalid = true;
                }

                // Display errors if there are any
                if (invalid)
                {
                    await new MessageDialog("Missing valid information:\n\n" + errors.ToString(), "Error").ShowAsync();
                }

                // Otherwise, create the ContactDetails object and add it to the database
                else
                {
                    ContactDetails cd = new ContactDetails();

                    cd.ContactID = 0; // Assigns an ID automatically
                    cd.FirstName = FirstName_TextBox.Text;
                    cd.LastName = LastName_TextBox.Text;
                    cd.CompanyName = CompanyName_TextBox.Text;
                    cd.MobilePhone = MobilePhone_TextBox.Text;

                    conn.Insert(cd);

                    await new MessageDialog("ContactDetails object added successfully.", "Update Successful").ShowAsync();
                }

                Results();
            }
            catch (Exception exception) 
            {
                await new MessageDialog("Exception: \n\n" + exception, "Error").ShowAsync();
            }
        }

        private async void UpdateContactDetails_Button_Click(object sender, RoutedEventArgs e)
        {
            // Attempts to update a ContactDetails object using the data in the textbox fields
            try
            {
                StringBuilder errors = new StringBuilder();
                bool invalid = false;
                int i; // Needed for the TryParse method

                // ContactDetailsID
                if (ContactDetailsID_TextBox.Text == "" || !int.TryParse(ContactDetailsID_TextBox.Text, out i))
                {
                    errors.Append("ID must have an integer value.\n");
                    invalid = true;
                }

                // FirstName
                if (FirstName_TextBox.Text == "")
                {
                    errors.Append("First name cannot be left blank.\n");
                    invalid = true;
                }

                // LastName
                if (LastName_TextBox.Text == "")
                {
                    errors.Append("Last name cannot be left blank.\n");
                    invalid = true;
                }

                // CompanyName
                if (CompanyName_TextBox.Text == "")
                {
                    errors.Append("Company name cannot be left blank.\n");
                    invalid = true;
                }

                // MobilePhone
                if (MobilePhone_TextBox.Text == "" || !int.TryParse(MobilePhone_TextBox.Text, out i))
                {
                    errors.Append("Mobile phone number must have an integer value.\n");
                    invalid = true;
                }

                // Display errors if there are any
                if (invalid)
                {
                    await new MessageDialog("Missing valid information:\n\n" + errors.ToString(), "Error").ShowAsync();
                }
                else
                {
                    // Update ContactDetails object with the specified ID
                    conn.Query<ContactDetails>("UPDATE ContactDetails SET FirstName='" +  FirstName_TextBox.Text
                        + "', LastName='" + LastName_TextBox.Text
                        + "', CompanyName='" + CompanyName_TextBox.Text
                        + "', MobilePhone='" + MobilePhone_TextBox.Text
                        + "' WHERE ContactID='" + int.Parse(ContactDetailsID_TextBox.Text) + "'");

                    await new MessageDialog("ContactDetail updated successfully.", "Update Successful").ShowAsync();
                }
                Results();
            }
            catch (Exception exception)
            {
                await new MessageDialog("Exception: \n\n" + exception, "Error").ShowAsync();
            }
        }

        private async void DeleteContactDetails_Button_Click(object sender, RoutedEventArgs e)
        {
            // Attempts to delete a ContactDetails object using the data in the ContactDetailsID textbox
            try
            {
                int i; // Needed for the TryParse() method below
                
                // If the ID field is empty or it is not a number
                if (ContactDetailsID_TextBox.Text == "" || !int.TryParse(ContactDetailsID_TextBox.Text, out i))
                {
                    await new MessageDialog("No valid ID entered.", "Error").ShowAsync();
                }
                else
                {
                    // Delete ContactDetails object with the specified ID
                    conn.Query<ContactDetails>("DELETE FROM ContactDetails WHERE ContactID='" 
                        + int.Parse(ContactDetailsID_TextBox.Text) + "'");

                    await new MessageDialog("ContactDetail object deleted successfully.", "Update Successful").ShowAsync();

                    // Clear the field
                    ContactDetailsID_TextBox.Text = "";
                    FirstName_TextBox.Text = "";
                    LastName_TextBox.Text = "";
                    CompanyName_TextBox.Text = "";
                    MobilePhone_TextBox.Text = "";
                }
                Results();
            }
            catch (Exception exception)
            {
                await new MessageDialog("Exception: \n\n" + exception, "Error").ShowAsync();
            }
        }

        // Updates the textfields using data pulled from a selected listview item
        private void ContactDetailsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ContactDetails)ContactDetailsListView.SelectedItem;

            // This condition clause catches the selection changed event that triggers after deleting the
            // active selection
            if (selectedItem != null) 
            {
                // Populate the fields with the data from the list
                ContactDetailsID_TextBox.Text = selectedItem.ContactID.ToString();
                FirstName_TextBox.Text = selectedItem.FirstName;
                LastName_TextBox.Text = selectedItem.LastName;
                CompanyName_TextBox.Text = selectedItem.CompanyName;
                MobilePhone_TextBox.Text = selectedItem.MobilePhone;
            }
        }
    }
}
