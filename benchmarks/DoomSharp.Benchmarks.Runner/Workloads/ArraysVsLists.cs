using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace DoomSharp.Benchmarks.Runner.Workloads;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class ArraysVsLists
{
    private const int Size = 1024;

    private readonly Random _randomizer = new(42);

    [Benchmark]
    public void PreAllocatedArray()
    {
        // Create a new instance with the intended size, and fill with data
        var array = new int[Size];
        for (var i = 0; i < Size; i++)
        {
            array[i] = _randomizer.Next();
        }
    }

    [Benchmark]
    public void PreAllocatedList()
    {
        // Create a new instance with the intended size, and fill with data
        // ReSharper disable once CollectionNeverQueried.Local
        var list = new List<int>(Size);
        for (var i = 0; i < Size; i++)
        {
            list.Add(_randomizer.Next());
        }
    }

    [Benchmark]
    public void DynamicallyResizedList()
    {
        // Create a new instance with a minimum size, and fill with data while resizing
        // ReSharper disable once CollectionNeverQueried.Local
        var list = new List<int>();
        for (var i = 0; i < Size; i++)
        {
            list.Add(_randomizer.Next());
        }

        list.TrimExcess(); // it's only fair to trim the list to the actual size since we're doing it for arrays as well
    }

    [Benchmark]
    public void DynamicallyResizedArrayDoublingSequence()
    {
        // Create a new instance with a small size, and fill with data while resizing in increasing sizes
        var array = new int[4];
        for (var i = 0; i < Size; i++)
        {
            if (array.Length == i)
            {
                Array.Resize(ref array, i * 2);
            }

            array[i] = _randomizer.Next();
        }

        // Resize down to the actual size
        Array.Resize(ref array, Size);
    }


    [Benchmark]
    public void DynamicallyResizedArrayChunked()
    {
        // Create a new instance with a small size, and fill with data while resizing in chunks
        var array = new int[10];
        for (var i = 0; i < Size; i++)
        {
            if (array.Length == i)
            {
                Array.Resize(ref array, i + 10);
            }
            array[i] = _randomizer.Next();
        }

        // Resize down to the actual size
        Array.Resize(ref array, Size);
    }

    [Benchmark]
    public void DynamicallyResizedArrayAggressive()
    {
        // Create a new instance with a minimum size, and fill with data while resizing
        var array = new int[1];
        for (var i = 0; i < Size; i++)
        {
            if (array.Length == i)
            {
                Array.Resize(ref array, i + 1);
            }
            array[i] = _randomizer.Next();
        }
    }
}