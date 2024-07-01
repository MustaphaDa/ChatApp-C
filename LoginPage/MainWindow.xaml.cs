using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace LoginPage
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login Successful!");

                // Open server and client windows
                OpenServerAndClientWindows(username);

                // Close the current window if needed
                 Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

        /* private bool AuthenticateUser(string username, string password)
         {
             try
             {
                 // Read all lines from the file
                 string[] lines = File.ReadAllLines("credentials.txt.txt");

                 // Iterate through each line to check for username and password
                 foreach (string line in lines)
                 {
                     // Split the line into username and password
                     string[] parts = line.Split(',');

                     // Check if the username and password match
                     if (parts.Length == 2 && parts[0] == username && parts[1] == password)
                     {
                         return true; // Authentication successful
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show("Error authenticating user: " + ex.Message);
             }

             return false; // Authentication failed
         }*/


        private bool AuthenticateUser(string username, string password)
        {
            
            Dictionary<string, string> credentials = new Dictionary<string, string>
        {
        {"student", "student"},
        {"admin", "admin"},
        {"admin1", "admin1"}
         };

            // Check if the provided username exists and the password matches
            if (credentials.ContainsKey(username) && credentials[username] == password)
            {
                return true; // Authentication successful
            }

            return false; // Authentication failed
        }




        private void OpenServerAndClientWindows(string username)
        {
            // Start
            StartChatServer();

            // Start ChatClient
            StartChatClient(username);
        }

        private void StartChatServer()
        {
            try
            {
                string serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatServer.exe");
                Process.Start(serverPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting ChatServer: " + ex.Message);
            }
        }

        private void StartChatClient(string username)
        {
            try
            {
                string clientPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatClient.exe");
                Process.Start(clientPath, username);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting ChatClient: " + ex.Message);
            }
        }

    }
}

