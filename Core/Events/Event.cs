using System;

namespace Rocket.Core
{
    /// <summary>
    /// Base event type passed to intrinsic element event handlers.
    /// </summary>
    public class Event
    {
        public object Target { get; set; }
        public bool Bubbles { get; set; } = true;
        public bool Cancelable { get; set; } = true;

        private bool _propagationStopped;
        private bool _defaultPrevented;

        public bool IsPropagationStopped => _propagationStopped;
        public bool PropagationStopped => _propagationStopped;
        public bool DefaultPrevented => _defaultPrevented;

        public void StopPropagation()
        {
            _propagationStopped = true;
        }

        /// <summary>
        /// Alias for <see cref="StopPropagation"/>.
        /// </summary>
        public void PreventPropagation()
        {
            StopPropagation();
        }

        public void PreventDefault()
        {
            _defaultPrevented = true;
        }

        /// <summary>
        /// Indicates whether propagation has been stopped.
        /// </summary>
        public bool IsPropagationStoppedOrPrevented => _propagationStopped;
    }

    /// <summary>
    /// Mouse event.
    /// </summary>
    public class MouseEvent : Event
    {
        public int ClientX { get; set; }
        public int ClientY { get; set; }
        public int Button { get; set; }
    }

    /// <summary>
    /// Keyboard event.
    /// </summary>
    public class KeyboardEvent : Event
    {
        public string Key { get; set; }
        public string Code { get; set; }
        public bool CtrlKey { get; set; }
        public bool ShiftKey { get; set; }
        public bool AltKey { get; set; }
        public bool MetaKey { get; set; }
    }

    /// <summary>
    /// Change event.
    /// </summary>
    public class ChangeEvent : Event
    {
        public object Value { get; set; }
    }

    /// <summary>
    /// Input event.
    /// </summary>
    public class InputEvent : Event
    {
        public string Data { get; set; }
        public object Value { get; set; }
    }
}

namespace Rocket.Core.DOM
{
    /// <summary>
    /// DOM-namespace alias for <see cref="Rocket.Core.Event"/>.
    /// </summary>
    public class Event : Rocket.Core.Event { }
}