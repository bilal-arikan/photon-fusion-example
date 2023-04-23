using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TimeExt
{
    public static TimeSpan Sum(this IEnumerable<TimeSpan> spans)
    {
        return TimeSpan.FromTicks(spans.Select(s => s.Ticks).Sum());
    }
}