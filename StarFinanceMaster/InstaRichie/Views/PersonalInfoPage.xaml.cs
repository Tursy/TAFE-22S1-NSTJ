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
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        // Populates listview with current data
        public void Results()
        {
            conn.CreateTable<PersonalInfo>();
            var query = conn.Table<PersonalInfo>();
            PersonalInfoListView.ItemsSource = query.ToList();
        }

        public PersonalInfoPage()
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

        private async void AddPersonalInfo_Button_Click(object sender, RoutedEventArgs e)
        {
            // Attempts to insert a PersonalInfos object using the data in the textbox fields
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

                // Address
                if (Address_TextBox.Text == "")
                {
                    errors.Append("Address name cannot be left blank.\n");
                    invalid = true;
                }

                // PhoneNumber
                if (PhoneNumber_TextBox.Text == "")
                {
                    errors.Append("Phone number cannot be left blank.\n");
                    invalid = true;
                }

                // Display errors if there are any
                if (invalid)
                {
                    await new MessageDialog("Missing valid information:\n\n" + errors.ToString(), "Error").ShowAsync();
                }

                // Otherwise, create the PersonalInfo object and add it to the database
                else
                {
                    PersonalInfo cd = new PersonalInfo();

                    cd.CustomerID = 0; // Assigns an ID automatically
                    cd.FirstName = FirstName_TextBox.Text;
                    cd.LastName = LastName_TextBox.Text;
                    cd.Address = Address_TextBox.Text;
                    cd.PhoneNumber = PhoneNumber_TextBox.Text.ToString();

                    conn.Insert(cd);

                    await new MessageDialog("PersonalInfos object added successfully.", "Update Successful").ShowAsync();
                }

                Results();
            }
            catch (Exception exception)
            {
                await new MessageDialog("Exception: \n\n" + exception, "Error").ShowAsync();
            }
        }

        private async void UpdatePersonalInfo_Button_Click(object sender, RoutedEventArgs e)
        {
            // Attempts to update a PersonalInfo object using the data in the textbox fields
            try
            {
                StringBuilder errors = new StringBuilder();
                bool invalid = false;
                int i; // Needed for the TryParse method

                // CustomerID
                if (CustomerID_TextBox.Text == "" || !int.TryParse(CustomerID_TextBox.Text, out i))
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

                // Address
                if (Address_TextBox.Text == "")
                {
                    errors.Append("Address cannot be left blank.\n");
                    invalid = true;
                }

                // PhoneNumber
                if (PhoneNumber_TextBox.Text == "" || !int.TryParse(PhoneNumber_TextBox.Text, out i))
                {
                    errors.Append("Phone number must have an integer value.\n");
                    invalid = true;
                }

                // Display errors if there are any
                if (invalid)
                {
                    await new MessageDialog("Missing valid information:\n\n" + errors.ToString(), "Error").ShowAsync();
                }
                else
                {
                    // Update PersonalInfo object with the specified ID
                    conn.Query<PersonalInfo>("UPDATE PersonalInfo SET FirstName='" + FirstName_TextBox.Text
                        + "', LastName='" + LastName_TextBox.Text
                        + "', Address='" + Address_TextBox.Text
                        + "', PhoneNumber='" + PhoneNumber_TextBox.Text.ToString()
                        + "' WHERE CustomerID='" + int.Parse(CustomerID_TextBox.Text) + "'");

                    await new MessageDialog("PersonalInfo updated successfully.", "Update Successful").ShowAsync();
                }
                Results();
            }
            catch (Exception exception)
            {
                await new MessageDialog("Exception: \n\n" + exception, "Error").ShowAsync();
            }
        }

        private async void DeletePersonalInfo_Button_Click(object sender, RoutedEventArgs e)
        {
            // Attempts to delete a PersonalInfo object using the data in the CustomerID textbox
            try
            {
                int i; // Needed for the TryParse() method below

                // If the ID field is empty or it is not a number
                if (CustomerID_TextBox.Text == "" || !int.TryParse(CustomerID_TextBox.Text, out i))
                {
                    await new MessageDialog("No valid ID entered.", "Error").ShowAsync();
                }
                else
                {
                    // Delete PersonalInfo object with the specified ID
                    conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE CustomerID='"
                        + int.Parse(CustomerID_TextBox.Text) + "'");

                    await new MessageDialog("PersonalInfo object deleted successfully.", "Update Successful").ShowAsync();

                    // Clear the field
                    CustomerID_TextBox.Text = "";
                    FirstName_TextBox.Text = "";
                    LastName_TextBox.Text = "";
                    Address_TextBox.Text = "";
                    PhoneNumber_TextBox.Text = "";
                }
                Results();
            }
            catch (Exception exception)
            {
                await new MessageDialog("Exception: \n\n" + exception, "Error").ShowAsync();
            }
        }

        // Updates the textfields using data pulled from a selected listview item
        private void PersonalInfoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (PersonalInfo)PersonalInfoListView.SelectedItem;

            // This condition clause catches the selection changed event that triggers after deleting the
            // active selection
            if (selectedItem != null)
            {
                // Populate the fields with the data from the list
                CustomerID_TextBox.Text = selectedItem.CustomerID.ToString();
                FirstName_TextBox.Text = selectedItem.FirstName;
                LastName_TextBox.Text = selectedItem.LastName;
                Address_TextBox.Text = selectedItem.Address;
                PhoneNumber_TextBox.Text = selectedItem.PhoneNumber.ToString();
            }
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }
    }
}