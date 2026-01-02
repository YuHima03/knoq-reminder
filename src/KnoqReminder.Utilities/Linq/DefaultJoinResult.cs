using System;
using System.Collections.Generic;
using System.Text;

namespace KnoqReminder.Utilities.Linq;

public static class DefaultJoinResult
{
    public static DefaultJoinResult<TOuter, TInner> Create<TOuter, TInner>(TOuter outer, TInner inner)
        => new(outer, inner);

    public static ValueTuple<T1, T2> Flatten<T1, T2>(this DefaultJoinResult<T1, T2> @this)
        => (@this.Outer, @this.Inner);

    public static ValueTuple<T1, T2, T3> Flatten<T1, T2, T3>(this DefaultJoinResult<DefaultJoinResult<T1, T2>, T3> @this)
        => (@this.Outer.Outer, @this.Outer.Inner, @this.Inner);

    public static ValueTuple<T1, T2, T3, T4> Flatten<T1, T2, T3, T4>(this DefaultJoinResult<DefaultJoinResult<DefaultJoinResult<T1, T2>, T3>, T4> @this)
        => (@this.Outer.Outer.Outer, @this.Outer.Outer.Inner, @this.Outer.Inner, @this.Inner);
}

public record struct DefaultJoinResult<TOuter, TInner>(
    TOuter Outer,
    TInner Inner);
