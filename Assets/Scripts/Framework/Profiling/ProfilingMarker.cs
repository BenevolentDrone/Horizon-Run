using System.Diagnostics;

namespace HereticalSolutions.Profiling
{
	public class ProfilingMarker
	{
		public string Name { get; private set; }
		
		public double ElapsedMilliseconds { get; private set; }

		public int Calls { get; private set; }

		public Stopwatch Stopwatch { get; private set; }

		public ProfilingMarker(string name)
		{
			Name = name;

			ElapsedMilliseconds = 0;

			Calls = 0;

			Stopwatch = new Stopwatch();
		}

		public void Start()
		{
			Calls++;

			Stopwatch.Reset();

			Stopwatch.Start();
		}

		public void Stop()
		{
			Stopwatch.Stop();

			ElapsedMilliseconds += Stopwatch.Elapsed.TotalMilliseconds;
		}

		public void Clear()
		{
			ElapsedMilliseconds = 0;

			Calls = 0;

			Stopwatch.Reset();
		}
	}
}