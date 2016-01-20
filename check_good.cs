using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BlueBirdKassa
{
    public class check_good
    {
        public long ID { get; set; }
        public string Good { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int Discount { get; set; }
        public decimal DiscountCost { get; set; }
        public string Depot { get; set; }
        public string Checkdate { get; set; }
        public string CheckID { get; set; }
        public check_good() { }
        public check_good(long id, string good, int count, decimal price, decimal cost)
        {
            this.ID = id;
            this.Good = good;
            this.Count = count;
            this.Price = price;
            this.Cost = cost;
        }
    }
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Constructor

        protected ViewModelBase()
        {
        }

        #endregion

        #region DisplayName

        public virtual string DisplayName { get; protected set; }

        #endregion

        #region Debugging Aides

        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
            }
        }

        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

#if DEBUG
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion
    }

    public class CheckViewModel : ViewModelBase
    {
        private readonly check_good _dataGood;

        public CheckViewModel(check_good good)
        {
            _dataGood = good;                    
        }
        public long ID
        {
            get { return _dataGood.ID; }
            set 
            { 
                _dataGood.ID = value;
                OnPropertyChanged("ID");
            }
        }
        public string Good
        {
            get { return _dataGood.Good; }
            set 
            {
                _dataGood.Good = value;
                OnPropertyChanged("Good");
            }
        }
        public int Count
        {
            get { return _dataGood.Count; }
            set
            {
                _dataGood.Count = value;
                OnPropertyChanged("Count");
            }
        }
        public decimal Price
        {
            get { return _dataGood.Price; }
            set
            {
                _dataGood.Price = value;
                OnPropertyChanged("Price");
            }
        }
        public decimal Cost
        {
            get { return _dataGood.Cost; }
            set
            {
                _dataGood.Cost = value;
                OnPropertyChanged("Cost");
            }
        }
    }
}
