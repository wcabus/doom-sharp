using BenchmarkDotNet.Running;
using DoomSharp.Benchmarks.Runner.Workloads;

var test = new UnsafeVsSafe();

// BenchmarkRunner.Run<ArrayClearVsNewInstance>();
// BenchmarkRunner.Run<StructsVsClasses>();
BenchmarkRunner.Run<UnsafeVsSafe>();