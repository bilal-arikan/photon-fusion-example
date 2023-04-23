using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class UniTaskExt
{
    public static async UniTask Then<T>(this UniTask<T> task, Action<T> continuationFunction)
    {
        continuationFunction(await task);
    }

    public static async UniTask Then<T>(this UniTask<T> task, Func<T, UniTask> continuationFunction)
    {
        await continuationFunction(await task);
    }

    public static async UniTask<TR> Then<T, TR>(this UniTask<T> task, Func<T, TR> continuationFunction)
    {
        return continuationFunction(await task);
    }

    public static async UniTask<TR> Then<T, TR>(this UniTask<T> task, Func<T, UniTask<TR>> continuationFunction)
    {
        return await continuationFunction(await task);
    }
    public static async UniTask Then(this UniTask task, Action continuationFunction)
    {
        await task;
        continuationFunction();
    }

    public static async UniTask Then(this UniTask task, Func<UniTask> continuationFunction)
    {
        await task;
        await continuationFunction();
    }

    public static async UniTask<T> Then<T>(this UniTask task, Func<T> continuationFunction)
    {
        await task;
        return continuationFunction();
    }

    public static async UniTask<T> Then<T>(this UniTask task, Func<UniTask<T>> continuationFunction)
    {
        await task;
        return await continuationFunction();
    }


    public static async UniTask Catch(this UniTask task, Action<Exception> continuationFunction)
    {
        try
        {
            await task;
        }
        catch (System.Exception e)
        {
            continuationFunction(e);
        }
    }

    public static async UniTask<T> Catch<T>(this UniTask<T> task, Func<Exception, T> continuationFunction)
    {
        try
        {
            return await task;
        }
        catch (System.Exception e)
        {
            return continuationFunction(e);
        }
    }

    public static async UniTask<T> Catch<T>(this UniTask<T> task, Action<Exception> continuationFunction)
    {
        try
        {
            return await task;
        }
        catch (System.Exception e)
        {
            continuationFunction(e);
            return default(T);
        }
    }


    public static async UniTask Finally(this UniTask task, Action continuationFunction)
    {
        try
        {
            await task;
        }
        finally
        {
            continuationFunction();
        }
    }

    public static async UniTask<T> Finally<T>(this UniTask<T> task, Action continuationFunction)
    {
        try
        {
            return await task;
        }
        finally
        {
            continuationFunction();
        }
    }

    public static async UniTask PlayAndWaitToStop(this ParticleSystem ps)
    {
        ps.Play();
        await new WaitUntil(() => ps.isStopped);
    }
}