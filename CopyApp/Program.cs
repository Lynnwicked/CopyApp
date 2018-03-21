using System;
using PowerArgs;

namespace CopyApp {
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