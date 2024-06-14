using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace DoomSharp.Benchmarks.Runner.Workloads;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.NativeAot60)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.NativeAot70)]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.NativeAot80)]
[MemoryDiagnoser]
public class UnsafeVsSafe
{
    // Code sample: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/unsafe-code#pointer-types

    [Benchmark]
    public void Unsafe()
    {
        // Normal pointer to an object.
        var a = new int[] { 10, 20, 30, 40, 50 };
        // Must be in unsafe code to use interior pointers.
        unsafe
        {
            // Must pin object on heap so that it doesn't move while using interior pointers.
            fixed (int* p = &a[0])
            {
                // p is pinned as well as object, so create another pointer to show incrementing it.
                int* p2 = p;
                Console.WriteLine(*p2);
                // Incrementing p2 bumps the pointer by four bytes due to its type ...
                p2 += 1;
                Console.WriteLine(*p2);
                p2 += 1;
                Console.WriteLine(*p2);
                Console.WriteLine("--------");
                Console.WriteLine(*p);
                // Dereferencing p and incrementing changes the value of a[0] ...
                *p += 1;
                Console.WriteLine(*p);
                *p += 1;
                Console.WriteLine(*p);
            }
        }

        Console.WriteLine("--------");
        Console.WriteLine(a[0]);

        /*
        Output:
        10
        20
        30
        --------
        10
        11
        12
        --------
        12
        */
    }

    [Benchmark]
    public void Safe()
    {
        // Normal pointer to an object.
        var a = new int[] { 10, 20, 30, 40, 50 };

        var index = 0;
        var index2 = index;
        Console.WriteLine(a[index2]);
        // Incrementing p2 bumps the pointer by four bytes due to its type ...
        index2 += 1;
        Console.WriteLine(a[index2]);
        index2 += 1;
        Console.WriteLine(a[index2]);
        Console.WriteLine("--------");
        Console.WriteLine(a[index]);
        // Dereferencing p and incrementing changes the value of a[0] ...
        a[index] += 1;
        Console.WriteLine(a[index]);
        a[index] += 1;
        Console.WriteLine(a[index]);

        Console.WriteLine("--------");
        Console.WriteLine(a[0]);

        /*
        Output:
        10
        20
        30
        --------
        10
        11
        12
        --------
        12
        */
    }
}
