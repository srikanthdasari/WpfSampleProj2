using System;
using System.Collections.Generic;
using System.Text;

namespace WpfSampleProj2.Lib.Extensions
{
    public static class ActionExtensions
    {
        public static void Call(this Action action)
        {
            action.IfNotNull(x => x());
        }

        public static void Call<T>(this Action<T> action, T param)
        {
            action.IfNotNull(x => x(param));
        }

        public static void Call<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
        {
            action.IfNotNull(x => x(param1, param2));
        }

    }
    public static class FuncExtensions
    {
        public static T Call<T>(this Func<T> action)
        {
            return action.IfNotNull(x => x());
        }
    }
}
