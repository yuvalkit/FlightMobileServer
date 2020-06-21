using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlightMobileApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private CommandManager manager;
        private string invalidCommandError;
        private string invalidSendError;
        private string connectionError;
        private Utils myUtils;

        public CommandController(CommandManager managerInput)
        {
            manager = managerInput;
            invalidCommandError = "The values sent to the simulator are invalid";
            invalidSendError = "Failed sending values to the simulator";
            connectionError = "The simulator is not connected";
            myUtils = new Utils();
        }

        // POST: api/command
        [HttpPost]
        public async Task<ActionResult> PostCommand([FromBody] JsonElement commandJson)
        {
            Command command;
            string commandString = commandJson.ToString();
            // check if the simulator is connected
            if (!manager.IsConnected())
            {
                return BadRequest(connectionError);
            }
            var commandJObject = JObject.Parse(commandString);
            // check if the command is valid
            if (!myUtils.IsCommandJObjectValid(commandJObject))
            {
                return BadRequest(invalidCommandError);
            }
            try
            {
                // trying to deserialize the string to a command
                command = JsonConvert.DeserializeObject<Command>(commandString);
            }
            catch
            {
                return BadRequest(invalidCommandError);
            }
            // send the command to the simulator and wait for the result
            bool result = await manager.Execute(command);
            // if sending the command failed
            if (!result)
            {
                return BadRequest(invalidSendError);
            }
            return Ok(command);
        }
    }
}
