using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightMobileApp.Controllers
{
    [Route("screenshot")]
    [ApiController]
    public class ScreenshotController : ControllerBase
    {
        private ScreenshotManager manager;
        public ScreenshotController(ScreenshotManager managerInput)
        {
            manager = managerInput;
        }

        // GET: /screenshot
        [HttpGet]
        public async Task<IActionResult> GetScreenshot()
        {
            string failError = "Failed getting screenshot from the simulator";
            // trying to get a screenshot
            var screenshotData = await manager.GetScreenshotData();
            // if got invalid image
            if (screenshotData == null)
            {
                return BadRequest(failError);
            }
            try
            {
                // return the screenshot data as an image file
                return File(screenshotData, "Image/jpg");
            }
            catch
            {
                return BadRequest(failError);
            }
        }
    }
}
