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
        private readonly Action<string> _writeOutputAction;

        public MsBuildLogger(Action<string> writeOutputAction)
        {
            _writeOutputAction = writeOutputAction;
        }

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
            _writeOutputAction("ProjectStarted: " + projectStartedEventArgs.Message);
        }

        private void EventSourceOnTaskStarted(object sender, TaskStartedEventArgs taskStartedEventArgs)
        {
            _writeOutputAction("TaskStarted: " + taskStartedEventArgs.Message);
        }

        private void EventSourceOnMessageRaised(object sender, BuildMessageEventArgs buildMessageEventArgs)
        {
            _writeOutputAction(buildMessageEventArgs.Message);
        }

        private void EventSourceOnWarningRaised(object sender, BuildWarningEventArgs buildWarningEventArgs)
        {
            _writeOutputAction("Warning: " + buildWarningEventArgs.Message);
        }

        private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs buildErrorEventArgs)
        {
            _writeOutputAction("Error: " + buildErrorEventArgs.Message);
        }

        private void EventSourceOnProjectFinished(object sender, ProjectFinishedEventArgs projectFinishedEventArgs)
        {
            _writeOutputAction("ProjectFinished: " + projectFinishedEventArgs.Message);
        }
    }
}
