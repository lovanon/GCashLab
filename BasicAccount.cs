using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class BasicAccount : BankAccount
    {
        public BasicAccount(string gcashNumber, string fullName, string pin, decimal initialDeposit, int age, string birthday, string gender, string street, string city, string province)
            : base(gcashNumber, fullName, pin, initialDeposit, age, birthday, gender, street, city, province) { }

        public override void Withdraw(decimal amount)
        {
            Console.WriteLine("\n Cash Out is not available for Basic (Unverified) accounts.");
            Console.WriteLine("     Please upgrade your account to access this feature.");
        }

        public override string GetAccountType() { return "Basic (Unverified)"; }
        public override decimal GetWalletLimit() { return 10000; }
        public override decimal GetDailyWithdrawalLimit() { return 0; }
    }
}
