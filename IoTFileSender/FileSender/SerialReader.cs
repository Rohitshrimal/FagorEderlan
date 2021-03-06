﻿using FileSender.Helper;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSender
{
    public class SerialReader:IDisposable
    {
        IoTHelper hub;
        string port;

        SerialPort serialPort;
        private Queue<byte> recievedData = new Queue<byte>();
        public SerialReader(string port, IoTHelper hub)
        {
            this.port = port;
            this.hub = hub;
            serialPort = new SerialPort(port,115200,Parity.None,8,StopBits.One);
            serialPort.DtrEnable = true;
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var value=serialPort.ReadLine();
            dynamic values = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
            int light = values.light;
            float temp = values.temp;
            string tsvalues= Newtonsoft.Json.JsonConvert.SerializeObject(new { Light = light, Temp = temp, Timestamp = DateTime.Now });
            await hub.SendDataAsync(tsvalues);
        }

        public void Dispose()
        {
           Dispose(true);
        }

        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (serialPort != null)
                {
                    serialPort.Dispose();
                }
            }
        }

        ~SerialReader()
        {
            Dispose(false);
        }

    }
}
