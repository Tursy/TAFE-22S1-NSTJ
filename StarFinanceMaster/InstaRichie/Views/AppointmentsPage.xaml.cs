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
            conn.CreateTable<Appointments>();
            var query = conn.Table<Appointments>();
            AppointmentList.ItemsSource = query.ToList();
        }

        private async void AddAppointment_Click(object sender, RoutedEventArgs e)
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
                else if (LocationBox.Text.ToString() == "")
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

                    DateTime startDateTime = appointmentDatePicker.Date.Date.Add(appStartTimePicker.Time).ToLocalTime();
                    DateTime endDateTime = appointmentDatePicker.Date.Date.Add(appEndTimePicker.Time).ToLocalTime();

                    // Inserts the data
                    conn.Insert(new Appointments()
                    {
                        EventName = EventNameBox.Text,
                        Location = LocationBox.Text,
                        StartTime = startDateTime,
                        EndTime = endDateTime
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

        private async void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string delSelection = ((Appointments)AppointmentList.SelectedItem).AppointmentID.ToString();

                if (delSelection == null)
                {
                    MessageDialog dialog = new MessageDialog("You have not selected an item to delete", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Query<Appointments>("DELETE FROM Appointments WHERE AppointmentID ='" + delSelection + "'");

                    Results();
                }
            }

            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is NullReferenceException)
                {
                    MessageDialog dialog = new MessageDialog("You have not selected an item to delete", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else
                {
                    MessageDialog dialog = new MessageDialog("An unknown eror has occured", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void UpdateAppointment_Click(object sender, RoutedEventArgs e)
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
                else if (LocationBox.Text.ToString() == "")
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
                    // had issues with time for updating, couldn't fix, current solution deletes selected item and inserts new
                    
                    string delSelection = ((Appointments)AppointmentList.SelectedItem).AppointmentID.ToString();
                    conn.Query<Appointments>("DELETE FROM Appointments WHERE AppointmentID ='" + delSelection + "'");

                    DateTime startDateTime = appointmentDatePicker.Date.Date.Add(appStartTimePicker.Time).ToLocalTime();
                    DateTime endDateTime = appointmentDatePicker.Date.Date.Add(appEndTimePicker.Time).ToLocalTime();

                    // Inserts the data
                    conn.Insert(new Appointments()
                    {
                        EventName = EventNameBox.Text,
                        Location = LocationBox.Text,
                        StartTime = startDateTime,
                        EndTime = endDateTime
                    });
                    Results();

                    // some of my last/last attempts at getting it working the original way

                    // grabs appointment id of selected item, sets variables to prepare for database insertion
                    //string updateSelection = ((Appointments)AppointmentList.SelectedItem).AppointmentID.ToString();

                    //DateTime startDateTime = appointmentDatePicker.Date.Date.Add(appStartTimePicker.Time);//.ToLocalTime();
                    //DateTime endDateTime = appointmentDatePicker.Date.Date.Add(appEndTimePicker.Time);//.ToLocalTime();

                    //String EventName = EventNameBox.Text;
                    //String Location = LocationBox.Text;
                    //String str_StartTime = startDateTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                    //String str_EndTime = endDateTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");

                    //DateTime StartTime = startDateTime;
                    //DateTime EndTime = endDateTime;

                    // updates database
                    //conn.Query<Appointments>("UPDATE Appointments SET EventName='" + EventName +
                    //    "', Location='" + Location +
                    //    "', StartTime='" + str_StartTime + //StartTime
                    //    "', EndTime ='" + str_EndTime + //EndTime
                    //    "' WHERE AppointmentID ='" + int.Parse(updateSelection) + "'");

                    //Results();
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
                    //MessageDialog dialog = new MessageDialog("Event Name already exists, Try a different name", "Oops..!");
                    MessageDialog dialog = new MessageDialog("Event Name already exists, Try a different name", ex.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("An unknown eror has occured", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

    }
}
