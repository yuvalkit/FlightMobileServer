using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public class TelnetClient
    {
        private Socket mySocket;

        public bool Connect(string ip, int port)
        {
            // open a socket
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
            try
            {
                // trying to connect with the socket
                mySocket.Connect(ip, port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string Receive()
        {
            byte[] rcvBuffer = new byte[1024];
            try
            {
                // trying to receive data with the socket
                int numberOfBytes = mySocket.Receive(rcvBuffer);
                return Encoding.ASCII.GetString(rcvBuffer, 0, numberOfBytes);
            }
            catch
            {
                return Utils.error;
            }
        }

        public bool Send(string data)
        {
            byte[] msgToSend = Encoding.ASCII.GetBytes(data);
            try
            {
                // trying to send data with the socket
                mySocket.Send(msgToSend);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
