using UnityEngine;
using System;

namespace AssemblyCSharp
{
    public static class FP
    {
        public static T Log<T>(T o, Func<T, String> messageGenerator)
        {
            UnityEngine.Debug.Log(o != null ? o.ToString() + ": " + messageGenerator(o) : "null");
            return o;
        }

        public static T Log<T>(T o, string message)
        {
            UnityEngine.Debug.Log((o != null ? o.ToString() : "null") + ": " + message);
            return o;
        }

        public static T Log<T>(T o)
        {
            return Log(o, " ");
        }
    }
}

