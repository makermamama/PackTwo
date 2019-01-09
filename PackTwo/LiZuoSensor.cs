using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace winformVision
{
    //立卓传感器
    class LiZuoSensor
    {
        //获取1传感器状态命令
        private static string cmdGetStautesOne = "S(00,30,1)";

        //触发
        private static string triggerOne  = "IN(1000)";

        //未触发
        private static string noTriggerOne = "IN(0000)";


        //获取2传感器状态命令
        private static string cmdGetStautesTwo  = "S(00,32,1)";

        //触发
        private static string triggerTwo = "IN(0100)";

        //未触发
        private static string noTriggerTwo = "IN(0000)";

        //绿灯灭
        private static string greenOff = "S(00,01,0)";
        //绿灯亮
        private static string greenON  = "S(00,01,1)";

        //红灯灭
        private static string redOff = "S(00,02,0)";
        //红灯亮
        private static string redON = "S(00,02,1))";

        //黄灯灭
        private static string yellowOff = "S(00,03,0)";
        //黄灯亮
        private static string yellowON = "S(00,03,1))";

        //剔除关
        private static string rejectOff = "S(00,04,0)";
        //剔除开
        private static string rejectON = "S(00,04,1))";

        private const UInt16  OpenGreen = 1;

        private const UInt16  OpenRed   = 2;

        private const UInt16 OpenYellow = 3;

        private const UInt16 AllClose = 4;

        private const UInt16 Reject = 5;

        private const UInt16 TreandSleep = 50;

        public static string CmdGetStautesOne { get => cmdGetStautesOne; set => cmdGetStautesOne = value; }
        public static string TriggerOne { get => triggerOne; set => triggerOne = value; }
        public static string NoTriggerOne { get => noTriggerOne; set => noTriggerOne = value; }
        public static string GreenOff { get => greenOff; set => greenOff = value; }
        public static string GreenON { get => greenON; set => greenON = value; }
        public static string RedOff { get => redOff; set => redOff = value; }
        public static string RedON { get => redON; set => redON = value; }
        public static string YellowOff { get => yellowOff; set => yellowOff = value; }
        public static string YellowON { get => yellowON; set => yellowON = value; }
        public static string CmdGetStautesTwo { get => cmdGetStautesTwo; set => cmdGetStautesTwo = value; }
        public static string TriggerTwo { get => triggerTwo; set => triggerTwo = value; }
        public static string NoTriggerTwo { get => noTriggerTwo; set => noTriggerTwo = value; }

        //发送指令线程
        Thread SendCmdThread;

        private Boolean TheadOver = false;

        Queue<UInt16> CmdQueue = new Queue<UInt16>();

        //串口通信
        private SerialPort ComDevice;

        //构造函数
        public LiZuoSensor() {
        }

        //打开串口
        public String  OpenPort(String ComName)
        {
            if (ComName == "") {
                return "please input com name";
            }

            if(ComDevice == null)
            {
                ComDevice = new SerialPort();
                ComDevice.BaudRate = 9600;
                ComDevice.PortName = ComName;
                ComDevice.DataBits = 8;
                ComDevice.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            }
            if (ComDevice.IsOpen)
            {
                return ComDevice.PortName + "串口已经开启";
            }
            else {
                try
                {
                    ComDevice.Open();
                    TheadStart();
                    return ComDevice.PortName+"开启成功";
                 }catch(Exception ex) {
                    return ex.ToString();
                }
            }
        }

        private String R_Tags = "";

        private Boolean FirstOne = false;

        private Boolean FirstTwo = false;

        private Boolean FirstOneLeave = false;

        private Boolean FirstTwoLeave = false;

        private Boolean SecodeOne = false;

        private Boolean SecodeTwo = false;

        //定义一个委托
        public delegate void delegateTriggerOne();
        //传感器1触发
        public event delegateTriggerOne OneTriggerEvent;

        //定义一个委托
        public delegate void delegateTriggerOneLeave();
        //传感器1触发
        public event delegateTriggerOneLeave OneTriggerLeaveEvent;

        //定义一个委托
        public delegate void delegateTriggerTwo();
        //传感器2触发
        public event delegateTriggerTwo TwoTriggerEvent;



        //串口接受数据
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] ReDatas = new byte[ComDevice.BytesToRead];
            //从串口读取数据
            ComDevice.Read(ReDatas, 0, ReDatas.Length);
            //实现数据的解码与显示
            string str = Encoding.Default.GetString(ReDatas);

            R_Tags += str;
            
            //感应到物品
            if (R_Tags.Length >= 8)
            {
                Console.WriteLine(R_Tags);
                //判断传感器1的触发逻辑
                if (R_Tags == noTriggerOne)
                    FirstOne = true;

                if (FirstOne)
                {
                    if (R_Tags == TriggerOne)
                        FirstTwo = true;
                }

                //判断传感器1的离开触发
                if (R_Tags == TriggerOne )
                    FirstOneLeave = true;

                if (FirstOneLeave)
                {
                    if (R_Tags == noTriggerOne)
                        FirstTwoLeave = true;
                }

                //判断传感器2的触发逻辑
                if (R_Tags == TriggerTwo )
                    SecodeOne = true;

                if (SecodeOne)
                {
                    if (R_Tags == noTriggerTwo)
                        SecodeTwo = true;
                }

                if (FirstOne && FirstTwo)
                {
                    if (OneTriggerEvent != null)
                        OneTriggerEvent();
                    Console.WriteLine("传感器1进入事件发出"+ DateTime.Now.ToString());
                    FirstOne = false;
                    FirstTwo = false;
                }

                if (SecodeOne && SecodeTwo)
                {
                    if (TwoTriggerEvent != null)
                        TwoTriggerEvent();
                    SecodeOne = false;
                    SecodeTwo = false;
                }

                if(FirstOneLeave && FirstTwoLeave)
                {
                    if(OneTriggerLeaveEvent != null)
                       OneTriggerLeaveEvent();
                    Console.WriteLine("传感器1离开事件发出"+ DateTime.Now.ToString());
                    FirstOneLeave = false;
                    FirstTwoLeave = false;
                }
                R_Tags = "";
            }
        }

        //开启数据发送线程
        private void TheadStart()
        {
            SendCmdThread = new Thread(SendCmd);
            SendCmdThread.Start();
        }


        //发送命令
        private void SendCmd()
        {
            while (true && !TheadOver)
            {

                if (CmdQueue.Count > 0)
                {
                    Console.WriteLine(CmdQueue.Count);
                    UInt16 cmd;
                    lock (CmdQueue)
                    {
                        cmd = CmdQueue.Dequeue();
                    }
                    switch (cmd)
                    {
                        case OpenGreen:
                            RedLightOff();
                            Thread.Sleep(TreandSleep);
                            GreenLight();
                            Thread.Sleep(TreandSleep);
                            break;

                        case OpenRed:
                            GreenLightOff();
                            Thread.Sleep(TreandSleep);
                            //YellowLightOff();
                            //Thread.Sleep(TreandSleep);
                            RedLight();
                            Thread.Sleep(TreandSleep);
                            break;

                        case OpenYellow:
                            GreenLightOff();
                            Thread.Sleep(TreandSleep);
                            RedLightOff();
                            Thread.Sleep(TreandSleep);
                            YellowLight();
                            Thread.Sleep(TreandSleep);
                            break;

                        case Reject:
                            byte[] bufON = Encoding.ASCII.GetBytes(rejectON);
                            ComDevice.Write(bufON, 0, bufON.Length);
                            Thread.Sleep(TreandSleep*40);
                            byte[] bufoff = Encoding.ASCII.GetBytes(rejectOff);
                            ComDevice.Write(bufoff, 0, bufoff.Length);
                            Thread.Sleep(TreandSleep);
                            break;

                        case AllClose:
                            GreenLightOff();
                            Thread.Sleep(TreandSleep);
                            RedLightOff();
                            Thread.Sleep(TreandSleep);
                            break;
                    }
                }
                //队列中没有任务
                else
                {
                    SendSensorOne();
                    Thread.Sleep(TreandSleep);
                    //SendSensorTwo();
                    //Thread.Sleep(TreandSleep);
                }
            }
            if (ComDevice != null)
            {
                ComDevice.Close();
                ComDevice.Dispose();
                ComDevice = null;
            }
        }



        //红灯亮任务
        public void RedAlarms()
        {
            lock (CmdQueue)
            {
                UInt16 cmd = OpenRed;
                CmdQueue.Enqueue(cmd);
                
            }
        }
        //剔除任务
        public void RejectTask()
        {
            lock (CmdQueue)
            {
                CmdQueue.Enqueue(Reject);
            }
        }



        //绿灯亮任务
        public void GreenAlarms()
        {
            lock (CmdQueue)
            {
                UInt16 cmd = OpenGreen;
                CmdQueue.Enqueue(cmd);
            }
        }

        //黄灯亮任务
        public void YellowAlarms()
        {
            lock (CmdQueue)
            {
                UInt16 cmd = OpenYellow;
                CmdQueue.Enqueue(cmd);
            }
        }

        //全灭任务
        public void AlarmsNolight()
        {
            lock (CmdQueue)
            {
                UInt16 cmd = AllClose;
                CmdQueue.Enqueue(cmd);
            }
        }

        //红灯亮
        private void RedLight()
        {
            //红灯亮
            string send = redON;

            byte[] buf = Encoding.ASCII.GetBytes(send);

            ComDevice.Write(buf, 0, buf.Length);
        }

        //红灯灭
        private void RedLightOff()
        {
            //红灯亮
            string send = redOff;

            byte[] buf = Encoding.ASCII.GetBytes(send);

            ComDevice.Write(buf, 0, buf.Length);
        }

        //绿灯亮
        private void GreenLight()
        {
            //红灯亮
            string send = greenON;

            byte[] buf = Encoding.ASCII.GetBytes(send);

            ComDevice.Write(buf, 0, buf.Length);
        }

        //绿灯灭
        private void GreenLightOff()
        {
            //红灯亮
            string send = greenOff;

            byte[] buf = Encoding.ASCII.GetBytes(send);

            ComDevice.Write(buf, 0, buf.Length);
        }

        //黄灯亮
        private void YellowLight()
        {
            //红灯亮
            string send = yellowON;

            byte[] buf = Encoding.ASCII.GetBytes(send);

            ComDevice.Write(buf, 0, buf.Length);
        }

        //绿灯灭
        private void YellowLightOff()
        {
            //红灯亮
            string send = yellowOff;

            byte[] buf = Encoding.ASCII.GetBytes(send);

            ComDevice.Write(buf, 0, buf.Length);
        }



        //发送传感器1的读取命令
        private void SendSensorOne()
        {
            string send = cmdGetStautesOne;

            byte[] buf = Encoding.ASCII.GetBytes(send);

            ComDevice.Write(buf, 0, buf.Length);
        }

        //发送传感器2的读取命令
        private void SendSensorTwo()
        {
            string send = cmdGetStautesOne;

            byte[] buf = Encoding.ASCII.GetBytes(send);

            ComDevice.Write(buf, 0, buf.Length);
        }

        //释放资源
        public void Dispose() {
            if (SendCmdThread != null)
            {
                TheadOver = true;                
            }
        }
    }
}
