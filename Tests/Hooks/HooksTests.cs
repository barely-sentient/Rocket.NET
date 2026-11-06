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
            Assert.Equal("default", Context(ctx));
        }

        [Fact]
        public void Context_ReturnsProvidedValue()
        {
            var ctx = CreateContext("default");
            ComponentContext.PushContext(ctx, "provided");
            Assert.Equal("provided", Context(ctx));
            ComponentContext.PopContext();
            Assert.Equal("default", Context(ctx));
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
            Assert.Equal("provided", Context(ctx));
            provider.PopContext();
            Assert.Equal("default", Context(ctx));
        }

        [Fact]
        public void SideEffect_AliasWorks()
        {
            int runs = 0;
            SideEffect(() => { runs++; }, null);
            Assert.Equal(1, runs);
        }

        [Fact]
        public void Reducer_ReturnsInitialValue()
        {
            var (state, _) = Reducer((int s, int a) => s + a, 7);
            Assert.Equal(7, state);
        }

        [Fact]
        public void Reducer_DispatchUpdatesStateAcrossRerender()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            var (_, dispatch) = Reducer((int s, int a) => s + a, 0);
            dispatch(3);
            dispatch(4);

            instance.HookIndex = 0;
            var (state, _) = Reducer((int s, int a) => s + a, 0);
            Assert.Equal(7, state);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Reducer_InitOverloadComputesInitialState()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            var (state, _) = Reducer((int s, int a) => s + a, 5, (int x) => x * 2);
            Assert.Equal(10, state);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Memo_DoesNotRecomputeWithSameDependencies()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            int calls = 0;
            var first = Memo(() => { calls++; return calls; }, new object[] { 1 });
            Assert.Equal(1, first);

            instance.HookIndex = 0;
            var second = Memo(() => { calls++; return calls; }, new object[] { 1 });
            Assert.Equal(1, second);
            Assert.Equal(1, calls);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Memo_RecomputesWhenDependenciesChange()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            int calls = 0;
            var first = Memo(() => { calls++; return calls * 10; }, new object[] { 1 });
            Assert.Equal(10, first);

            instance.HookIndex = 0;
            var second = Memo(() => { calls++; return calls * 10; }, new object[] { 2 });
            Assert.Equal(20, second);
            Assert.Equal(2, calls);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Memo_StandaloneEvaluatesFactory()
        {
            int calls = 0;
            var value = Memo(() => { calls++; return 42; }, null);
            Assert.Equal(42, value);
            Assert.Equal(1, calls);
        }

        [Fact]
        public void Callback_PreservesIdentityWithSameDependencies()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            Action first = Callback(() => { }, new object[] { 1 });
            instance.HookIndex = 0;
            Action second = Callback(() => { }, new object[] { 1 });
            Assert.Same(first, second);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Callback_ChangesIdentityWhenDependenciesChange()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            Action first = Callback(() => { }, new object[] { 1 });
            instance.HookIndex = 0;
            Action second = Callback(() => { }, new object[] { 2 });
            Assert.NotSame(first, second);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Callback_GenericInfersDelegateType()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            var cb = Callback((int x) => x + 1, new object[] { 1 });
            Assert.Equal(6, cb(5));

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Ref_PreservesInstanceAndMutationsAcrossRerender()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            var first = Ref(0);
            first.Current = 42;

            instance.HookIndex = 0;
            var second = Ref(999);
            Assert.Same(first, second);
            Assert.Equal(42, second.Current);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void Ref_StandaloneCreatesMutableCell()
        {
            var r = Ref(5);
            Assert.Equal(5, r.Current);
            r.Current = 10;
            Assert.Equal(10, r.Current);
        }

        [Fact]
        public void Id_IsStableAcrossRerender()
        {
            var instance = new Instance();
            ComponentContext.CurrentInstance = instance;
            instance.ResetHooks();

            var first = Id();
            instance.HookIndex = 0;
            var second = Id();
            Assert.Equal(first, second);

            ComponentContext.CurrentInstance = null;
        }

        [Fact]
        public void DeferredValue_ReturnsInput()
        {
            Assert.Equal(123, DeferredValue(123));
        }

        [Fact]
        public void Transition_StartTransitionRunsAction()
        {
            int runs = 0;
            var (_, start) = Transition();
            start(() => runs++);
            Assert.Equal(1, runs);
        }
    }
}