using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public class CommandManager
    {
        private TelnetClient myClient;
        private BlockingCollection<AsyncCommand> commandQueue;
        private string aileronString;
        private string elevatorString;
        private string rudderString;
        private string throttleString;
        private string ip;
        private int port;
        private bool connected;

        public CommandManager(string ipInput, int portInput)
        {
            myClient = new TelnetClient();
            commandQueue = new BlockingCollection<AsyncCommand>();
            ip = ipInput;
            port = portInput;
            aileronString = "/controls/flight/aileron";
            elevatorString = "/controls/flight/elevator";
            rudderString = "/controls/flight/rudder";
            throttleString = "/controls/engines/current-engine/throttle";
            connected = true;
            Start();
        }

        public Task<bool> Execute(Command command)
        {
            // create an async command from the command and add it to the queue
            var asyncCommand = new AsyncCommand(command);
            commandQueue.Add(asyncCommand);
            return asyncCommand.Task;
        }

        public bool IsConnected()
        {
            return connected;
        }

        public void ProcessCommands()
        {
            // if the connection failed
            if (!myClient.Connect(ip, port))
            {
                connected = false;
            }
            if (!myClient.Send("data\r\n"))
            {
                connected = false;
            }
            // do all the queue command
            foreach (AsyncCommand command in commandQueue.GetConsumingEnumerable())
            {
                bool result = SendCommand(command.Command);
                command.Completion.SetResult(result);
            }
        }

        public void Start()
        {
            Task.Factory.StartNew(ProcessCommands);
        }

        private string SetCommand(double value, string property)
        {
            // make a set command
            return "set " + GetPropertyPath(property) + " " + value.ToString() + "\r\n";
        }

        private string GetCommand(string property)
        {
            // make a get command
            return "get " + GetPropertyPath(property) + "\r\n";
        }

        private string GetPropertyPath(string property)
        {
            // get the property command path
            switch (property)
            {
                case "aileron":
                    return aileronString;
                case "elevator":
                    return elevatorString;
                case "rudder":
                    return rudderString;
                case "throttle":
                    return throttleString;
            }
            return "";
        }

        private bool SendAndCheckIfValid(double val, string property)
        {
            double receiveVal;
            string receiveWithoutNewLine;
            // send set and get to the simulator and check if failed
            string receive = SendSetAndGet(val, property);
            if (receive == Utils.error)
            {
                return false;
            }
            try
            {
                // remove new line chars
                receiveWithoutNewLine = Regex.Replace(receive, @"\n|\r", "");
                receiveVal = Double.Parse(receiveWithoutNewLine);
                // if the received value is not the sent value
                if ((receiveVal != val) && !IsValuesClose(receiveVal, val))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private string SendSetAndGet(double val, string property)
        {
            string setCommand = SetCommand(val, property);
            string getCommand = GetCommand(property);
            // send set command and check if failed
            if (!myClient.Send(setCommand))
            {
                return Utils.error;
            }
            // send get command and check if failed
            if (!myClient.Send(getCommand))
            {
                return Utils.error;
            }
            // return the received value
            return myClient.Receive();
        }

        private bool IsValuesClose(double value1, double value2)
        {
            // check if the values are close at the deviation range
            double deviation = 0.00001;
            if ((value1 < value2) && ((value1 + deviation) < value2))
            {
                return false;
            }
            if ((value1 > value2) && (value1 > (deviation + value2)))
            {
                return false;
            }
            return true;
        }

        public bool SendCommand(Command command)
        {
            double aileron = command.Aileron;
            // send the aileron value and check if failed
            if (!SendAndCheckIfValid(aileron, "aileron"))
            {
                return false;
            }
            double rudder = command.Rudder;
            // send the rudder value and check if failed
            if (!SendAndCheckIfValid(rudder, "rudder"))
            {
                return false;
            }
            double elevator = command.Elevator;
            // send the elevator value and check if failed
            if (!SendAndCheckIfValid(elevator, "elevator"))
            {
                return false;
            }
            double throttle = command.Throttle;
            // send the throttle value and check if failed
            if (!SendAndCheckIfValid(throttle, "throttle"))
            {
                return false;
            }
            return true;
        }
    }
}
