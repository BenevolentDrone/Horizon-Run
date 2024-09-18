using System;
using System.Collections.Generic;
using System.Linq;

using DefaultEcs.System;

#if USE_PROFILING_MARKERS
using Unity.Profiling;
#endif

namespace HereticalSolutions.Entities
{
    /// <summary>
    /// Represents a collection of <see cref="ISystem{T}"/> to update sequentially.
    /// </summary>
    /// <typeparam name="T">The type of the object used as state to update the systems.</typeparam>
    [IgnoreProfiling]
    public class SequentialSystemWithMarkers<T> : ISystem<T>
    {
        #region Fields

        private readonly ISystem<T>[] systems;
        
#if USE_PROFILING_MARKERS
        private readonly ProfilerMarker[] markers;
        
        private readonly bool[] usageMask;
#endif

        #endregion

        #region Initialisation

        /// <summary>
        /// Initialises a new instance of the <see cref="SequentialSystem{T}"/> class.
        /// </summary>
        /// <param name="systems">The <see cref="ISystem{T}"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="systems"/> is null.</exception>
        public SequentialSystemWithMarkers(IEnumerable<ISystem<T>> systems)
        {
            this.systems = (systems ?? throw new ArgumentNullException(nameof(systems))).Where(s => s != null).ToArray();
            
#if USE_PROFILING_MARKERS

            markers = new ProfilerMarker[this.systems.Length];
            
            usageMask = new bool[this.systems.Length];

            for (int i = 0; i < markers.Length; i++)
            {
                var systemType = this.systems[i].GetType();

                string markerName = systemType.Name;
                
                markers[i] = new ProfilerMarker(markerName);
                
                usageMask[i] = !systemType
                    .IsDefined(
                        typeof(IgnoreProfilingAttribute),
                        false);
                
                if (usageMask[i]
                    && !MarkersRepository.Markers.Has(markerName))
                {
                    MarkersRepository.Markers.Add(
                        markerName,
                        markers[i]);
                }
            }
#endif
            
            IsEnabled = true;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SequentialSystem{T}"/> class.
        /// </summary>
        /// <param name="systems">The <see cref="ISystem{T}"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="systems"/> is null.</exception>
        public SequentialSystemWithMarkers(params ISystem<T>[] systems)
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
                //foreach (ISystem<T> system in systems)
                for (int i = 0; i < systems.Length; i++)
                {
#if USE_PROFILING_MARKERS
                    if (usageMask[i])
                    {
                        using (markers[i].Auto()) //To ensure it stays in release build
                        {

                        //markers[i].Begin();
#endif

                    systems[i].Update(state);
                            
#if USE_PROFILING_MARKERS
                        }
                        //markers[i].End();
                    }
                    else
                    {
                        systems[i].Update(state);
                    }
#endif
                }
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
