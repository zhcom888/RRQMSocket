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
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace RRQMCore.Reflection
{
    /// <summary>
    /// 表示方法
    /// </summary>
    public class Method
    {
        /// <summary>
        /// 方法执行委托
        /// </summary>
        private readonly Func<object, object[], object> m_invoker;

        private MethodInfo m_info;

        private bool m_isByRef;

        private bool m_wait;

        /// <summary>
        /// 方法
        /// </summary>
        /// <param name="method">方法信息</param>
        public Method(MethodInfo method)
        {
            this.m_info = method ?? throw new ArgumentNullException(nameof(method));
            this.Name = method.Name;

            foreach (var item in method.GetParameters())
            {
                if (item.ParameterType.IsByRef)
                {
                    this.m_isByRef = true;

                    if (method.ReturnType != typeof(void) && method.ReturnType != typeof(Task))
                    {
                        this.HasReturn = true;
                        if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                        {
                            this.m_wait = true;
                            this.ReturnType = method.ReturnType.GetGenericArguments()[0];
                        }
                        else
                        {
                            this.ReturnType = method.ReturnType;
                        }
                    }
                    return;
                }
            }
            this.m_invoker = this.CreateInvoker(method);
        }

        /// <summary>
        /// 是否具有返回值
        /// </summary>
        public bool HasReturn { get; private set; }

        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo Info => this.m_info;

        /// <summary>
        /// 是否有引用类型
        /// </summary>
        public bool IsByRef => this.m_isByRef;

        /// <summary>
        /// 获取方法名
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 返回值类型。
        /// <para>当方法为void或task时，为null</para>
        /// <para>当方法为task泛型时，为泛型元素类型</para>
        /// </summary>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// 应当等待
        /// </summary>
        public bool ShouldWait => this.m_wait;

        /// <summary>
        /// 执行方法。
        /// <para>当方法为void或task时，会返回null</para>
        /// <para>当方法为task泛型时，会wait后的值</para>
        /// <para>注意：当调用方为UI主线程时，调用异步方法，则极有可能发生死锁。</para>
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object Invoke(object instance, params object[] parameters)
        {
            object re;
            if (this.m_isByRef)
            {
                re = this.m_info.Invoke(instance, parameters);
            }
            else
            {
                re = this.m_invoker.Invoke(instance, parameters);
            }
            if (this.m_wait)
            {
                Task task = (Task)re;
                task.Wait();
                return task.GetType().GetProperty("Result").GetValue(task);
            }
            else
            {
                return re;
            }
        }

        /// <summary>
        /// 调用异步结果
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<object> InvokeAsync(object instance, params object[] parameters)
        {
            if (this.m_wait)
            {
                if (this.m_isByRef)
                {
                    Task task = (Task)this.m_info.Invoke(instance, parameters);
                    await task;
                    return task.GetType().GetProperty("Result").GetValue(task);
                }
                else
                {
                    Task task = (Task)this.m_invoker.Invoke(instance, parameters);
                    await task;
                    return task.GetType().GetProperty("Result").GetValue(task);
                }
            }
            else
            {
                return Task.FromResult(this.Invoke(instance, parameters));
            }

        }

        /// <summary>
        /// 生成方法的调用委托
        /// </summary>
        /// <param name="method">方法成员信息</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private Func<object, object[], object> CreateInvoker(MethodInfo method)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var parameters = Expression.Parameter(typeof(object[]), "parameters");

            var instanceCast = method.IsStatic ? null : Expression.Convert(instance, method.DeclaringType);
            var parametersCast = method.GetParameters().Select((p, i) =>
            {
                var parameter = Expression.ArrayIndex(parameters, Expression.Constant(i));
                return Expression.Convert(parameter, p.ParameterType);
            });

            var body = Expression.Call(instanceCast, method, parametersCast);

            if (method.ReturnType == typeof(void) || method.ReturnType == typeof(Task))
            {
                var action = Expression.Lambda<Action<object, object[]>>(body, instance, parameters).Compile();
                return (_instance, _parameters) =>
                {
                    action.Invoke(_instance, _parameters);
                    return null;
                };
            }
            else
            {
                this.HasReturn = true;
                if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    this.m_wait = true;
                    this.ReturnType = method.ReturnType.GetGenericArguments()[0];
                }
                else
                {
                    this.ReturnType = method.ReturnType;
                }
                var bodyCast = Expression.Convert(body, typeof(object));
                return Expression.Lambda<Func<object, object[], object>>(bodyCast, instance, parameters).Compile();
            }
        }
    }
}