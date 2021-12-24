using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HokmGame.Util
{
    public class CustomLogger : MonoBehaviour
    {
        void Awake()
        {
            Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            // Log log to your logging system.
            switch (type)
            {
                case LogType.Assert:
                    Debug.LogAssertion($"{condition}");
                    break;
                case LogType.Error:
                case LogType.Exception:
                    Debug.LogError($"{condition}, StackTrac: {stackTrace}");
                    break;
                case LogType.Log:
                    Debug.Log($"{condition}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"{condition}");
                    break;
                default:
                    Debug.Log($"{condition}");
                    break;
            }

        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception;
            var baseException = exception?.GetBaseException();
            Debug.LogException(exception);
        }

        // This function is called after all frame updates for the last frame of the object’s existence
        void OnDestroy()
        {
            Application.logMessageReceivedThreaded -= Application_logMessageReceivedThreaded;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }
    }

}
