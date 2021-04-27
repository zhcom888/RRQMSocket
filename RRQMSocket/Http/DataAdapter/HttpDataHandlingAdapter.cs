﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRQMCore.ByteManager;
using RRQMCore.Log;
using RRQMSocket;

namespace RRQMSocket.Http
{
    public class HttpDataHandlingAdapter : DataHandlingAdapter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxSize"></param>
        public HttpDataHandlingAdapter(int maxSize)
        {
            this.MaxSize = maxSize;
            this.terminatorCode = Encoding.UTF8.GetBytes("\r\n\r\n");
        }

        private byte[] terminatorCode;

        /// <summary>
        /// 允许的最大长度
        /// </summary>
        public int MaxSize { get; private set; }

        private ByteBlock tempByteBlock;

        private HttpRequest httpRequest;
        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="byteBlock"></param>
        protected override void PreviewReceived(ByteBlock byteBlock)
        {
            string s = Encoding.UTF8.GetString(byteBlock.Buffer,0,(int)byteBlock.Length);
            byte[] buffer = byteBlock.Buffer;
            int r = (int)byteBlock.Position;
            if (this.tempByteBlock != null)
            {
                this.tempByteBlock.Write(buffer, 0, r);
                buffer = this.tempByteBlock.Buffer;
                r = (int)this.tempByteBlock.Position;
            }


            if (this.httpRequest == null)
            {
                int index = buffer.IndexOfFirst(0, r, this.terminatorCode);
                if (index > 0)
                {
                    this.httpRequest = new HttpRequest();
                    this.httpRequest.ReadHeaders(buffer, 0, r);

                    if (this.httpRequest.Content_Length > 0)
                    {
                        this.httpRequest.Body = this.BytePool.GetByteBlock(this.httpRequest.Content_Length);
                        this.httpRequest.Body.Write(buffer, index +1, r - (index + 1));
                        if (this.httpRequest.Body.Length == this.httpRequest.Content_Length)
                        {
                            this.PreviewHandle(this.httpRequest);
                        }
                    }
                    else
                    {
                        this.PreviewHandle(this.httpRequest);
                    }

                }
                else if (r > this.MaxSize)
                {
                    if (this.tempByteBlock != null)
                    {
                        this.tempByteBlock.Dispose();
                        this.tempByteBlock = null;
                    }

                    Logger.Debug(LogType.Error, this, "在已接收数据大于设定值的情况下未找到终止符号，已放弃接收");
                    return;
                }
                else if (this.tempByteBlock == null)
                {
                    this.tempByteBlock = this.BytePool.GetByteBlock(r * 2);
                    this.tempByteBlock.Write(buffer, 0, r);
                }

            }
            else
            {
                if (r>=this.httpRequest.Content_Length-this.httpRequest.Body.Length)
                {
                    this.httpRequest.Body.Write(buffer, 0, this.httpRequest.Content_Length - (int)this.httpRequest.Body.Length);
                    this.PreviewHandle(this.httpRequest);
                }
                
            }
        }

        private void PreviewHandle(HttpRequest  httpRequest)
        {
            this.httpRequest = null;
            try
            {
                httpRequest.Build();
                this.GoReceived(null, httpRequest);
            }
            finally
            {
                httpRequest.Dispose();
            }
        }

        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        protected override void PreviewSend(byte[] buffer, int offset, int length)
        {
            this.GoSend(buffer, offset, length);
        }
    }
}