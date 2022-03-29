using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using StartFinance.Models;
using SQLite.Net;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }
        public void Results()
        {
            // Creating table
            conn.CreateTable<NewAppointments>();
            var query = conn.Table<NewAppointments>();
            AppointmentList.ItemsSource = query.ToList();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // validating that necessary fields are not left null

                // checks if appointment name is null
                if (EventNameBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Event name not entered", "Oops..!");
                    await dialog.ShowAsync();
                }

                // checks if appointment location is null
                if (LocationBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Event location not entered", "Oops..!");
                    await dialog.ShowAsync();
                }

                // checks if appointment date is null
                else if (appointmentDatePicker.SelectedDate == null)
                {
                    MessageDialog dialog = new MessageDialog("Appointment date not entered", "Oops..!");
                    await dialog.ShowAsync();
                }

                // checks if appointment start time is null
                else if (appStartTimePicker.SelectedTime == null)
                {
                    MessageDialog dialog = new MessageDialog("Appointment start time not entered", "Oops..!");
                    await dialog.ShowAsync();
                }

                // checks if appointment end time is null
                else if (appEndTimePicker.SelectedTime == null)
                {
                    MessageDialog dialog = new MessageDialog("Appointment end time not entered", "Oops..!");
                    await dialog.ShowAsync();
                }


                else
                {

                    // date picker also grabs current time/run time of the app and so when you add the time from the time picker it results in weird time issues
                    // .Date.Date strips the time from the date picker, resetting to midnight. The time from the time picker is then addded and converted.

                    // Should work
                    DateTime startDateTime = appointmentDatePicker.Date.Date.Add(appStartTimePicker.Time).ToLocalTime();
                    DateTime endDateTime = appointmentDatePicker.Date.Date.Add(appEndTimePicker.Time).ToLocalTime();

                    //startDateTime = startDateTime.ToLocalTime(); //.TimeSpan,
                    //endDateTime = endDateTime.ToLocalTime();


                    // Inserts the data
                    conn.Insert(new NewAppointments()
                    {
                        EventName = EventNameBox.Text,
                        Location = LocationBox.Text,
                        //EventDate = eventDateTime,
                        StartTime = startDateTime, //.ToLocalTime(), //.TimeSpan,
                        EndTime = endDateTime //.ToLocalTime() //.Time,
                    });
                    Results();

                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the event name or entered invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Event Name already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("An unknown eror has occured", "Oops..!");
                    await dialog.ShowAsync();
                }

            }
        }

        // Clears the fields
        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string delSelection = ((NewAppointments)AppointmentList.SelectedItem).EventName;
                if (delSelection == null)
                {
                    MessageDialog dialog = new MessageDialog("You have not selected an item to delete", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<NewAppointments>();
                    var query1 = conn.Table<NewAppointments>();
                    var query3 = conn.Query<NewAppointments>("DELETE FROM NewAppointments WHERE EventName ='" + delSelection + "'");
                    //var query4 = conn.Query<NewAppointments>("SELECT * FROM NewAppointments");

                    AppointmentList.ItemsSource = query1.ToList();
                    //AppointmentList.ItemsSource = query4.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("You have not selected an item to delete", "Oops..!");
                await dialog.ShowAsync();
            }

            
        }

    }
}
