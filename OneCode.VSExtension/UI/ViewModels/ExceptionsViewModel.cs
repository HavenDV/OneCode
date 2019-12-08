using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OneCode.VsExtension.UI.ViewModels
{
    public sealed class ExceptionsViewModel
    {
        public ObservableCollection<Exception> Values { get; set; }

        public DelegateCommand<Exception> ShowCommand { get; }

        public ExceptionsViewModel(List<Exception> exceptions)
        {
            exceptions = exceptions ?? throw new ArgumentNullException(nameof(exceptions));

            Values = new ObservableCollection<Exception>(exceptions);

            ShowCommand = new DelegateCommand<Exception>(OnShow);
        }

        private static void OnShow(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            MessageBox.Show(exception.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
