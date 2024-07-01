using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using ChatClient.Net.IO;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; } // Command for opening file dialog
        public RelayCommand SendFileCommand { get; set; } // New command for sending files

        public string Username { get; set; }
        public string Message { get; set; }
        public UserModel SelectedUser { get; set; }

        private Server _server;
        private string _selectedFilePath; // To store selected file path

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.privateMsgReceivedEvent += PrivateMessageReceived;
            _server.userDisconnectEvent += RemoveUser;
            _server.fileReceivedEvent += HandleFile;
            _server.privateFileReceivedEvent += HandlePrivateFile;

            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            SendFileCommand = new RelayCommand(SendFile, CanSendFile);
            OpenFileCommand = new RelayCommand(OpenFileDialog); // Command to open file dialog
        }

        private bool CanSendMessage(object obj)
        {
            return !string.IsNullOrEmpty(Message);
        }

        private void SendMessage(object obj)
        {
            if (SelectedUser != null)
            {
                _server.SendPrivateMessageToUser(SelectedUser.UID, Message);
            }
            else
            {
                _server.SendMessageToServer(Message);
            }
        }

        private bool CanSendFile(object obj)
        {
            return !string.IsNullOrEmpty(_selectedFilePath);
        }

        private void SendFile(object obj)
        {
            if (!string.IsNullOrEmpty(_selectedFilePath))
            {
                // Read the file content
                byte[] fileData = File.ReadAllBytes(_selectedFilePath);

                if (SelectedUser != null)
                {
                    // Send file to all users
                    _server.SendFileToServer(_selectedFilePath);
                }
                else
                {
                    // Send file to all users
                    _server.SendFileToServer(_selectedFilePath);
                }
            }
        }

        private void OpenFileDialog(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                SendFile(null); // Call SendFile method to send the selected file
            }
        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.FirstOrDefault(x => x.UID == uid);
            if (user != null)
            {
                Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
            }
        }

        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }



        private void PrivateMessageReceived()
        {
            var senderUsername = _server.PacketReader.ReadMessage();
            var privateMsg = _server.PacketReader.ReadMessage();
            var msg = $"(Private) [{senderUsername}]: {privateMsg}";
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
        private void HandleFile() { 
             
            var filePath = _server.PacketReader.ReadMessage();
            string filecontent = File.ReadAllText(filePath);
            var msg = $"[{DateTime.Now}]: {Username}: File send of content\n: {filecontent}";
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }
      
        private void HandlePrivateFile()
        {
           
                // Read the recipient's UID and the file location
                var recipientUID = _server.PacketReader.ReadMessage();
                var filePath = _server.PacketReader.ReadMessage();
                string filecontent = File.ReadAllText(filePath);
                var msg = $"(Private file:) [{recipientUID}]: of content \n : {filecontent}";
                Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
              
        }

    }
}
