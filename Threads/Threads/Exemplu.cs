using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads
{
    public class Exemplu
    {
        static void Main(string[] args)
        {
            BankAcct acct = new BankAcct(10);
            Thread[] threads = new Thread[15];

            Thread.CurrentThread.Name = "main thread";

            for(int i = 0; i < 15; i++)
            {
                Thread t = new Thread(new ThreadStart(acct.Issue));
                t.Name = i.ToString();
                threads[i] = t;
            }

            for(int i = 0; i < 15; i++)
            {
                Console.WriteLine("thread {0} alive: {1}", threads[i].Name, threads[i].IsAlive);
                threads[i].Start();
                Console.WriteLine("thread {0} alive: {1}", threads[i].Name, threads[i].IsAlive);
            }

            Console.WriteLine("priority: {0}", Thread.CurrentThread.Priority);
            Console.WriteLine("thread {0} ending", Thread.CurrentThread.Name);
            Console.ReadLine(); 
        }
    }
}

class BankAcct
{
    //private Object acctLock = new object();
    double Balance { set; get; }
    public BankAcct(double balance)    
    {
        Balance = balance;
    }   

    public double Withdraw(double amt)
    {
        if((Balance - amt) < 0)
        {
            Console.WriteLine($"sorry ${Balance} in account");
            return Balance;
        }

        lock (this)//(acctLock)
        {
            if(Balance >= amt)
            {
                Console.WriteLine("removed {0} and {1} left in the account", amt, (Balance-amt));
                Balance -= amt;
            }
            return Balance; 
        }
    }

    public void Issue()
    {
        Withdraw(1);
    }

}