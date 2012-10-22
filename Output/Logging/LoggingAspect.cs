using System;
using log4net;
using PostSharp.Laos;

namespace FT.Output.Logging
{
    [Serializable]
    public class LoggingAspect : OnMethodBoundaryAspect
    {
        private static string GetParameterString(object[] parameters)
        {
            string list = String.Empty;
            if (parameters != null)
            {                
                for (int i = 0; i < parameters.Length; i++)
                {
                    list += ((parameters[i].ToString() == parameters[i].GetType().FullName) ? 
                        parameters[i].GetType().Name : parameters[i].ToString());

                    if (i < parameters.Length - 1)
                    {
                        list += ", ";
                    }
                }
                if (list != String.Empty)
                {
                    return "[" + list + "]";
                }
            }
            return list;
        }

        public override void OnEntry(MethodExecutionEventArgs eventArgs)
        {            
            ILog log = LogManager.GetLogger(eventArgs.Method.DeclaringType.FullName);
            log.Debug("Invoking " + eventArgs.Method.Name + " " + 
                GetParameterString(eventArgs.GetReadOnlyArgumentArray()));            
        }

        public override void OnException(MethodExecutionEventArgs eventArgs)
        {            
            ILog log = LogManager.GetLogger(eventArgs.Method.DeclaringType.FullName);
            log.Warn("Caught Error " + eventArgs.Method.Name + ": " + eventArgs.Exception.Message);
        }

        public override void OnSuccess(MethodExecutionEventArgs eventArgs)
        {
            ILog log = LogManager.GetLogger(eventArgs.Method.DeclaringType.FullName);
            log.Debug("Completed " + eventArgs.Method.Name);            
        }
    }
}
