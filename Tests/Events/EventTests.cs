using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using Xunit;

namespace Tests
{
    public class EventTests
    {
        [Fact]
        public void StopPropagation_SetsFlag()
        {
            var ev = new Rocket.Core.Event();
            Assert.False(ev.IsPropagationStopped);
            ev.StopPropagation();
            Assert.True(ev.IsPropagationStopped);
            Assert.True(ev.PropagationStopped);
        }

        [Fact]
        public void PreventPropagation_AliasSetsFlag()
        {
            var ev = new Rocket.Core.Event();
            ev.PreventPropagation();
            Assert.True(ev.IsPropagationStopped);
        }

        [Fact]
        public void PreventDefault_SetsFlag()
        {
            var ev = new Rocket.Core.Event();
            ev.PreventDefault();
            Assert.True(ev.DefaultPrevented);
        }

        [Fact]
        public void IntrinsicProps_SupportEventHandlers()
        {
            bool clicked = false;
            var props = new HTMLButtonElementProps
            {
                OnClick = e => { clicked = true; e.StopPropagation(); }
            };
            props.OnClick(new Rocket.Core.Event());
            Assert.True(clicked);
        }
    }
}