using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExRegistryHive
{
    public class ViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        string _ResultText = string.Empty;
        public string ResultText
        {
            set
            {
                _ResultText = value;
                OnPropertyChanged("ResultText");
            }
            get
            {
                return _ResultText;
            }
        }

        bool _LoadBTEnabled = true;
        public bool LoadBTEnabled
        {
            set
            {
                _LoadBTEnabled = value;
                OnPropertyChanged("LoadBTEnabled");
            }
            get
            {
                return _LoadBTEnabled;
            }
        }
        bool _GetBTEnabled = false;
        public bool GetBTEnabled
        {
            set
            {
                _GetBTEnabled = value;
                OnPropertyChanged("GetBTEnabled");
            }
            get
            {
                return _GetBTEnabled;
            }
        }
        bool _UnLoadBTEnabled = false;
        public bool UnLoadBTEnabled
        {
            set
            {
                _UnLoadBTEnabled = value;
                OnPropertyChanged("UnLoadBTEnabled");
            }
            get
            {
                return _UnLoadBTEnabled;
            }
        }
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();

            _vm = new ViewModel();
            DataContext = _vm;
        }

        string hivename = "Test";
        private void LoadHive_Click(object sender, RoutedEventArgs e)
        {
            string filename = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FilterIndex = 1;
            openFileDialog.FileName = "SOFTWARE";
            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.InitialDirectory = @"C:\";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                filename = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            bool rtn = ExRegistry.LoadHive(hivename, filename, ExRegistry.ExRegistryKey.HKEY_LOCAL_MACHINE);
            if (!rtn)
            {
                _vm.ResultText = "Failed Loading";
                return;
            }
            _vm.LoadBTEnabled = false;
            _vm.UnLoadBTEnabled = true;
            _vm.GetBTEnabled = true;
        }
        private void GetReg_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey baseKey = Registry.LocalMachine.OpenSubKey(hivename + "\\Microsoft\\Windows NT\\CurrentVersion");
            _vm.ResultText = hivename + "\\Microsoft\\Windows NT\\CurrentVersion" + Environment.NewLine + "ProductName : " + baseKey.GetValue("ProductName").ToString();
            baseKey.Close();
        }
        private void UnLoadHive_Click(object sender, RoutedEventArgs e)
        {
            bool rtn = ExRegistry.UnloadHive(hivename, ExRegistry.ExRegistryKey.HKEY_LOCAL_MACHINE);
            if (!rtn)
            {
                _vm.ResultText = "Failed UnLoading";
                return;
            }
            _vm.LoadBTEnabled = true;
            _vm.UnLoadBTEnabled = false;
            _vm.GetBTEnabled = false;
        }
    }
}
