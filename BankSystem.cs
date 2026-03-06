using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class BankSystem
    {
        private static List<BankAccount> accounts = new List<BankAccount>();

        // REUSABLE HELPER METHODS
        private void Pause()
        {
            Console.WriteLine("\n  Press any key to continue...");
            Console.ReadKey();
        }
        private decimal ReadAmount()
        {
            while (true)
            {
                try
                {
                    string input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    { 
                        Console.Write("  Cannot be empty. Try again: "); 
                        continue; 
                    }
                    return 
                        Convert.ToDecimal(input);
                }
                catch 
                { 
                    Console.Write("  Invalid input. Enter a valid amount: "); 
                }
            }
        }

        private int ReadInt(string label, int min, int max)
        {
            while (true)
            {
                try
                {
                    int value = Convert.ToInt32(Console.ReadLine());
                    if (value >= min && value <= max) 
                        return value;
                    Console.Write("  Enter a value between " + min + " and " + max + ".\n  " + label + ": ");
                }
                catch 
                { 
                    Console.Write("  Numbers only.\n  " + label + ": "); 
                }
            }
        }

        private bool ValidateLettersOnly(string input)
        {
            foreach (char c in input)
                if (!char.IsLetter(c) && c != ' ') 
                    return false;
            return true;
        }

        private int ComputeAge(string birthday)
        {
            DateTime bday = DateTime.ParseExact(birthday, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            int age = DateTime.Now.Year - bday.Year;
            if (DateTime.Now < bday.AddYears(age)) 
                age--;
            return age;
        }

        private void ReplaceAccount(BankAccount upgraded)
        {
            for (int i = 0; i < accounts.Count; i++)
                if (accounts[i].GCashNumber == upgraded.GCashNumber)
                { 
                    accounts[i] = upgraded; 
                    break; 
                }
        }

        // VALIDATION HELPERS
        private string ValidateName()
        {
            Console.Write("\n  First Name: ");
            string firstName = "";
            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write("  Cannot be empty.\n  First Name: "); 
                    continue; 
                }
                if (!ValidateLettersOnly(input))
                { 
                    Console.Write("  Letters only.\n  First Name: "); 
                    continue; 
                }
                string[] fnParts = input.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string fnFormatted = "";
                foreach (string p in fnParts)
                    fnFormatted += char.ToUpper(p[0]) + p.Substring(1).ToLower() + " ";
                firstName = fnFormatted.Trim();
                break;
            }
            Console.Write("  Middle Name: ");
            string middleName = "";
            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                { 
                    Console.Write("  Cannot be empty.\n  Middle Name: "); 
                    continue; 
                }
                if (!ValidateLettersOnly(input))
                {
                    Console.Write("  Letters only.\n  Middle Name: "); 
                    continue; 
                }
                string[] parts = input.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string formatted = "";
                foreach (string p in parts)
                    formatted += (p.Length == 1 ? char.ToUpper(p[0]) + "." : char.ToUpper(p[0]) + p.Substring(1).ToLower()) + " ";
                middleName = formatted.Trim();
                break;
            }
            Console.Write("  Last Name: ");
            string lastName = "";
            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                { 
                    Console.Write("  Cannot be empty.\n  Last Name: "); 
                    continue; 
                }
                if (!ValidateLettersOnly(input))
                { 
                    Console.Write("  Letters only.\n  Last Name: "); 
                    continue; 
                }
                string[] lnParts = input.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string lnFormatted = "";
                foreach (string p in lnParts)
                    lnFormatted += char.ToUpper(p[0]) + p.Substring(1).ToLower() + " ";
                lastName = lnFormatted.Trim();
                break;
            }

            return string.IsNullOrEmpty(middleName)
                ? firstName + " " + lastName
                : firstName + " " + middleName + " " + lastName;
        }

        private int ValidateAge()
        {
            Console.Write("  Age: ");
            return ReadInt("Age", 8, 120);
        }

        private string ValidateBirthday(int age)
        {
            Console.Write("  Month (MM): ");
            int month = ReadInt("Month (MM)", 1, 12);

            Console.Write("  Day (DD): ");
            int day = ReadInt("Day (DD)", 1, 31);

            Console.Write("  Year (YYYY): ");
            int year = ReadInt("Year (YYYY)", 1900, DateTime.Now.Year);

            try
            {
                DateTime bday = new DateTime(year, month, day);
                if (bday > DateTime.Now)
                { 
                    Console.WriteLine("  Birthday cannot be in the future."); 
                    return ValidateBirthday(age); 
                }

                if (ComputeAge(bday.ToString("MM/dd/yyyy")) != age)
                { 
                    Console.WriteLine("  Birthday does not match age entered."); 
                    return ValidateBirthday(age); 
                }

                return bday.ToString("MM/dd/yyyy");
            }
            catch
            {
                Console.WriteLine("  Invalid date (e.g. Feb 30 does not exist).");
                return ValidateBirthday(age);
            }
        }

        private string ValidateGender()
        {
            while (true)
            {
                Console.WriteLine("  [1] Male  [2] Female  [3] Prefer not to say");
                Console.Write("  Choice: ");
                switch (Console.ReadLine())
                {
                    case "1": return "Male";
                    case "2": return "Female";
                    case "3": return "Prefer not to say";
                    default: Console.WriteLine("  Choose a valid option."); 
                        break;
                }
            }
        }

        private string ValidateAddress(string field, bool lettersOnly)
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || input.Trim().Length < 2)
                { 
                    Console.Write("  " + field + " is too short.\n  " + field + ": "); 
                    continue; 
                }

                bool invalid = false;
                foreach (char c in input)
                {
                    if (lettersOnly && !char.IsLetter(c) && c != ' ')
                    { 
                        invalid = true; 
                        break; 
                    }
                    else if (!lettersOnly && !char.IsLetterOrDigit(c) && c != ' ' && c != '.' && c != ',' && c != '#' && c != '-')
                    { 
                        invalid = true; 
                        break; 
                    }
                }

                if (invalid)
                { 
                    Console.Write("  Invalid " + field + ".\n  " + field + ": "); 
                    continue; 
                }

                string[] words = input.Trim().Split(' ');
                string result = "";
                foreach (string w in words)
                    if (w.Length > 0) result += char.ToUpper(w[0]) + w.Substring(1).ToLower() + " ";
                return result.Trim();
            }
        }

        private string ValidateGCashNumber()
        {
            while (true)
            {
                Console.Write("  09 - ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                { 
                    Console.WriteLine("  Cannot be empty."); 
                    continue; 
                }
                if (input.Length != 9)
                { 
                    Console.WriteLine("  Enter exactly 9 digits."); 
                    continue; 
                }

                bool allDigits = true;
                foreach (char c in input)
                    if (!char.IsDigit(c)) 
                    { allDigits = false; 
                        break; 
                    }
                if (!allDigits)
                { 
                    Console.WriteLine("  Numbers only."); 
                    continue; 
                }

                bool allSame = true;
                for (int i = 1; i < input.Length; i++)
                    if (input[i] != input[0]) 
                    { allSame = false; 
                        break; 
                    }
                if (allSame)
                { 
                    Console.WriteLine("  Invalid GCash number."); 
                    continue; 
                }

                return "09" + input;
            }
        }

        private string ValidateMpin(string label)
        {
            while (true)
            {
                Console.Write("  " + label + ": ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || input.Length != 4)
                { 
                    Console.WriteLine("  MPIN must be exactly 4 digits."); 
                    continue; 
                }

                bool allDigits = true;
                foreach (char c in input)
                    if (!char.IsDigit(c)) 
                    { allDigits = false; 
                        break; 
                    }
                if (!allDigits)
                { 
                    Console.WriteLine("  Numbers only."); 
                    continue; 
                }
                return input;
            }
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        private bool VerifyOtp(string otp, string gcashNumber)
        {
            string masked = gcashNumber.Substring(0, 4) + "XXX" + gcashNumber.Substring(7);
            Console.WriteLine("\n  A 6-digit code will be sent to " + masked);
            Console.WriteLine("  NEVER SHARE YOUR OTP OR USE IT IN ANY LINK.");
            Console.WriteLine("  Your OTP is " + string.Join(" ", otp.ToCharArray()));

            for (int i = 0; i < 3; i++)
            {
                Console.Write("\n  Enter OTP: ");
                if (Console.ReadLine() == otp) 
                    return true;
                int left = 2 - i;
                if (left > 0) 
                    Console.WriteLine("  Invalid OTP. " + left + " attempt(s) left.");
            }
            Console.WriteLine("  Too many invalid OTP attempts.");
            return false;
        }

        public BankAccount FindAccount(string gcashNumber)
        {
            foreach (BankAccount acc in accounts)
                if (acc.GCashNumber == gcashNumber) 
                    return acc;
            return null;
        }

        // REGISTER
        public void RegisterAccount()
        {
            Console.Clear();
            Console.WriteLine("=======      GCASH - SIGN UP      =======");
            Console.WriteLine("\n [ PERSONAL INFORMATION ]");

            string fullName = ValidateName();

            foreach (BankAccount acc in accounts)
            {
                if (acc.FullName == fullName)
                {
                    Console.WriteLine("\n  An account with this name already exists.");
                    Console.WriteLine("  Please use a different name or contact support.");
                    Pause(); 
                    return;
                }
            }

            int age = ValidateAge();
            Console.WriteLine("  Birthday:");
            string birthday = ValidateBirthday(age);
            Console.WriteLine("  Gender:");
            string gender = ValidateGender();
            Console.Write("  Street Address: ");
            string street = ValidateAddress("Street", false);
            Console.Write("  City: ");
            string city = ValidateAddress("City", true);
            Console.Write("  Province: ");
            string province = ValidateAddress("Province", true);

            Console.WriteLine("\n  Enter your GCash Number:");
            string gcashNumber = "";
            while (true)
            {
                gcashNumber = ValidateGCashNumber();
                if (FindAccount(gcashNumber) != null)
                {
                    Console.WriteLine("  GCash Number already exists. Please use a different number.");
                    Console.WriteLine("  Enter your GCash Number:");
                }
                else break;
            }

            Console.WriteLine("\n [ CREATE YOUR MPIN ]");
            Console.WriteLine("  NOTE: Never share your MPIN with anyone.");
            string mpin = ValidateMpin("Enter MPIN (4 digits)");
            while (true)
            {
                if (ValidateMpin("Confirm MPIN") == mpin) 
                    break;
                Console.WriteLine("  MPIN does not match. Try again.");
            }

            Console.Write("\n  Initial Deposit (min PHP 500.00): PHP ");
            decimal deposit = 0;
            while (true)
            {
                deposit = ReadAmount();
                decimal limit = age >= 18 ? 100000 : 10000;
                if (deposit < 500)
                { 
                    Console.Write("  Minimum initial deposit is PHP 500.00.\n  Try again: PHP "); 
                    continue; }
                if (deposit > limit)
                { 
                    Console.Write("  Exceeds wallet limit of PHP " + limit.ToString("N2") + ".\n  Try again: PHP "); 
                    continue; }
                break;
            }

            BankAccount newAccount;
            if (age >= 18)
                newAccount = new GSave(gcashNumber, fullName, mpin, deposit, age, birthday, gender, street, city, province);
            else
                newAccount = new BasicAccount(gcashNumber, fullName, mpin, deposit, age, birthday, gender, street, city, province);

            accounts.Add(newAccount);

            Console.WriteLine("\n========================================");
            Console.WriteLine("  Registration Successful!");
            Console.WriteLine("  Name         : " + fullName);
            Console.WriteLine("  GCash No.    : " + gcashNumber);
            Console.WriteLine("  Account Type : " + newAccount.GetAccountType());
            Console.WriteLine("  Balance      : PHP " + deposit.ToString("N2"));
            if (age < 18)
                Console.WriteLine("  NOTE: Basic accounts can only Cash In and Receive Money.");
            Console.WriteLine("==========================================");
            Pause();
        }

        // LOGIN
        public BankAccount Login()
        {
            Console.Clear();
            Console.WriteLine("=======      GCASH - LOG IN      =======");
            Console.WriteLine("\n  Enter your GCash Number:");
            string gcashNumber = ValidateGCashNumber();
            BankAccount account = FindAccount(gcashNumber);

            if (account == null)
            {
                Console.WriteLine("\n  Account not found.");
                Pause();
                return null;
            }

            if (account.IsLocked)
            {
                Console.WriteLine("\n  Account is locked. Verify your identity to unlock.");
                if (!VerifyOtp(GenerateOtp(), gcashNumber))
                {
                    Console.WriteLine("\n  OTP verification failed.");
                    Pause();
                    return null;
                }
                string newPin = ValidateMpin("New MPIN");
                while (true)
                {
                    if (ValidateMpin("Confirm MPIN") == newPin) 
                        break;
                    Console.WriteLine("  MPIN mismatch.");
                }
                account.ChangePin(newPin);
                account.IsLocked = false;
                account.FailedAttempts = 0;
                Console.WriteLine("\n  Account unlocked! Please login again.");
                Pause();
                return null;
            }

            // OTP verification
            if (!VerifyOtp(GenerateOtp(), gcashNumber))
            {
                Console.WriteLine("\n  OTP verification failed.");
                Pause();
                return null;
            }

            // MPIN entry
            int attempts = 0;
            while (attempts < 3)
            {
                Console.Write("\n  Enter MPIN: ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                { 
                    Console.WriteLine("  MPIN cannot be empty."); 
                    continue; }

                if (account.ValidatePin(input))
                {
                    CheckAgeUpgrade(account);
                    Console.WriteLine("\n  Login Successful! Welcome, " + account.FullName + "!");
                    Pause(); 
                    return account;
                }

                attempts++;
                int remaining = 3 - attempts;

                Console.WriteLine("\n  Incorrect MPIN.");
                if (remaining > 0)
                {
                    Console.WriteLine("  [1] Retry (" + remaining + " attempt(s) left)");
                    Console.WriteLine("  [2] Forgot MPIN?");
                    Console.Write("  Choice: ");
                    string choice = Console.ReadLine();

                    if (choice == "2")
                    {
                        if (!VerifyOtp(GenerateOtp(), gcashNumber))
                        {
                            Console.WriteLine("\n  OTP failed.");
                            Pause();
                            return null;
                        }
                        string newPin = ValidateMpin("New MPIN");
                        while (true)
                        {
                            if (ValidateMpin("Confirm MPIN") == newPin) break;
                            Console.WriteLine("  MPIN mismatch.");
                        }
                        account.ChangePin(newPin);
                        account.FailedAttempts = 0;
                        Console.WriteLine("\n  MPIN changed! Please login again.");
                        Pause();
                        return null;
                    }
                }
                else
                {
                    account.IsLocked = true;
                    account.LockStrikes++;
                    Console.WriteLine("  Too many failed attempts. Account is now locked.");
                    Pause();
                    return null;
                }
            }
            return null;
        }

        private void CheckAgeUpgrade(BankAccount account)
        {
            if (!(account is BasicAccount)) return;
            try
            {
                int currentAge = ComputeAge(account.Birthday);
                if (currentAge >= 18)
                {
                    Console.WriteLine("\n  You are now 18! You are eligible for Full Verification.");
                    Console.WriteLine("  [1] Upgrade to GSave");
                    Console.WriteLine("  [2] Later");
                    Console.Write("  Choice: ");

                    if (Console.ReadLine() == "1")
                    {
                        GSave upgraded = new GSave(account.GCashNumber, account.FullName, "", account.Balance, currentAge, account.Birthday, account.Gender, account.Street, account.City, account.Province);
                        upgraded.LockStrikes = account.LockStrikes;
                        upgraded.TotalTransactions = account.TotalTransactions;
                        upgraded.TransactionHistory = account.TransactionHistory;

                        ReplaceAccount(upgraded);

                        Console.WriteLine("\n  Account upgraded to GSave!");
                        Console.WriteLine("       Please login again to apply changes.");
                        Pause();
                    }
                }
            }
            catch { }
        }

        // SEND MONEY
        public void SendMoney(BankAccount sender)
        {
            if (sender is BasicAccount)
            {
                Console.WriteLine("\n  Send Money is not available for Basic (Unverified) accounts.");
                Pause();
                return;
            }

            Console.Clear();
            Console.WriteLine("=======      GCASH - SEND MONEY      =======");
            sender.ShowBalance();

            Console.WriteLine("\n  Enter Recipient GCash Number:");
            string recipientNumber = ValidateGCashNumber();

            if (recipientNumber == sender.GCashNumber)
            {
                Console.WriteLine("\n  You cannot send money to yourself.");
                Pause();
                return;
            }

            BankAccount recipient = FindAccount(recipientNumber);
            if (recipient == null)
            {
                Console.WriteLine("\n  Recipient account not found.");
                Pause();
                return;
            }

            Console.WriteLine("\n  Send to: " + recipient.FullName);
            Console.WriteLine("  [1] Confirm  [2] Cancel");
            Console.Write("  Choice: ");
            if (Console.ReadLine() != "1")
            {
                Console.WriteLine("\n  Transfer cancelled.");
                Pause();
                return;
            }

            Console.Write("\n  Enter Amount: PHP ");
            decimal amount = ReadAmount();

            if (amount <= 0)
            { 
                Console.WriteLine("\n  Amount must be greater than zero."); 
                Pause(); 
                return; 
            }

            if (amount > sender.Balance)
            { 
                Console.WriteLine("\n  Insufficient balance."); 
                Pause(); 
                return; 
            }

            if (recipient.Balance + amount > recipient.GetWalletLimit())
            {
                Console.WriteLine("\n  Recipient wallet limit exceeded.");
                Console.WriteLine("      They can only receive up to: PHP " + (recipient.GetWalletLimit() - recipient.Balance).ToString("N2"));
                Pause(); 
                return;
            }

            Console.Write("\n  Enter MPIN to confirm: ");
            if (!sender.ValidatePin(Console.ReadLine()))
            {
                Console.WriteLine("\n  Incorrect MPIN. Transaction cancelled.");
                Pause();
                return;
            }

            sender.DeductBalance(amount);
            sender.TotalTransactions++;
            sender.LastTransactionDate = DateTime.Now;
            sender.TransactionHistory.Add(new Transaction(TransactionType.SendMoney, amount, sender.Balance, "Sent to " + recipient.FullName + " (" + recipient.GCashNumber + ")"));
            recipient.ReceiveMoney(amount, sender.FullName + " (" + sender.GCashNumber + ")");

            Console.WriteLine("\n========================================");
            Console.WriteLine("  Transfer Successful!");
            Console.WriteLine("  Sent to     : " + recipient.FullName);
            Console.WriteLine("  Amount      : PHP " + amount.ToString("N2"));
            Console.WriteLine("  New Balance : PHP " + sender.Balance.ToString("N2"));
            Console.WriteLine("========================================");
            Pause();
        }

        // TRANSACTION HISTORY
        public void ShowTransactionHistory(BankAccount account)
        {
            Console.Clear();
            Console.WriteLine("=======      GCASH - TRANSACTION HISTORY      =======");
            Console.WriteLine("  [1] All  [2] Cash In  [3] Cash Out  [4] Send Money  [5] Received");
            Console.Write("  Filter: ");
            string choice = Console.ReadLine();

            if (account.TransactionHistory.Count == 0)
            {
                Console.WriteLine("\n  No transactions found.");
                Pause();
                return;
            }

            bool found = false;
            foreach (Transaction t in account.TransactionHistory)
            {
                bool show = choice == "1" ||
                            (choice == "2" && t.TransactionType == TransactionType.CashIn) ||
                            (choice == "3" && t.TransactionType == TransactionType.CashOut) ||
                            (choice == "4" && t.TransactionType == TransactionType.SendMoney) ||
                            (choice == "5" && t.TransactionType == TransactionType.ReceiveMoney);
                if (show) { t.DisplayTransaction(); found = true; }
            }

            if (!found) Console.WriteLine("\n  No transactions found for selected filter.");
            Pause();
        }

        // VIEW PROFILE
        public void ViewProfile(BankAccount account)
        {
            Console.Clear();
            Console.WriteLine("=======      GCASH - MY PROFILE      =======");
            Console.WriteLine("  Name         : " + account.FullName);
            Console.WriteLine("  GCash No.    : " + account.GCashNumber);
            Console.WriteLine("  Age          : " + account.Age);
            Console.WriteLine("  Birthday     : " + account.Birthday);
            Console.WriteLine("  Gender       : " + account.Gender);
            Console.WriteLine("  Address      : " + account.Street + ", " + account.City + ", " + account.Province);
            Console.WriteLine("  Account Type : " + account.GetAccountType());
            Console.WriteLine("  Balance      : PHP " + account.Balance.ToString("N2"));
            Console.WriteLine("  Wallet Limit : PHP " + account.GetWalletLimit().ToString("N2"));
            if (!(account is BasicAccount))
                Console.WriteLine("  Daily W.Limit: PHP " + account.GetDailyWithdrawalLimit().ToString("N2"));
            Console.WriteLine("  Lock Strikes : " + account.LockStrikes);
            Console.WriteLine("  Transactions : " + account.TotalTransactions);
            Console.WriteLine("========================================");
            Pause();
        }

        // CHANGE MPIN
        public void ChangeMpin(BankAccount account)
        {
            Console.Clear();
            Console.WriteLine("=======      GCASH - CHANGE MPIN      =======");
            Console.Write("\n  Current MPIN: ");

            if (!account.ValidatePin(Console.ReadLine()))
            {
                Console.WriteLine("\n  Incorrect MPIN.");
                Pause();
                return;
            }

            string newPin = ValidateMpin("New MPIN");
            while (true)
            {
                if (ValidateMpin("Confirm MPIN") == newPin) 
                    break;
                Console.WriteLine("  MPIN mismatch. Try again.");
            }
            account.ChangePin(newPin);
            Console.WriteLine("\n  MPIN changed successfully!");
            Pause();
        }

        // UPGRADE TO GSAVE
        public BankAccount UpgradeToGSave(BankAccount account)
        {
            Console.Clear();
            Console.WriteLine("=======      GCASH - UPGRADE TO GSAVE      =======");

            if (!(account is BasicAccount))
            {
                Console.WriteLine("\n  Your account is already verified.");
                Pause();
                return account;
            }

            int actualAge = 0;
            try 
            { 
                actualAge = ComputeAge(account.Birthday); 
            }
            catch 
            { 
                actualAge = account.Age; 
            }

            if (actualAge < 18)
            {
                Console.WriteLine("\n  You must be 18 or above to upgrade.");
                Console.WriteLine("      Current Age : " + actualAge);
                Console.WriteLine("      Birthday    : " + account.Birthday);
                Pause();
                return account;
            }

            account.Age = actualAge;

            Console.WriteLine("\n  You are eligible to upgrade to GSave!");
            Console.WriteLine("\n  Current Account : Basic (Unverified)");
            Console.WriteLine("  New Account     : GSave (Verified)");
            Console.WriteLine("  Wallet Limit    : PHP 100,000.00");
            Console.WriteLine("  Daily Withdraw  : PHP 10,000.00");
            Console.WriteLine("\n  [1] Confirm Upgrade");
            Console.WriteLine("  [2] Cancel");
            Console.Write("  Choice: ");

            if (Console.ReadLine() == "1")
            {
                GSave upgraded = new GSave(account.GCashNumber, account.FullName, "", account.Balance, account.Age, account.Birthday, account.Gender, account.Street, account.City, account.Province);
                upgraded.LockStrikes = account.LockStrikes;
                upgraded.TotalTransactions = account.TotalTransactions;
                upgraded.TransactionHistory = account.TransactionHistory;

                ReplaceAccount(upgraded);

                Console.WriteLine("\n  Account successfully upgraded to GSave!");
                Console.WriteLine("       Please login again to apply changes.");
                Pause();
                return upgraded;
            }
            else
            {
                Console.WriteLine("\n  Upgrade cancelled.");
                Pause();
                return account;
            }
        }

        // APPLY FOR GCREDIT
        public void ApplyForGCredit(BankAccount account)
        {
            Console.Clear();
            Console.WriteLine("=======      GCASH - APPLY FOR GCREDIT      =======");

            if (account is BasicAccount)
            {
                Console.WriteLine("\n  Upgrade to GSave first before applying for GCredit.");
                Pause();
                return;
            }

            if (account is GCredit)
            {
                Console.WriteLine("\n  You are already a GCredit user!");
                Pause();
                return;
            }

            bool t = account.TotalTransactions >= 3;
            bool b = account.Balance >= 5000;
            bool l = account.LockStrikes < 2;

            Console.WriteLine("\n  Checking eligibility...\n");
            Console.WriteLine("  Transactions (min. 3)   : " + account.TotalTransactions + "/3 - " + (t ? "[PASSED]" : "[FAILED]"));
            Console.WriteLine("  Balance (min. PHP 5,000): PHP " + account.Balance.ToString("N2") + " - " + (b ? "[PASSED]" : "[FAILED]"));
            Console.WriteLine("  Lock History (max 1)    : " + account.LockStrikes + " strike(s) - " + (l ? "[PASSED]" : "[DISQUALIFIED]"));

            if (t && b && l)
            {
                Console.WriteLine("\n  Congratulations! You are eligible for GCredit!");
                Console.WriteLine("  [1] Upgrade to GCredit  [2] Cancel");
                Console.Write("  Choice: ");

                if (Console.ReadLine() == "1")
                {
                    GCredit upgraded = new GCredit(account.GCashNumber, account.FullName, "", account.Balance, account.Age, account.Birthday, account.Gender,
                        account.Street, account.City, account.Province);
                    upgraded.LockStrikes = account.LockStrikes;
                    upgraded.TotalTransactions = account.TotalTransactions;
                    upgraded.TransactionHistory = account.TransactionHistory;

                    ReplaceAccount(upgraded);

                    Console.WriteLine("\n  Upgraded to GCredit! Daily withdrawal limit: PHP 50,000.00");
                    Console.WriteLine("       Please login again to apply changes.");
                }
            }
            else
                Console.WriteLine("\n  You do not meet the requirements yet. Keep using GCash to qualify!");

            Pause();
        }
    }
}