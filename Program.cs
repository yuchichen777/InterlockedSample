using System.Diagnostics;

namespace InterlockedSample
{
     internal class Program
     {
          static object _lock = new object();
          static void Main(string[] args)
          {
               Stopwatch sw = new Stopwatch();
               int balanceValue = 10000000;
               Member member = new Member() { Balance = balanceValue };
               List<Task> tasks = new List<Task>();
               sw.Start();
               for (int i = 0; i < 1000000; i++)
               {
                    tasks.Add(Task.Run(() => member.UpdateBalance()));
               }
               Task.WaitAll(tasks.ToArray());
               sw.Stop();
               Console.WriteLine("Normal Version");
               Console.WriteLine($"member remaining balance is {member.Balance}");
               Console.WriteLine($"Exec Time Cost : {sw.ElapsedMilliseconds}");

               tasks.Clear();
               member.Balance = balanceValue;
               sw.Restart();
               for (int i = 0; i < 1000000; i++)
               {
                    tasks.Add(Task.Run(() => member.UpdateBalanceWithLock()));
               }
               Task.WaitAll(tasks.ToArray());
               sw.Stop();
               Console.WriteLine("Lock Version:");
               Console.WriteLine($"member remaining balance is {member.Balance}");
               Console.WriteLine($"Exec Time Cost : {sw.ElapsedMilliseconds}");

               tasks.Clear();
               member.Balance = balanceValue;
               sw.Restart();
               for (int i = 0; i < 1000000; i++)
               {
                    tasks.Add(Task.Run(() => member.UpdateBalanceWithInterlock()));
               }
               Task.WaitAll(tasks.ToArray());
               sw.Stop();
               Console.WriteLine("InterLocked Version:");
               Console.WriteLine($"member remaining balance is {member.Balance}");
               Console.WriteLine($"Exec Time Cost : {sw.ElapsedMilliseconds}");

               Console.ReadKey();
          }
     }

     public class Member
     {
          object _lock = new object();
          public int Balance;

          public void UpdateBalance()
          {
               Balance -= 10;
          }

          public void UpdateBalanceWithLock()
          {
               lock (_lock)
               {
                    Balance -= 10;
               }
          }

          public void UpdateBalanceWithInterlock()
          {
               int val;
               do
               {
                    val = Balance;
               }
               while (val != Interlocked.CompareExchange(ref Balance, val - 10, val));
          }
     }
}
