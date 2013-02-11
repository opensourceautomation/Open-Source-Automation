namespace OSAE.Roomba
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.IO;
    using RoombaSCI;
    using OSAE;

    public class Roomba : OSAEPluginBase
    {
        Logging logging = Logging.GetLogger("Roomba");
        RoombaAPI roomba = new RoombaAPI("COM8", 115200);
        short speed;
        List<RoombaAPI> roombas = new List<RoombaAPI>();

        public void ProcessCommand(System.Data.DataTable table)
        {
            System.Data.DataRow row = table.Rows[0];
            string command = row["method_name"].ToString();

            if (command == "CLEAN")
            {
                roomba.Throttle = speed;
                roomba.Start();
                roomba.Safe();
                roomba.Control();
                roomba.Clean();
            }

            if (command == "exit")
            {
                roomba.Off();
                roomba.Close();
                roomba.Dispose();
            }

            if (command == "off")
            {
                roomba.Throttle = speed;
                roomba.Off();
                roomba.Close();
            }

            if (command == "left")
            {
                roomba.Control();
                roomba.Left();
            }

            if (command == "halt" || command == "stop")
            {
                roomba.Control();
                roomba.Stop();
            }

            if (command == "motors on")
            {
                roomba.Control();
                roomba.Motors(3);
            }

            if (command == "motors off")
            {
                roomba.Control();
                roomba.Motors(0);
            }

            if (command == "speed up")
            {
                roomba.Throttle = Convert.ToInt16(roomba.Throttle + 25);
            }
            if (command == "slow down")
            {
                roomba.Throttle = Convert.ToInt16(roomba.Throttle - 25);
            }

            if (command == "spin left")
            {
                roomba.Control();
                roomba.SpinLeft();
            }

            if (command == "spin right")
            {
                roomba.Control();
                roomba.SpinRight();
            }

            if (command == "right")
            {
                roomba.Control();
                roomba.Right();
                System.Threading.Thread.Sleep(500);
            }

            if (command == "forward")
            {
                roomba.Control();
                roomba.Forward();
            }

            if (command == "backward")
            {
                roomba.Control();
                roomba.Backward();
            }
            if (command == "start")
            {
                roomba.Start();
                roomba.Safe();
                roomba.Control();
            }
            if (command == "connect")
            {
                roomba.Connect();
            }

            
        }

        public void RunInterface(string pluginName)
        {
            DataSet objs = osae.GetObjectsByType("ROOMBA");

            foreach(DataRow obj in objs.Tables[0].Rows)
            {
                roombas.Add(new RoombaAPI("COM"+osae.GetObjectProperty(obj["object_name"].ToString(),"Port"),115200));
            }
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public void PollSensorData()
        {
                SensorData data = roomba.Sensors;
                if (data != null)
                {
                    Console.WriteLine(data.Temperature.ToString());
                    osae.ObjectPropertySet(

                }
            }

            if (command == "get distance")
            {
                SensorData data = roomba.Sensors();
                if (data != null)
                {
                    Console.WriteLine(data.Distance.ToString());
                    using (SpeechSynthesizer speak = new SpeechSynthesizer())
                    {
                        speak.Speak(data.Distance.ToString() + " millimeters");
                    }
                }
            }

            if (command == "get charge")
            {
                SensorData data = roomba.Sensors();
                if (data != null)
                {
                    Console.WriteLine(data.Charge.ToString());
                    using (SpeechSynthesizer speak = new SpeechSynthesizer())
                    {
                        speak.Speak(data.Charge.ToString());
                    }
                }
            }

            if (command == "any virtual walls")
            {
                SensorData data = roomba.Sensors();
                if (data != null)
                {
                    string result = "No";
                    if (data.VirtualWall)
                    {
                        result = "Yes";
                        Console.WriteLine(result);
                    }
                    using (SpeechSynthesizer speak = new SpeechSynthesizer())
                    {
                        speak.Speak(result);
                    }
                }
            }
        }
    }
}
