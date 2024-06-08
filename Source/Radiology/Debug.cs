using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Radiology;

public static class Debug
{
    public static void Log(object o)
    {
        Verse.Log.Warning($"{o}");
    }

    public static string AsText(IEnumerable obj)
    {
        var builder = new StringBuilder();
        builder.Append("[ ");

        var first = true;
        foreach (var v in obj)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;

            if (v is IEnumerable enumerable)
            {
                builder.Append(AsText(enumerable));
            }
            else
            {
                builder.Append(v);
            }
        }

        builder.Append(" ]");

        return builder.ToString();
    }

    public static string AsText<A, B>(Dictionary<A, B> obj)
    {
        var builder = new StringBuilder();
        builder.Append("{ ");

        var first = true;


        foreach (var entry in obj)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;

            builder.Append(entry.Key);
            builder.Append(": ");
            if (entry.Value is IEnumerable value)
            {
                builder.Append(AsText(value));
            }
            else
            {
                builder.Append(entry.Value);
            }
        }

        builder.Append(" }");

        return builder.ToString();
    }
}