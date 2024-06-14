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
public class StructsVsClasses
{
    public class SimpleClass
    {
        public int Value1;
        public int Value2;
    }

    public struct SimpleStruct
    {
        public int Value1;
        public int Value2;
    }

    private const int Size = 1_000_000;

    private readonly SimpleClass[] _classes = new SimpleClass[Size];
    private readonly SimpleStruct[] _structs = new SimpleStruct[Size];

    public StructsVsClasses()
    {
        for (var i = 0; i < Size; i++)
        {
            _classes[i] = new SimpleClass { Value1 = Random.Shared.Next(), Value2 = Random.Shared.Next() };
            _structs[i] = new SimpleStruct { Value1 = Random.Shared.Next(), Value2 = Random.Shared.Next() };
        }
    }

    [Benchmark]
    public int StructArrayAccess()
    {
        var result = 0;
        for (var i = 0; i < Size; i++)
        {
            result += Helper1(_structs, i);
        }

        return result;
    }

    [Benchmark]
    public int ClassArrayAccess()
    {
        var result = 0;
        for (var i = 0; i < Size; i++)
        {
            result += Helper2(_classes, i);
        }

        return result;
    }

    [Benchmark]
    public int StructImmediateAccess()
    {
        var result = 0;
        for (var i = 0; i < Size; i++)
        {
            result += _structs[i].Value1;
        }

        return result;
    }

    [Benchmark]
    public int ClassImmediateAccess()
    {
        var result = 0;
        for (var i = 0; i < Size; i++)
        {
            result += _classes[i].Value1;
        }

        return result;
    }

    public int Helper1(SimpleStruct[] array, int index)
    {
        return array[index].Value1;
    }

    public int Helper2(SimpleClass[] array, int index)
    {
        return array[index].Value1;
    }
}