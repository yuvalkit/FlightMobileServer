using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public class AsyncCommand
    {
        public Command Command { get; private set; }

        public TaskCompletionSource<bool> Completion { get; private set; }

        public Task<bool> Task { get => Completion.Task; }

        public AsyncCommand(Command commandInput)
        {
            Command = commandInput;
            Completion = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
