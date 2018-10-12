using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FWO;

namespace FWOClientTestGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //clients
        GTLClientServerComm ServerComm;
        GTLClientFWO FWOClient;
        GTLClientDescriptions DSCClient;

        public void StartApp()
        {
            //start the Clients
            ServerComm = new GTLClientServerComm("127.0.0.1", (int)ServerPorts.ServerCommService, "ServerCommService");
            ServerComm.Start();
        }

        public async void Checkservices()
        {
            Console.WriteLine("Waiting for servercomm...");
            //wait for the server to come online
            await ServerComm.ServerConnectedTask(true);

            await Task.Delay(1000);

            Console.WriteLine(ServerComm.Reply.ToolVersion);
        }

        public MainWindow()
        {
            InitializeComponent();

            StartApp();
            Checkservices();
        }
    }
}
