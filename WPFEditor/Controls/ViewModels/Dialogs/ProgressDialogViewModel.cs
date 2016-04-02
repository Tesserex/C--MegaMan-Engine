using System;
using System.Diagnostics;
using System.Windows;

namespace MegaMan.Editor.Controls.ViewModels.Dialogs
{
    public class ProgressDialogViewModel : ViewModelBase
    {
        private Stopwatch _stopwatch;
        private Window _window;

        public ProgressDialogViewModel(Window window, Progress<ProgressDialogState> reporter, Stopwatch stopwatch = null)
        {
            reporter.ProgressChanged += ReportProgress;
            _window = window;

            if (stopwatch != null)
            {
                window.Hide();
                _stopwatch = stopwatch;
            }
        }

        public void ReportProgress(object sender, ProgressDialogState progress)
        {
            if (progress.Title != null)
                Title = progress.Title;

            if (progress.Description != null)
                Description = progress.Description;

            ProgressValue = progress.ProgressPercentage;

            if (_stopwatch != null && _stopwatch.ElapsedMilliseconds > 500 && _window.IsEnabled)
            {
                _window.Show();
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private int _progress;
        public int ProgressValue
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visibility;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }
    }

    public class ProgressDialogState
    {
        public int ProgressPercentage { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
