using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WpfSampleProj2.Lib.Extensions
{
    public static class TryCatchExtensions
    {
        public static Exception Do(Action action)
        {
            var ex = _do(action);
            if(ex.IsNull())
            {
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
                return ex;
            }

            return noexp;
        }
    }



    
}
