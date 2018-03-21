using System;
using PowerArgs;

namespace PlexApp {
  public class Program {
    public static void Main(string[] args) {
      try {
        Args.InvokeAction<Options>(args);
      }
      catch (Exception e) {
        Console.WriteLine(e.GetBaseException().Message);

        Environment.Exit(-1);
      }
    }
  }
}