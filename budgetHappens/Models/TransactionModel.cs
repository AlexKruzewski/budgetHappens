using budgetHappens.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetHappens.Models
{
    public class TransactionModel
    {
        #region Parameters

        public decimal Amount { get; set; }
        public string StringAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public TransactionType TransactionType { get; set;}

        #endregion

        #region Attributes
        #endregion

        #region Constructors
        /// <summary>
        /// Builds the withdrawal model and populates it with the given data
        /// </summary>
        /// <param name="amount">Amount of the withdrawal</param>
        /// <param name="description">Description of the withdrawal</param>
        /// <param name="currencySymbol">Currency symbol used (for display)</param>
        public TransactionModel(decimal amount, string description, string currencySymbol, TransactionType transactionType)
        {
            this.Amount = amount;
            switch(transactionType)
            {
                case TransactionType.Deposit:
                    this.StringAmount = String.Format("{0}{1}",currencySymbol, amount.ToString("0.00"));
                    break;
                case TransactionType.Withdrawal:
                    this.StringAmount = String.Format("-{0}{1}",currencySymbol, amount.ToString("0.00"));
                    break;
            }
            this.Description = description;
            this.TransactionDate = DateTime.Now;
            this.TransactionType = transactionType;
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        

       
        
    }
}
