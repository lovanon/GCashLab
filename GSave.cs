using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class GSave : BankAccount
    {
        public GSave(string gcashNumber, string fullName, string pin, decimal initialDeposit, int age, string birthday, string gender, string street, string city, string province)
            : base(gcashNumber, fullName, pin, initialDeposit, age, birthday, gender, street, city, province) { }

        public override void Withdraw(decimal amount)
        {
            ResetDailyLimitsIfNewDay();

            if (amount <= 0)
            { Console.WriteLine("\n Amount must be greater than zero."); 
                return; 
            }

            if (amount > Balance)
            { Console.WriteLine("\n Insufficient balance.\n     Current Balance: PHP " + Balance.ToString("N2")); 
                return; }

            if (DailyWithdrawalTotal >= 10000)
            { Console.WriteLine("\n Daily Cash Out limit of PHP 10,000.00 reached. Try again tomorrow."); 
                return; }

            if (DailyWithdrawalTotal + amount > 10000)
            {
                Console.WriteLine("\n Amount exceeds daily Cash Out limit.");
                Console.WriteLine("     Remaining allowance today: PHP " + (10000 - DailyWithdrawalTotal).ToString("N2"));
                return;
            }

            Balance -= amount;
            DailyWithdrawalTotal += amount;
            LastTransactionDate = DateTime.Now;
            TotalTransactions++;
            TransactionHistory.Add(new Transaction(TransactionType.CashOut, amount, Balance, "Cash Out from GCash Wallet"));

            Console.WriteLine("\n Cash Out Successful!");
            Console.WriteLine("      Amount      : PHP " + amount.ToString("N2"));
            Console.WriteLine("      New Balance : PHP " + Balance.ToString("N2"));
            Console.WriteLine("      Daily Cash Out used: PHP " + DailyWithdrawalTotal.ToString("N2") + " / PHP 10,000.00");
        }
        public override string GetAccountType() 
        { 
            return "GSave (Verified)"; 
        }
        public override decimal GetWalletLimit() 
        { 
            return 100000; 
        }
        public override decimal GetDailyWithdrawalLimit() 
        { 
            return 10000; 
        }
    }
}
