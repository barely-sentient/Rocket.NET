using System;
using Rocket.Core;

namespace Rocket.Core.Components
{
    /// <summary>
    /// Common attributes shared by all intrinsic HTML elements.
    /// </summary>
    public class IntrinsicAttributes
    {
        public string Id { get; set; }
        public string ClassName { get; set; }
        public string Style { get; set; }
        public string Title { get; set; }
        public string Lang { get; set; }
        public string Dir { get; set; }
        public bool Hidden { get; set; }
        public int TabIndex { get; set; }
        public bool Draggable { get; set; }
        public string ContentEditable { get; set; }
        public string AccessKey { get; set; }
        public bool SpellCheck { get; set; }
        public string Translate { get; set; }
        public string Role { get; set; }
        public string AriaLabel { get; set; }
        public string DataId { get; set; }

        public Action<Event> OnClick { get; set; }
        public Action<Event> OnDblClick { get; set; }
        public Action<Event> OnMouseDown { get; set; }
        public Action<Event> OnMouseUp { get; set; }
        public Action<Event> OnMouseEnter { get; set; }
        public Action<Event> OnMouseLeave { get; set; }
        public Action<Event> OnMouseOver { get; set; }
        public Action<Event> OnMouseOut { get; set; }
        public Action<Event> OnMouseMove { get; set; }
        public Action<Event> OnKeyDown { get; set; }
        public Action<Event> OnKeyUp { get; set; }
        public Action<Event> OnKeyPress { get; set; }
        public Action<Event> OnChange { get; set; }
        public Action<Event> OnInput { get; set; }
        public Action<Event> OnFocus { get; set; }
        public Action<Event> OnBlur { get; set; }
        public Action<Event> OnSubmit { get; set; }
        public Action<Event> OnLoad { get; set; }
        public Action<Event> OnError { get; set; }
        public Action<Event> OnContextMenu { get; set; }
        public Action<Event> OnWheel { get; set; }
    }
}