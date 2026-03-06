using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public enum TransactionType { CashIn, CashOut, SendMoney, ReceiveMoney }
    public class Transaction
    {
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
        public string Details { get; set; }

        public Transaction(TransactionType type, decimal amount, decimal balanceAfter, string details)
        {
            TransactionType = type;
            Amount = amount;
            Date = DateTime.Now;
            BalanceAfterTransaction = balanceAfter;
            Details = details;
        }

        public void DisplayTransaction()
        {
            string typeLabel = "";
            switch (TransactionType)
            {
                case TransactionType.CashIn: typeLabel = "Cash In"; 
                    break;
                case TransactionType.CashOut: typeLabel = "Cash Out"; 
                    break;
                case TransactionType.SendMoney: typeLabel = "Send Money"; 
                    break;
                case TransactionType.ReceiveMoney: typeLabel = "Receive Money"; 
                    break;
            }
            Console.WriteLine("========================================");
            Console.WriteLine(" Date    : " + Date.ToString("MM/dd/yyyy hh:mm tt"));
            Console.WriteLine(" Type    : " + typeLabel);
            Console.WriteLine(" Amount  : PHP " + Amount.ToString("N2"));
            Console.WriteLine(" Details : " + Details);
            Console.WriteLine(" Balance : PHP " + BalanceAfterTransaction.ToString("N2"));
            Console.WriteLine("========================================");
        }
    }
}
