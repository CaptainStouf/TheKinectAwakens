﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;

public class ListenClient : MonoBehaviour
{

    Thread t;
    Client c;
    public static string ori;
    public static string pos;


    // Use this for initialization
    void Start()
    {
        c = new Client();

        t = new Thread(new ThreadStart(c.run));
        t.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnApplicationQuit()
    {
        c.stop();
        t.Join();
        Debug.Log("Stopped Thread");
    }

    public class Client
    {
        bool running;
        public void run()
        {
			Debug.Log ("Starting Thread");
            running = true;
            try
            {
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse("10.19.186.134");
                System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, 1234);
                soc.ReceiveTimeout = 1000;
                soc.SendTimeout = 1000;
                soc.Connect(remoteEP);
                soc.Send(System.Text.Encoding.ASCII.GetBytes("Output\n"));
                while (running)
                {
                    byte[] buffer = new byte[300000];
                    int iRx = soc.Receive(buffer);
                    char[] chars = new char[iRx];

                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                    System.String recv = new System.String(chars);
                    //Console.WriteLine(recv);
                    //Debug.Log(recv);
                    //msg = recv;
                    string[] msgs = recv.Split('@');
                    ori = msgs[0];
					pos = msgs[1];
                    /*
                    if (msgs[0].Split(' ').Length == 4)
                    {
                        ori = msgs[0];
                        pos = msgs[1];
                    }
                    else {
                        pos = msgs[0];
                        ori = msgs[1];
                    }*/
                }
                soc.Close();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                Debug.Log(e);
            }
        }
        public void stop()
        {
            running = false;
        }
    }
}
