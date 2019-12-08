using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;
using OneCode.VsExtension.Services;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OneCode.VsExtension.UI.ViewModels
{
    public sealed class ExceptionsViewModel
    {
        private ExceptionsService ExceptionsService { get; }


        public ObservableCollection<Exception> Values { get; set; }

        public DelegateCommand<Exception> ShowCommand { get; }

        public ExceptionsViewModel(ExceptionsService exceptionsService)
        {
            ExceptionsService = exceptionsService ?? throw new ArgumentNullException(nameof(exceptionsService));

            Values = new ObservableCollection<Exception>(ExceptionsService.Exceptions);

            ShowCommand = new DelegateCommand<Exception>(OnShow);
        }

        private void OnShow(Exception value)
        {
            if (value == null)
            {
                return;
            }

            try
            {
                MessageBox.Show(value.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }
    }
}
