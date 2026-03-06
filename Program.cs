using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            BankSystem bankSystem = new BankSystem();
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("                                   ");
                Console.WriteLine("              GCASH                ");
                Console.WriteLine("     Your Everyday Money App       ");
                Console.WriteLine("                                   ");
                Console.WriteLine("===================================");
                Console.WriteLine(" [1] Register");
                Console.WriteLine(" [2] Login");
                Console.WriteLine(" [3] Exit");
                Console.WriteLine("===================================");
                Console.Write(" Choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        bankSystem.RegisterAccount();
                        break;

                    case "2":
                        BankAccount account = bankSystem.Login();
                        if (account != null) ShowDashboard(account, bankSystem);
                        break;

                    case "3":
                        running = false;
                        Console.Clear();
                        Console.WriteLine("\n Thank you for using GCash! Goodbye!\n");
                        break;

                    default:
                        Console.WriteLine("\n Invalid option.");
                        Console.WriteLine(" Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ShowDashboard(BankAccount account, BankSystem bankSystem)
        {
            bool loggedIn = true;
            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("  Hi, " + account.FullName.Split(' ')[0] + "!");
                Console.WriteLine("  Wallet  : PHP " + account.Balance.ToString("N2"));
                Console.WriteLine("  Account : " + account.GetAccountType());
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("  [1] Cash In");
                Console.WriteLine("  [2] Cash Out");
                Console.WriteLine("  [3] Send Money");
                Console.WriteLine("  [4] Transaction History");
                Console.WriteLine("  [5] My Account");
                Console.WriteLine("  [6] Logout");
                Console.WriteLine("========================================");
                Console.Write("  Choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("=======      GCASH - CASH IN      =======");
                        account.ShowBalance();
                        account.ResetDailyLimitsIfNewDay();
                        Console.WriteLine(" Daily Cash In used : PHP " + account.DailyCashInTotal.ToString("N2") + " / PHP 50,000.00");
                        Console.WriteLine(" Remaining today    : PHP " + (50000 - account.DailyCashInTotal).ToString("N2"));
                        Console.WriteLine(" Wallet             : PHP " + account.Balance.ToString("N2") + " / PHP " + account.GetWalletLimit().ToString("N2"));
                        Console.WriteLine(" Can still Cash In  : PHP " + (account.GetWalletLimit() - account.Balance).ToString("N2"));
                        Console.Write("\n  Enter Amount: PHP ");
                        try
                        {
                            account.Deposit(Convert.ToDecimal(Console.ReadLine()));
                        }
                        catch
                        {
                            Console.WriteLine("\n Invalid amount. Please enter numbers only.");
                        }
                        Console.WriteLine("\n Press any key to return...");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("=======      GCASH - CASH OUT      =======");
                        account.ShowBalance();
                        if (!(account is BasicAccount))
                        {
                            account.ResetDailyLimitsIfNewDay();
                            Console.WriteLine(" Daily Cash Out used: PHP " + account.DailyWithdrawalTotal.ToString("N2") + " / PHP " + account.GetDailyWithdrawalLimit().ToString("N2"));
                            Console.WriteLine(" Remaining today    : PHP " + (account.GetDailyWithdrawalLimit() - account.DailyWithdrawalTotal).ToString("N2"));
                        }
                        Console.Write("\n Enter Amount: PHP ");
                        try
                        {
                            account.Withdraw(Convert.ToDecimal(Console.ReadLine()));
                        }
                        catch
                        {
                            Console.WriteLine("\n Invalid amount. Please enter numbers only.");
                        }
                        Console.WriteLine("\n Press any key to return...");
                        Console.ReadKey();
                        break;

                    case "3":
                        bankSystem.SendMoney(account);
                        break;
                    case "4":
                        bankSystem.ShowTransactionHistory(account);
                        break;
                    case "5":
                        ShowMyAccount(account, bankSystem);
                        break;

                    case "6":
                        Console.Clear();
                        Console.WriteLine("\n Are you sure you want to logout?");
                        Console.WriteLine("  [1] Yes  [2] No");
                        Console.Write("  Choice: ");
                        if (Console.ReadLine() == "1")
                        {
                            loggedIn = false;
                            Console.WriteLine("\n Logged out. Thank you for using GCash!");
                            Console.WriteLine("\n Press any key...");
                            Console.ReadKey();
                        }
                        break;

                    default:
                        Console.WriteLine("\n Invalid option.");
                        Console.WriteLine(" Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ShowMyAccount(BankAccount account, BankSystem bankSystem)
        {
            bool inMenu = true;
            while (inMenu)
            {
                Console.Clear();
                Console.WriteLine("=======      GCASH - MY PROFILE      =======");
                Console.WriteLine(" [1] View Profile");
                Console.WriteLine(" [2] Change MPIN");

                if (account is BasicAccount)
                {
                    Console.WriteLine(" [3] Upgrade to GSave");
                    Console.WriteLine(" [4] Back");
                }
                else
                {
                    Console.WriteLine(" [3] Apply for GCredit");
                    Console.WriteLine(" [4] Back");
                }

                Console.WriteLine("=====================================");
                Console.Write(" Choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        bankSystem.ViewProfile(account);
                        break;
                    case "2":
                        bankSystem.ChangeMpin(account);
                        break;
                    case "3":
                        if (account is BasicAccount)
                            account = bankSystem.UpgradeToGSave(account);
                        else
                            bankSystem.ApplyForGCredit(account);
                        break;
                    case "4":
                        inMenu = false;
                        break;
                    default:
                        Console.WriteLine("\n Invalid option.");
                        Console.WriteLine(" Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}