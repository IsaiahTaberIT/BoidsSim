using UnityEngine;



namespace PersonalHelpers
{
    [System.Serializable ]
    public class SpecLog
    {
        public bool DoLogging = false;

        /// <summary>
        /// Only Logs to the console if a variable is set on the object
        /// </summary>
        /// <param name="message"></param>
        /// <param name="gameObject"></param>
        public void Log(object message, GameObject gameObject = null)
        {
            if (DoLogging)
            {
                if (gameObject != null)
                {
                    Debug.Log(message, gameObject);
                }
                else
                {
                    Debug.Log(message);

                }
            }

        }


        /// <summary>
        /// Only Logs to the console if a variable is set on the object
        /// </summary>
        /// <param name="label"></param>
        /// <param name="message"></param>
        /// <param name="gameObject"></param>
        public void Log(string lable,object message, GameObject gameObject = null)
        {
            if (DoLogging)
            {
                if (gameObject != null)
                {
                    Debug.Log(lable + " , " + message, gameObject);
                }
                else
                {
                    Debug.Log(lable + " , " + message);

                }
            }

        }


        /// <summary>
        /// Only Logs to the console if a variable is set on the object
        /// </summary>
        /// <param name="message"></param>
        /// <param name="gameObject"></param>
        public void LogWarning(object message, GameObject gameObject = null)
        {
            if (DoLogging)
            {
                if (gameObject != null)
                {
                    Debug.LogWarning(message, gameObject);
                }
                else
                {
                    Debug.LogWarning(message);

                }
            }

        }


        /// <summary>
        /// Only Logs to the console if a variable is set on the object
        /// </summary>
        /// <param name="message"></param>
        /// <param name="gameObject"></param>
        public void LogError(object message, GameObject gameObject = null)
        {
            if (DoLogging)
            {
                if (gameObject != null)
                {
                    Debug.LogError(message, gameObject);
                }
                else
                {
                    Debug.LogError(message);

                }
            }

        }

    }

}

