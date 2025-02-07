//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在RRQMCore.XREF命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://www.yuque.com/eo2w71/rrqm
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
using RRQMCore.Dependency;
using RRQMCore.Log;
using System;

namespace RRQMSocket
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// 插件执行顺序
        /// <para>该属性值越大，越靠前执行。值相等时，按添加先后顺序</para>
        /// <para>该属性效果，仅在<see cref="IPluginsManager.Add(IPlugin)"/>之前设置有效。</para>
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 日志记录器。
        /// <para>在<see cref="IPluginsManager.Add(IPlugin)"/>之前如果没有赋值的话，随后会用<see cref="IContainer.Resolve{T}"/>填值</para>
        /// </summary>
        public ILog Logger { get; set; }

        /// <summary>
        /// 包含此插件的插件管理器
        /// </summary>
        IPluginsManager PluginsManager { get; set; }
    }
}
