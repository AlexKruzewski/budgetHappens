﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetHappens.Models
{
    public class WithdrawalModel
    {
        #region Parameters

        public decimal Amount { get; set; }
        public string StringAmount { get; set; }
        public DateTime WithdrawalDate { get; set; }
        public string Description { get; set; }

        #endregion

        #region Attributes
        #endregion

        #region Constructors
        public WithdrawalModel(decimal amount, string description, string currencySymbol)
        {
            this.Amount = amount;
            this.StringAmount = currencySymbol + amount.ToString("0.00");
            this.Description = description;
            this.WithdrawalDate = DateTime.Now;
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        

       
        
    }
}