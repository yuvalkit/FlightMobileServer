using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public class ScreenshotManager
    {
        private HttpClient myClient;
        private string ip;
        private int port;
        private string url;

        public ScreenshotManager(string ipInput, int portInput)
        {
            myClient = new HttpClient();
            ip = ipInput;
            port = portInput;
            url = "http://" + ip + ":" + port.ToString() + "/screenshot";
        }

        public async Task<byte[]> GetScreenshotData()
        {
            try
            {
                // getting a response from the url
                HttpResponseMessage response = await myClient.GetAsync(url);
                // get the data bytes from the response
                var screenshotData = await response.Content.ReadAsByteArrayAsync();
                // check if the data is a valid image
                if (!IsValidImage(screenshotData))
                {
                    return null;
                }
                return screenshotData;
            }
            catch
            {
                return null;
            }
        }

        private bool IsValidImage(byte[] imageData)
        {
            try
            {
                // try to convert the image data to a valid image
                ConvertImageData(imageData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ConvertImageData(byte[] imageData)
        {
            // convert the given image data to a valid image
            using (var memoryStream = new MemoryStream(imageData))
            {
                using (var bitmapImage = new Bitmap(memoryStream)) { }
            }
        }
    }
}
