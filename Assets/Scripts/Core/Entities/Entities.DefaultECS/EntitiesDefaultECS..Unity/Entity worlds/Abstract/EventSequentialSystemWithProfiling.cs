using System;
using System.Collections.Generic;
using System.Linq;

using DefaultEcs.System;

#if USE_PROFILING_MARKERS || USE_PROFILING_SAMPLERS
using Unity.Profiling;
using UnityEngine.Profiling;
#endif

#if USE_PROFILING_CUSTOM_MARKERS
using HereticalSolutions.Profiling;
#endif

namespace HereticalSolutions.Entities
{
    /// <summary>
    /// Represents a collection of <see cref="ISystem{T}"/> to update sequentially.
    /// </summary>
    /// <typeparam name="T">The type of the object used as state to update the systems.</typeparam>
    [IgnoreProfiling]
    public class EventSequentialSystemWithProfiling<T, TEvent> : ISystem<T>
    {
        #region Fields

        private readonly ISystem<T>[] systems;
        
#if USE_PROFILING_MARKERS
        private readonly ProfilerMarker marker;
#elif USE_PROFILING_SAMPLERS
        private readonly CustomSampler sampler;
#elif USE_PROFILING_CUSTOM_MARKERS
        private readonly ProfilingMarker marker;
#endif

        #endregion

        #region Initialisation

        /// <summary>
        /// Initialises a new instance of the <see cref="SequentialSystem{T}"/> class.
        /// </summary>
        /// <param name="systems">The <see cref="ISystem{T}"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="systems"/> is null.</exception>
        public EventSequentialSystemWithProfiling(IEnumerable<ISystem<T>> systems)
        {
            this.systems = (systems ?? throw new ArgumentNullException(nameof(systems))).Where(s => s != null).ToArray();

#if USE_PROFILING_MARKERS || USE_PROFILING_SAMPLERS || USE_PROFILING_CUSTOM_MARKERS
            string markerName = $"{typeof(TEvent).Name} event system";
#endif
            
#if USE_PROFILING_MARKERS
            marker = new ProfilerMarker(markerName);
            
            if (!MarkersRepository.Markers.Has(markerName))
            {
                MarkersRepository.Markers.Add(
                    markerName,
                    marker);
            }
#elif USE_PROFILING_SAMPLERS
            sampler = CustomSampler.Create(markerName);
            
            if (!SamplersRepository.Samplers.Has(markerName))
            {
                SamplersRepository.Samplers.Add(
                    markerName,
                    sampler);
            }
#elif USE_PROFILING_CUSTOM_MARKERS
            marker = ProfilingManager.AllocateMarker(markerName);
#endif
            
            IsEnabled = true;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SequentialSystem{T}"/> class.
        /// </summary>
        /// <param name="systems">The <see cref="ISystem{T}"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="systems"/> is null.</exception>
        public EventSequentialSystemWithProfiling(params ISystem<T>[] systems)
            : this(systems as IEnumerable<ISystem<T>>)
        { }

        #endregion

        #region ISystem

        /// <summary>
        /// Gets or sets whether the current <see cref="SequentialSystem{T}"/> instance should update or not.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Updates all the systems once sequentially.
        /// </summary>
        /// <param name="state">The state to use.</param>
        public void Update(T state)
        {
            if (IsEnabled)
            {

#if USE_PROFILING_MARKERS
                using (marker.Auto()) //To ensure it stays in release build
                {
#elif USE_PROFILING_SAMPLERS
                sampler.Begin();
#elif USE_PROFILING_CUSTOM_MARKERS
                marker.Start();
#endif
                for (int i = 0; i < systems.Length; i++)
                {
                    systems[i].Update(state);
                }
#if USE_PROFILING_MARKERS
                }
#elif USE_PROFILING_SAMPLERS
                sampler.End();
#elif USE_PROFILING_CUSTOM_MARKERS
                marker.Stop();
#endif
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes all the inner <see cref="ISystem{T}"/> instances.
        /// </summary>
        public void Dispose()
        {
            for (int i = systems.Length - 1; i >= 0; --i)
            {
                systems[i].Dispose();
            }
        }

        #endregion
    }
}
