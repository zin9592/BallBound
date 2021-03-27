using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace UdpServer
{
    class Program
    {
        static System.Timers.Timer sendTimer;
        static bool checkThread = true;
        static int recv = 0, bufferSize = 128;
        static byte[] data = new byte[128];
        static IPEndPoint ep = new IPEndPoint(IPAddress.Any, 20000);
        static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        static IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        static EndPoint remoteEP = (EndPoint)sender;
        static List<EndPoint> userPoint = new List<EndPoint>();
        static List<byte[]> userData = new List<byte[]>();
        static void sendProcessing(object sender, System.Timers.ElapsedEventArgs e)
        {
            string s = "";
            for (int j = 0; j < userPoint.Count; ++j)
            {
                s += Encoding.UTF8.GetString(userData[j], 0, recv);
            }
            for (int i = 0; i < userPoint.Count; ++i)
            {
                try
                {
                    server.SendTo(Encoding.UTF8.GetBytes(s), userPoint[i]);
                    Console.WriteLine("sendData: " + s);
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
        }
        static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.High;
            const int userNum = 2;
            recv = 0;
            byte[] data = new byte[bufferSize];

            sendTimer = new System.Timers.Timer();
            sendTimer.Interval = 15;
            sendTimer.Elapsed += sendProcessing;
            server.Bind(ep);

            Console.WriteLine("Waiting for a client...");
            int count = 0;
            //지금은 userNum만큼 무조건 받아야 게임시작이 된다.
            while (count++ < userNum)
            {
                data = new byte[bufferSize];
                recv = server.ReceiveFrom(data, ref remoteEP);
                if (recv <= 0)
                {
                    Console.WriteLine("error");
                    continue;
                }
                EndPoint tempEP = remoteEP;
                userPoint.Add(tempEP);
                userData.Add(data);
                Console.WriteLine("connect {0}", remoteEP.ToString());
            }
            //유저에게 번호를 부여하여 전송
            for (int next = 0; next < userPoint.Count; ++next)
            {
                data = Encoding.UTF8.GetBytes(Convert.ToString(next));
                server.SendTo(data, userPoint[next]);
            }
            //데이터 받기 시작
            sendTimer.Enabled = true;
            while (true)
            {
                int T = userPoint.Count;
                ////너무 빠르게 받아서 같은 값만 받을 경우를 우려하여 while문으로 더 받음
                for(int i=0; i<T; ++i) { 
                    data = new byte[bufferSize];
                    try
                    {
                        recv = server.ReceiveFrom(data, ref remoteEP);
                    }
                    catch (Exception ex) { /*Environment.Exit(0);*/ }
                    //만약 클라이언트가 꺼졋다면 리스트에 있는지 확인후 삭제
                    string end = Encoding.UTF8.GetString(data, 0, recv);
                    //클라이언트측에서 폼을 꺼버렸을 경우
                    if (end == "form_closed")
                    {
                        for (int next = 0; next < userPoint.Count; ++next)
                        {
                            if (remoteEP.Equals(userPoint[next]))
                            {
                                userPoint.Remove(userPoint[next]);
                                userData.Remove(userData[next]);
                            }
                        }
                        Console.WriteLine("user disconnected!!");
                        continue;
                    }
                    for (int next = 0; next < userPoint.Count; ++next)
                    {
                        if (remoteEP.Equals(userPoint[next]))
                        {
                            userData[next] = data;
                        }
                    }
                }
                //Thread.Sleep(1);
            }

            server.Close();
        }
    }
}
