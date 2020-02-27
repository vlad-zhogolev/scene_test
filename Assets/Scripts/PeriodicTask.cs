﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class PeriodicTask
{
    public static async Task Run(Action action, TimeSpan period, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(period, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
                action();
        }
    }

    public static Task Run(Action action, TimeSpan period)
    {
        return Run(action, period, CancellationToken.None);
    }
}
