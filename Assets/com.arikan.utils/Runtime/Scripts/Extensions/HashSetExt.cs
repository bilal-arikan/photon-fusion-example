using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class ArikanExtensions
{
    public static T Pop<T>(this HashSet<T> set)
    {
        var e = set.GetEnumerator();
        if (e.MoveNext())
        {
            set.Remove(e.Current);
            return e.Current;
        }

        throw new System.ArgumentException("HashSet must not be empty.");
    }

    public static T Find<T>(this HashSet<T> sequence, Predicate<T> match)
    {
        foreach (var i in sequence)
            if (match(i))
            {
                return i;
            }
        return default(T);
    }
    public static List<T> FindAll<T>(this HashSet<T> sequence, Predicate<T> match)
    {
        List<T> temp = new List<T>();
        foreach (var i in sequence)
            if (match(i))
            {
                temp.Add(i);
            }
        return temp;
    }
}
