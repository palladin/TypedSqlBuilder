using System;
using System.Collections.Immutable;

namespace ParamsTest
{
    public class Program
    {
        // Test method with params ImmutableArray
        public static void TestMethod(params ImmutableArray<int> values)
        {
            Console.WriteLine($"Received {values.Length} values");
            foreach (var value in values)
            {
                Console.WriteLine($"Value: {value}");
            }
        }
        
        public static void Main()
        {
            Console.WriteLine("Testing params ImmutableArray...");
            
            // Try to call it with multiple parameters
            TestMethod(1, 2, 3, 4);
            
            Console.WriteLine("Test completed!");
        }
    }
}
