using Rocket.Core;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using static Rocket.Core.Hooks;
using Xunit;

namespace Tests
{
    public class HooksTests
    {
        [Fact]
        public void State_ReturnsInitialValue()
        {
            var (value, _) = State(42);
            Assert.Equal(42, value);
        }

        [Fact]
        public void State_SetterUpdatesStoredValue()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            var (value, setValue) = State(5);
            Assert.Equal(5, value);

            setValue(10);

            instance.HookIndex = 0;
            var (value2, _) = State(5);
            Assert.Equal(10, value2);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void SideEffect_RunsOnFirstRender()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            int runs = 0;
            SideEffect(() => { runs++; }, null);
            Assert.Equal(1, runs);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void SideEffect_DoesNotRerunWithSameDependencies()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            int runs = 0;
            SideEffect(() => { runs++; }, new object[] { 1 });

            instance.HookIndex = 0;
            SideEffect(() => { runs++; }, new object[] { 1 });

            Assert.Equal(1, runs);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void SideEffect_RerunsWhenDependenciesChange()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            int runs = 0;
            SideEffect(() => { runs++; }, new object[] { 1 });

            instance.HookIndex = 0;
            SideEffect(() => { runs++; }, new object[] { 2 });

            Assert.Equal(2, runs);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void SideEffect_SupportsCleanup()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            int cleanups = 0;
            SideEffect(() => { return () => { cleanups++; }; }, new object[] { 1 });

            instance.HookIndex = 0;
            SideEffect(() => { return () => { cleanups++; }; }, new object[] { 2 });

            Assert.Equal(1, cleanups);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Context_ReturnsDefaultWithoutProvider()
        {
            var ctx = CreateContext("default");
            Assert.Equal("default", UseContext(ctx));
        }

        [Fact]
        public void Context_ReturnsProvidedValue()
        {
            var ctx = CreateContext("default");
            ComponentContext.PushContext(ctx, "provided");
            Assert.Equal("provided", UseContext(ctx));
            ComponentContext.PopContext();
            Assert.Equal("default", UseContext(ctx));
        }

        [Fact]
        public void Provider_PushesAndPopsContext()
        {
            var ctx = CreateContext("default");
            var provider = new Provider<string>
            {
                Props = new ProviderProps<string> { Context = ctx, Value = "provided" }
            };

            provider.PushContext();
            Assert.Equal("provided", UseContext(ctx));
            provider.PopContext();
            Assert.Equal("default", UseContext(ctx));
        }

        [Fact]
        public void UseState_AliasWorks()
        {
            var (value, _) = UseState(99);
            Assert.Equal(99, value);
        }

        [Fact]
        public void UseEffect_AliasWorks()
        {
            int runs = 0;
            UseEffect(() => { runs++; }, null);
            Assert.Equal(1, runs);
        }
    }
}