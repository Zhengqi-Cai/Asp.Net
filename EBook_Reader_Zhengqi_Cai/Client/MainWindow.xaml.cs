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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConsoleClient;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ViewBook> Books=new List<ViewBook>();
        BindingModel BM = new BindingModel();
        public BookWindow bWin; //maybe add listener is better choice
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            this.DataContext = BM;
            //GenreNames g;
            //var r = Enum.TryParse<GenreNames>("Art", out g);
            BM.getBooksAsync();
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            BM.theBook = new ViewBook() { UploaderName="zhengqi_cai@outlook.com"};

            if (bWin == null)
            {
                bWin = new BookWindow(Mode.Post,BM, this);
            }
            bWin.Show();

        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = lstBooks.SelectedItem;
            if (selected == null)
            {
                setStatus("Please select a book to edit!");
            }
            else
            {
                ViewBook vBook = selected as ViewBook;
                vBook.PathUrl = "";
                BM.theBook = vBook;
                if (bWin == null)
                {
                    bWin = new BookWindow(Mode.Put,BM, this);
                }
                bWin.Show();
            }
        }

        private async void Delete_ClickAsync(object sender, RoutedEventArgs e)
        {
            var selected = lstBooks.SelectedItem;
            if (selected == null)
            {
                setStatus("Please select a book to edit!");
            }
            else
            {
                ViewBook vBook = selected as ViewBook;
                BM.theBook = vBook;
                var msg = await BM.deleteBookAsync();
                if (msg.IsSuccessStatusCode)
                {
                    setStatus("Operation Succeed!");
                    BM.getBooksAsync();
                }
                else
                {
                    setStatus("Operation Fail!");

                }

            }
        }

        public void setStatus(string s)
        {
            statusBarText.Text = s;
        }
    }
}
