using System.Collections.Generic;
using System.Diagnostics;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Profiling
{
	public static class ProfilingManager
	{
		private static IRepository<string, ProfilingMarker> markers =
			RepositoryFactory.BuildDictionaryRepository<string, ProfilingMarker>();

		private static List<ProfilingMarkerDTO> report = new List<ProfilingMarkerDTO>();

		public static IEnumerable<ProfilingMarker> AllMarkers => markers.Values;

		public static IEnumerable<ProfilingMarkerDTO> Report => report;

		public static ProfilingMarker AllocateMarker(string name)
		{
			if (!markers.Has(name))
			{
				markers.Add(
					name,
					new ProfilingMarker(name));
			}

			return markers.Get(name);
		}

		public static void Flush()
		{
			report.Clear();

			foreach (var marker in markers.Values)
			{
				report.Add(
					new ProfilingMarkerDTO
					{
						Name = marker.Name,
						ElapsedMilliseconds = marker.ElapsedMilliseconds,
						Calls = marker.Calls
					});

				marker.Clear();
			}
		}
	}
}