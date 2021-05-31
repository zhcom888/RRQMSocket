﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRQMCore.ByteManager;
using RRQMSocket;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TokenService<MySocketClient> tokenService = new TokenService<MySocketClient>();

            var config = new ServerConfig();
            config.SetValue(ServerConfig.IPHostProperty, new IPHost(7789));
            config.SetValue(TcpServerConfig.MaxCountProperty, 10000);
            config.SetValue(TokenServerConfig.VerifyTokenProperty,"123TT");

            tokenService.Setup(config);
            tokenService.Start();
        }
    }
    class MySocketClient : SocketClient
    {
        protected override void HandleReceivedData(ByteBlock byteBlock, object obj)
        {

        }
    }
}