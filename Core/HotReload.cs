using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Core
{
    /// <summary>
    /// Abstraction implemented by a host (such as an AppWindow) that can
    /// re-run the component tree in response to a .NET hot reload update.
    /// </summary>
    public interface IHotReloadTarget
    {
        void HotReloadComponents(IEnumerable<Type> updatedTypes);
    }

    /// <summary>
    /// .NET hot reload entry point. Referenced by application assemblies via
    /// <see cref="System.Reflection.Metadata.MetadataUpdateHandlerAttribute"/>.
    /// Moved from the OpenGL renderer so it is renderer agnostic.
    /// </summary>
    public static class HotReloadManager
    {
        /// <summary>
        /// The currently registered host that receives hot reload updates.
        /// </summary>
        public static IHotReloadTarget Current { get; set; }

        /// <summary>
        /// Alias for <see cref="Current"/>.
        /// </summary>
        public static IHotReloadTarget CurrentTarget { get => Current; set => Current = value; }

        /// <summary>
        /// Alias for <see cref="Current"/>.
        /// </summary>
        public static IHotReloadTarget Target { get => Current; set => Current = value; }

        /// <summary>
        /// The currently registered executor, used when no host is set.
        /// </summary>
        public static VirtualDomExecutor CurrentExecutor { get; set; }

        /// <summary>
        /// Alias for <see cref="CurrentExecutor"/>.
        /// </summary>
        public static VirtualDomExecutor Executor { get => CurrentExecutor; set => CurrentExecutor = value; }

        /// <summary>
        /// Optional fallback callback used when no <see cref="Current"/> host is set.
        /// </summary>
        public static Action<IEnumerable<Type>> HotReloadAction { get; set; }

        /// <summary>
        /// Alias for <see cref="HotReloadAction"/>.
        /// </summary>
        public static Action<IEnumerable<Type>> Handler { get => HotReloadAction; set => HotReloadAction = value; }

        public static void UpdateApplication(Type[] updatedTypes)
        {
            Console.WriteLine($"[HotReload] UpdateApplication: {(updatedTypes == null ? "(null)" : string.Join(", ", (object[])updatedTypes))}");
            if (Current != null)
            {
                Current.HotReloadComponents(updatedTypes);
                return;
            }
            if (HotReloadAction != null)
            {
                HotReloadAction(updatedTypes);
                return;
            }
            if (CurrentExecutor != null)
            {
                CurrentExecutor.Refresh(updatedTypes);
                return;
            }
            // No host registered; nothing to do.
        }

        public static void UpdateApplication(IEnumerable<Type> updatedTypes)
        {
            UpdateApplication(updatedTypes == null ? null : updatedTypes.ToArray());
        }

        public static void ClearCache(Type[] updatedTypes)
        {
            // No-op; keep hook/state caches as-is.
        }

        public static void ClearCache(IEnumerable<Type> updatedTypes)
        {
            ClearCache(updatedTypes == null ? null : updatedTypes.ToArray());
        }
    }
}
