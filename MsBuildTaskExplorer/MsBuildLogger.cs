using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MsBuildTaskExplorer
{
    internal class MsBuildLogger : Logger
    {
        public override void Initialize(IEventSource eventSource)
        {
            eventSource.ProjectStarted += EventSourceOnProjectStarted;
            eventSource.TaskStarted += EventSourceOnTaskStarted;
            eventSource.MessageRaised += EventSourceOnMessageRaised;
            eventSource.WarningRaised += EventSourceOnWarningRaised;
            eventSource.ErrorRaised += EventSourceOnErrorRaised;
            eventSource.ProjectFinished += EventSourceOnProjectFinished;
        }

        private void EventSourceOnProjectStarted(object sender, ProjectStartedEventArgs projectStartedEventArgs)
        {
            Debug.WriteLine("ProjectStarted: " + projectStartedEventArgs.Message);
        }

        private void EventSourceOnTaskStarted(object sender, TaskStartedEventArgs taskStartedEventArgs)
        {
            Debug.WriteLine("TaskStarted: " + taskStartedEventArgs.Message);
        }

        private void EventSourceOnMessageRaised(object sender, BuildMessageEventArgs buildMessageEventArgs)
        {
            Debug.WriteLine(buildMessageEventArgs.Message);
        }

        private void EventSourceOnWarningRaised(object sender, BuildWarningEventArgs buildWarningEventArgs)
        {
            Debug.WriteLine("Warning: " + buildWarningEventArgs.Message);
        }

        private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs buildErrorEventArgs)
        {
            Debug.WriteLine("Error: " + buildErrorEventArgs.Message);
        }

        private void EventSourceOnProjectFinished(object sender, ProjectFinishedEventArgs projectFinishedEventArgs)
        {
            Debug.WriteLine("ProjectFinished: " + projectFinishedEventArgs.Message);
        }
    }
}
