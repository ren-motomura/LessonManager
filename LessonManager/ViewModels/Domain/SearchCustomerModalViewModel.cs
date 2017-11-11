using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LessonManager.Models;
using LessonManager.Commands;

namespace LessonManager.ViewModels.Domain
{
    class SearchCustomerModalViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public SearchCustomerModalViewModel()
        {
            SearchFromNameOrDescriptionCommand = new DelegateCommand();
            SearchFromNameOrDescriptionCommand.ExecuteHandler = SearchFromNameOrDescriptionCommandExecute;

            SearchConditionRemoveCommand = new DelegateCommand();
            SearchConditionRemoveCommand.ExecuteHandler = SearchConditionRemoveCommandExecute;

            Customers = Storage.GetInstance().Customers;
        }

        private ImmutableList<Customer> customers_;
        public ImmutableList<Customer> Customers
        {
            get { return customers_; }
            set
            {
                customers_ = value;
                RaisePropertyChanged();
            }
        }

        private Customer selectedCustomer_;
        public Customer SelectedCustomer
        {
            get { return selectedCustomer_; }
            set
            {
                selectedCustomer_ = value;
                RaisePropertyChanged();
            }
        }
        public bool IsSelected { get { return SelectedCustomer != null; } }

        public DelegateCommand SearchFromNameOrDescriptionCommand { get; set; }
        private void SearchFromNameOrDescriptionCommandExecute(object parameter)
        {
            var searchBox = parameter as Views.Domain.SearchBox;
            string searchText = searchBox.SearchText;
            if (searchText == null || searchText == "")
            {
                return;
            }

            Customers = Storage.GetInstance().Customers;
            Customers = Customers.FindAll((Customer c) =>
            {
                return c.Name.Contains(searchText) || c.Description.Contains(searchText);
            });
        }

        public DelegateCommand SearchConditionRemoveCommand { get; set; }
        private void SearchConditionRemoveCommandExecute(object parameter)
        {
            var searchBox = parameter as Views.Domain.SearchBox;
            searchBox.SearchText = "";

            Customers = Storage.GetInstance().Customers;
        }
    }
}
