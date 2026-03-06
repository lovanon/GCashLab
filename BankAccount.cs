using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public abstract class BankAccount
    {
        public string GCashNumber { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public bool IsLocked { get; set; }
        public int FailedAttempts { get; set; }
        public int LockStrikes { get; set; }
        public decimal DailyCashInTotal { get; set; }
        public decimal DailyWithdrawalTotal { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public List<Transaction> TransactionHistory { get; set; }
        public int TotalTransactions { get; set; }
        public decimal Balance { get; protected set; }

        private string pin;

        public BankAccount(string gcashNumber, string fullName, string pin, decimal initialDeposit, int age, string birthday, string gender, string street, string city, string province)
        {
            GCashNumber = gcashNumber;
            FullName = fullName;
            this.pin = pin;
            Balance = initialDeposit;
            Age = age;
            Birthday = birthday;
            Gender = gender;
            Street = street;
            City = city;
            Province = province;
            IsLocked = false;
            FailedAttempts = 0;
            LockStrikes = 0;
            DailyCashInTotal = 0;
            DailyWithdrawalTotal = 0;
            LastTransactionDate = DateTime.Now;
            TransactionHistory = new List<Transaction>();
            TotalTransactions = 0;
        }

        public bool ValidatePin(string inputPin) 
        { 
            return pin == inputPin; 
        }
        public void ChangePin(string newPin) 
        { 
            pin = newPin; 
        }
        public void DeductBalance(decimal amount) 
        { 
            Balance -= amount; 
        }
        public void ResetDailyLimitsIfNewDay()
        {
            if (LastTransactionDate.Date != DateTime.Now.Date)
            {
                DailyCashInTotal = 0;
                DailyWithdrawalTotal = 0;
            }
        }

        public void Deposit(decimal amount)
        {
            ResetDailyLimitsIfNewDay();
            decimal walletLimit = GetWalletLimit();

            if (amount <= 0)
            { 
                Console.WriteLine("\n Amount must be greater than zero."); 
                return; 
            }

            if (amount > 50000)
            {
                Console.WriteLine("\n Maximum Cash In per transaction is PHP 50,000.00.");
                return; 
            }

            if (DailyCashInTotal >= 50000)
            { Console.WriteLine("\n Daily Cash In limit of PHP 50,000.00 has been reached."); 
                return; 
            }

            if (DailyCashInTotal + amount > 50000)
            {
                Console.WriteLine("\n Amount exceeds daily Cash In limit.");
                Console.WriteLine("     Remaining allowance today: PHP " + (50000 - DailyCashInTotal).ToString("N2"));
                return;
            }

            if (Balance >= walletLimit)
            { 
                Console.WriteLine("\n Your wallet is already full. Limit: PHP " + walletLimit.ToString("N2")); 
                return; 
            }

            if (Balance + amount > walletLimit)
            {
                Console.WriteLine("\n Amount exceeds your wallet limit.");
                Console.WriteLine("     You can only Cash In up to: PHP " + (walletLimit - Balance).ToString("N2"));
                return;
            }

            Balance += amount;
            DailyCashInTotal += amount;
            LastTransactionDate = DateTime.Now;
            TotalTransactions++;
            TransactionHistory.Add(new Transaction(TransactionType.CashIn, amount, Balance, "Cash In to GCash Wallet"));

            Console.WriteLine("\n Cash In Successful!");
            Console.WriteLine("      Amount      : PHP " + amount.ToString("N2"));
            Console.WriteLine("      New Balance : PHP " + Balance.ToString("N2"));
            Console.WriteLine("      Daily Cash In used: PHP " + DailyCashInTotal.ToString("N2") + " / PHP 50,000.00");
        }

        public void ReceiveMoney(decimal amount, string senderName)
        {
            Balance += amount;
            TotalTransactions++;
            LastTransactionDate = DateTime.Now;
            TransactionHistory.Add(new Transaction(TransactionType.ReceiveMoney, amount, Balance, "Received from " + senderName));
        }

        public void ShowBalance()
        {
            Console.WriteLine("\n Current Balance: PHP " + Balance.ToString("N2"));
        }

        public abstract void Withdraw(decimal amount);
        public abstract string GetAccountType();
        public abstract decimal GetWalletLimit();
        public abstract decimal GetDailyWithdrawalLimit();
    }
}
