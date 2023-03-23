using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using FileScannerLibrary.Intreface;
using Microsoft.Win32;


namespace WpfFileScannerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker _worker;
        private readonly IFileScanner _fileScanner;
        private int _totalFiles;
        private int _processedFiles;
        private string _sourcePath;
        private string _destinationPath;
        private bool _compressFielCheck;

        public MainWindow()
        {
            InitializeComponent();
            _worker = new BackgroundWorker();
            _fileScanner =  FileScannerLibrary.Classes.FileScanner.Instance;

            _worker.WorkerReportsProgress = true;
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void BrowseSourceButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                CheckFileExists = false,
                FileName = "Select Folder",
                Filter = "Folders|no.files"
            };

            if (dialog.ShowDialog() == true)
            {
                SourcePathTextBox.Text = dialog.FileName;
            }
        }

        private void BrowseDestinationButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                CheckFileExists = false,
                FileName = "Select Folder",
                Filter = "Folders|no.files"
            };

            if (dialog.ShowDialog() == true)
            {
                DestinationPathTextBox.Text = dialog.FileName;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SourcePathTextBox.Text) && !string.IsNullOrWhiteSpace(DestinationPathTextBox.Text))
            {
                _sourcePath = SourcePathTextBox.Text;
                _destinationPath = DestinationPathTextBox.Text;
                _compressFielCheck = CompressCheckBox.IsChecked ?? false;
                _totalFiles = CountFiles(SourcePathTextBox.Text);
                StartButton.IsEnabled = false;
                _worker.RunWorkerAsync();
            }
        }

        private int CountFiles(string sourcePath)
        {
            return Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Length;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int totalOperations = _totalFiles * 2; // One for processing, one for copying
            int completedOperations = 0;

            _fileScanner.FileProcessed += (s, args) =>
            {
                completedOperations++;
                int progressPercentage = (int)((double)completedOperations / totalOperations * 100);
                _worker.ReportProgress(progressPercentage);
            };

            _fileScanner.FileCopied += (s, args) =>
            {
                completedOperations++;
                int progressPercentage = (int)((double)completedOperations / totalOperations * 100);
                _worker.ReportProgress(progressPercentage);
            };

            _fileScanner.ScanAndOrganizeFiles(_sourcePath, _destinationPath, _compressFielCheck);
        }
         

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Scanning and organizing process completed!", "Completed", MessageBoxButton.OK, MessageBoxImage.Information);
            StartButton.IsEnabled = true;
        }
    }
}