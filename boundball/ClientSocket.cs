using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace boundball
{
    class ClientSocket
    {
        //통신 변수
        int recv, bufferSize = 128;
        byte[] data;
        string stringData, userNum;
        IPEndPoint serverEP;
        Socket client;
        IPEndPoint Sender;
        EndPoint remoteEP;
        //스레드 변수
        Thread receiveThread;
        FriendBall[] friend;
        Block[,] block;
        DoubleBufPanel panel;

        public ClientSocket(FriendBall[] tempFriend, ref Block[,] tempBlock, DoubleBufPanel _panel)
        {
            recv = 0;
            data = new byte[bufferSize];
            stringData = "";
            serverEP = new IPEndPoint(IPAddress.Parse("59.24.142.171"), 20000);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Sender = new IPEndPoint(IPAddress.Any, 0);
            remoteEP = (EndPoint)Sender;
            //스레드 초기화
            receiveThread = new Thread(receiveProcessing);
            friend = tempFriend;
            block = tempBlock;
            panel = _panel;
        }
        ~ClientSocket()
        {
            receiveThread.Abort();
        }
        Block b = new Block();
        public void receiveProcessing()
        {
            while (true)
            {
                try
                {
                    data = new byte[bufferSize];
                    recv = client.ReceiveFrom(data, ref remoteEP);
                    if (recv <= 0)
                    {
                        return;
                    }
                    string Data = Encoding.UTF8.GetString(data, 0, recv);
                    stringData = string.Format("{0},{1}", Data, remoteEP.ToString());
                    if (Data == "user disconnected!!") return;
                    while (Data.Length > 0)
                    {
                        int i = Convert.ToInt32(Data.Substring(0, 1));
                        int user = Convert.ToInt32(Data.Substring(1, 10));
                        int blockPoint = Convert.ToInt32(Data.Substring(11, 10));
                        if (i.ToString() == userNum)
                        {
                            Data = Data.Substring(21);
                            continue;
                        }
                        friend[i].x = user / 100000;
                        friend[i].y = user % 100000;
                        Point p = new Point(blockPoint / 100000, blockPoint % 100000);
                        if(block[p.Y, p.X].Shape == 2)
                        {
                            block[p.Y, p.X].Shape = 0;
                            b = block[p.Y, p.X];
                            block[p.Y, p.X].RegenerationBlock();
                        }
                        else if (block[p.Y, p.X].Shape == 10)
                        {
                            block[p.Y, p.X].Shape = 11;
                            b = block[p.Y, p.X];
                            block[p.Y, p.X].RegenerationBlock();
                        }
                        Data = Data.Substring(21);
                        panel.Refresh();
                    }
                }
                catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/ }
                Thread.Sleep(1);
            }
        }
        public void sendProcessing(Ball ball, Point tempBlock)
        {
            data = new byte[bufferSize];
            string s = string.Format("{0}{1:D5}{2:D5}{3:D5}{4:D5}", userNum, ball.real_x, ball.y, tempBlock.X, tempBlock.Y);
            data = Encoding.UTF8.GetBytes(s);
            client.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
        public void StartConnection()
        {
            //처음 연결 전송
            stringData = "FirstConnection";
            data = Encoding.UTF8.GetBytes(stringData);
            client.SendTo(data, data.Length, SocketFlags.None, serverEP);
            //유저 번호 받기
            recv = client.ReceiveFrom(data, ref remoteEP);
            userNum = Encoding.UTF8.GetString(data, 0, recv);
            receiveThread.Start();
        }
        public void AbortSocket()
        {
            receiveThread.Abort();
        }
    }
}
