using ConsoleClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class BindingModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public ObservableCollection<ViewBook> Books { get; set; }

        public ViewBook theBook { set; get; }


        CoreConsoleClient consoleClient;

        public BindingModel()
        {
            Books = new ObservableCollection<ViewBook>();
            theBook = new ViewBook();
            consoleClient = new CoreConsoleClient("https://localhost:44321/api/ApiBook");
        }

        public async void getBooksAsync()
        {
            IEnumerable<ViewBook> books = await consoleClient.GetBookList();
            Books.Clear();
            foreach(var book in books)
            {
                Books.Add(book);
            }
        }

        public async Task<HttpResponseMessage> postBookAsync()
        {
            return await consoleClient.CreateBook(theBook);
        }

        public async Task<HttpResponseMessage> putBookAsync()
        {
            return await consoleClient.EditBook(theBook);
        }

        public async Task<HttpResponseMessage> deleteBookAsync()
        {
            return await consoleClient.DeleteBook(theBook.BookId);
        }


    }
}
