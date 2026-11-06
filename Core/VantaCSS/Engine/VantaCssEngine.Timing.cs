using System;
using Rocket.Core.DOM;

namespace VantaCSS
{
    /// <summary>
    /// Timing management for <see cref="VantaCssEngine"/>.
    /// </summary>
    /// <remarks>
    /// The engine owns a clock (in seconds) so that transitions can be
    /// evaluated at any given point in time. Callers may either advance the
    /// clock with <see cref="Tick"/> or query a style at an explicit time.
    /// </remarks>
    public partial class VantaCssEngine
    {
        /// <summary>Current engine time in seconds.</summary>
        public double CurrentTime { get; set; }

        /// <summary>Advances the clock by <paramref name="seconds"/>.</summary>
        public void Tick(double seconds) => CurrentTime += seconds;

        /// <summary>Advances the clock by a <see cref="TimeSpan"/>.</summary>
        public void Tick(TimeSpan time) => CurrentTime += time.TotalSeconds;

        /// <summary>Sets the clock to an absolute value.</summary>
        public void SetTime(double seconds) => CurrentTime = seconds;

        /// <summary>Sets the clock from a <see cref="TimeSpan"/>.</summary>
        public void SetTime(TimeSpan time) => CurrentTime = time.TotalSeconds;

        /// <summary>Resets the clock and clears transition state.</summary>
        public void ResetTime()
        {
            CurrentTime = 0;
            ClearTransitions();
        }
    }
}
