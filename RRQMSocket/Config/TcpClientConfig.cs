﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRQMCore.ByteManager;
using RRQMCore.Dependency;

namespace RRQMSocket
{
    /// <summary>
    /// TcpClient配置
    /// </summary>
    public class TcpClientConfig : RRQMConfig
    {
        /// <summary>
        /// 数据处理适配器
        /// </summary>
        public DataHandlingAdapter DataHandlingAdapter
        {
            get { return (DataHandlingAdapter)GetValue(DataHandlingAdapterProperty); }
            set { SetValue(DataHandlingAdapterProperty, value); }
        }

        /// <summary>
        /// 数据处理适配器
        /// </summary>
        public static readonly DependencyProperty DataHandlingAdapterProperty =
            DependencyProperty.Register("DataHandlingAdapter", typeof(DataHandlingAdapter), typeof(TcpClientConfig), new NormalDataHandlingAdapter());


        /// <summary>
        /// 远程IPHost
        /// </summary>
        public IPHost RemoteIPHost
        {
            get { return (IPHost)GetValue(RemoteIPHostProperty); }
            set { SetValue(RemoteIPHostProperty, value); }
        }

        /// <summary>
        /// 远程IPHost
        /// </summary>
        public static readonly DependencyProperty RemoteIPHostProperty =
            DependencyProperty.Register("RemoteIPHost", typeof(IPHost), typeof(TcpClientConfig), null);


        /// <summary>
        /// 内存池实例
        /// </summary>
        public BytePool BytePool
        {
            get { return (BytePool)GetValue(BytePoolProperty); }
            set { SetValue(BytePoolProperty, value); }
        }

        /// <summary>
        /// 内存池实例
        /// </summary>
        public static readonly DependencyProperty BytePoolProperty =
            DependencyProperty.Register("BytePool", typeof(BytePool), typeof(TcpClientConfig), new BytePool());


        /// <summary>
        /// 仅发送，即不开启接收线程，
        /// 同时不会感知断开操作。
        /// </summary>
        public bool OnlySend
        {
            get { return (bool)GetValue(OnlySendProperty); }
            set { SetValue(OnlySendProperty, value); }
        }

        /// <summary>
        /// 仅发送，即不开启接收线程，
        /// 同时不会感知断开操作。
        /// </summary>
        public static readonly DependencyProperty OnlySendProperty =
            DependencyProperty.Register("OnlySend", typeof(bool), typeof(TcpClientConfig), false);

        /// <summary>
        /// 在异步发送时，使用独立线程发送
        /// </summary>
        public bool SeparateThreadSend
        {
            get { return (bool)GetValue(SeparateThreadSendProperty); }
            set { SetValue(SeparateThreadSendProperty, value); }
        }

        /// <summary>
        /// 在异步发送时，使用独立线程发送
        /// </summary>
        public static readonly DependencyProperty SeparateThreadSendProperty =
            DependencyProperty.Register("SeparateThreadSend", typeof(bool), typeof(TcpClientConfig), false);


        /// <summary>
        /// 独立线程发送缓存区
        /// </summary>
        public int SeparateThreadSendBufferLength
        {
            get { return (int)GetValue(SeparateThreadSendBufferLengthProperty); }
            set { SetValue(SeparateThreadSendBufferLengthProperty, value); }
        }

        /// <summary>
        /// 独立线程发送缓存区
        /// </summary>
        public static readonly DependencyProperty SeparateThreadSendBufferLengthProperty =
            DependencyProperty.Register("SeparateThreadSendBufferLength", typeof(int), typeof(TcpClientConfig), 1024);



    }
}