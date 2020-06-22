using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public class Utils
    {
        // error status, the same error string as in the simulator
        static public string error = "ERR";

        public bool IsCommandJObjectValid(JObject commandJObject)
        {
            // check that the command has all the values names
            if (!(commandJObject.ContainsKey("aileron")
                && commandJObject.ContainsKey("rudder")
                && commandJObject.ContainsKey("elevator")
                && commandJObject.ContainsKey("throttle")))
            {
                return false;
            }
            // get all the values
            double aileron = commandJObject.Value<double>("aileron");
            double rudder = commandJObject.Value<double>("rudder");
            double elevator = commandJObject.Value<double>("elevator");
            double throttle = commandJObject.Value<double>("throttle");
            // check that all the values are valid
            if (!IsValidValues(aileron, rudder, elevator, throttle))
            {
                return false;
            }
            return true;
        }

        private bool IsValidValues(double aileron, double rudder, double elevator,
            double throttle)
        {
            // check that the aileron value is in the range [-1,1]
            if ((aileron > 1) || (aileron < -1))
            {
                return false;
            }
            // check that the rudder value is in the range [-1,1]
            if ((rudder > 1) || (rudder < -1))
            {
                return false;
            }
            // check that the elevator value is in the range [-1,1]
            if ((elevator > 1) || (elevator < -1))
            {
                return false;
            }
            // check that the throttle value is in the range [0,1]
            if ((throttle > 1) || (throttle < 0))
            {
                return false;
            }
            return true;
        }
    }
}
