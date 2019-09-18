using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for BookWindow.xaml
    /// </summary>
    public partial class BookWindow : Window
    {
        BindingModel BM_;
        MainWindow mWin_;
        Mode mode_;
        public BookWindow(Mode mode, BindingModel bm, MainWindow mWin)
        {
            mode_ = mode;
            BM_ = bm;
            this.DataContext = BM_;
            mWin_ = mWin;

            InitializeComponent();
        }

        private async void BtnSubmit_ClickAsync(object sender, RoutedEventArgs e)
        {
            bool isSuccess = false;
            if (mode_ == Mode.Post)
            {
                var msg = await BM_.postBookAsync();
                isSuccess = msg.IsSuccessStatusCode;
            }
            else if(mode_==Mode.Put)
            {
                var msg = await BM_.putBookAsync();
                isSuccess = msg.IsSuccessStatusCode;

            }
            if (isSuccess)
            {
                mWin_.setStatus("Operation Succeed!");
                BM_.getBooksAsync();
            }
            else
            {
                mWin_.setStatus("Operation Fail!");
            }
            this.Close();

        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "PDF|*.pdf|MOBI|*.mobi";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                BM_.theBook.PathUrl = dlg.FileName;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mWin_.bWin = null;
        }
    }
}
