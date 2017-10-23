using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WpfSampleProj2.Lib.Constants;
using WpfSampleProj2.Lib.Extensions;

namespace WpfSampleProj2.Lib.Helper
{
    public static class TryCatchHelper
    {
        public static Exception Execute(Action action)
        {
            var ex = _do(action);
            if(ex.IsNotNull())
            {
                Debug.Write(ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace);
            }
            return ex;
        }

        public static Exception Execute(Func<string> tag, Action action)
        {

            var ex = _do(action);

            if(ex.IsNotNull())
            {
                var ctx = tag.Call();
                if (ctx.IsNotEmpty())
                    ex.Data[ConstExceptions.Context] = ctx;

                Debug.Write(ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace);
            }

            return ex;
        }

        private static Exception _do(Action action)
        {
            Exception noexp = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                noexp = ex;
            }

            return noexp;
        }
    }



    
}
