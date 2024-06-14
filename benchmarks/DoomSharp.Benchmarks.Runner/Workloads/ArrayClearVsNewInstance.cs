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
public class ArrayClearVsNewInstance
{
    private const int Size = 1024;

    private readonly Random _randomizer = new(42);

    private int[] _renewedArray = new int[Size];
    private readonly int[] _preAllocatedArray = new int[Size];

    [Benchmark]
    public void RenewingArrays()
    {
        // Create a new instance and fill with data
        _renewedArray = new int[Size];
        for (var i = 0; i < Size; i++)
        {
            _renewedArray[i] = i;
        }
    }

    [Benchmark]
    public void ReusingArrays()
    {
        // Clear the array and fill with data
        Array.Clear(_preAllocatedArray);
        for (var i = 0; i < Size; i++)
        {
            _preAllocatedArray[i] = i;
        }
    }

    [Benchmark]
    public void RenewingArraysUsingRandom()
    {
        // Create a new instance and fill with data
        _renewedArray = new int[Size];
        for (var i = 0; i < Size; i++)
        {
            _renewedArray[i] = _randomizer.Next();
        }
    }

    [Benchmark]
    public void ReusingArraysUsingRandom()
    {
        // Clear the array and fill with data
        Array.Clear(_preAllocatedArray);
        for (var i = 0; i < Size; i++)
        {
            _preAllocatedArray[i] = _randomizer.Next();
        }
    }
}