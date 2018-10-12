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



namespace FWOClientTest_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            InitializeComponent();
            StartApplication();
        }


        async void StartApplication()
        {
            FWOClientStarter.StartClients(45010, "127.0.0.1");
            await FWOClientStarter.WaitForClientConnectTask();

            log.Debug("Client Started!!!");

            await FWOClientStarter.WaitForClientDisconnectTask();

            log.Debug("Server disconnected");
        }
    }
}
