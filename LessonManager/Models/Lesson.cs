﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LessonManager.Models
{
    public class Lesson : INotifyPropertyChanged
    {
        public enum PType
        {
            Cash,
            Card,
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private int id_;
        public int ID
        {
            get { return id_; }
            set
            {
                id_ = value;
                RaisePropertyChanged();
            }
        }

        private int studioID_;
        public int StudioID
        {
            get { return studioID_; }
            set
            {
                studioID_ = value;
                RaisePropertyChanged();
            }
        }

        private int staffID_;
        public int StaffID
        {
            get { return staffID_; }
            set
            {
                staffID_ = value;
                RaisePropertyChanged();
            }
        }

        private int customerID_;
        public int CustomerID
        {
            get { return customerID_; }
            set
            {
                customerID_ = value;
                RaisePropertyChanged();
            }
        }

        private int fee_;
        public int Fee
        {
            get { return fee_; }
            set
            {
                fee_ = value;
                RaisePropertyChanged();
            }
        }

        private PType paymentType_;
        public PType PaymentType
        {
            get { return paymentType_; }
            set
            {
                paymentType_ = value;
                RaisePropertyChanged();
            }
        }

        private DateTime takenAt_;
        public DateTime TakenAt
        {
            get { return takenAt_; }
            set
            {
                takenAt_ = value;
                RaisePropertyChanged();
            }
        }
    }
}
