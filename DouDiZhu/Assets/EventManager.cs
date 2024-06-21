using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 事件管理中心
/// </summary>
public class EventManager
{
    /// <summary>
    /// 事件列表
    /// </summary>
    private Dictionary<GameEvent, List<Delegate>> _eventHandlers = new();

    /// <summary>
    /// 泛型注册事件
    /// </summary>
    /// <param name="gameEvent"></param>
    /// <param name="handler"></param>
    /// <typeparam name="T"></typeparam>
    public void RegisterEvent<T>(GameEvent gameEvent, Action<T> handler)
    {
        if (!_eventHandlers.TryGetValue(gameEvent, out var handlers)) //如果没找到
        {
            handlers = new List<Delegate>();
            _eventHandlers[gameEvent] = handlers;
        }

        handlers.Add(handler);
    }

    /// <summary>
    /// 无参数注册事件
    /// </summary>
    /// <param name="gameEvent"></param>
    /// <param name="handler"></param>
    public void RegisterEvent(GameEvent gameEvent, Action handler)
    {
        if (!_eventHandlers.TryGetValue(gameEvent, out var handlers))
        {
            handlers = new List<Delegate>();
            _eventHandlers[gameEvent] = handlers;
        }

        handlers.Add(handler);
    }

    /// <summary>
    /// 泛型取消事件
    /// </summary>
    /// <param name="gameEvent"></param>
    /// <param name="handler"></param>
    /// <typeparam name="T"></typeparam>
    public void CancelEvent<T>(GameEvent gameEvent, UnityAction<T> handler)
    {
        if (_eventHandlers.TryGetValue(gameEvent, out var handlers))
        {
            handlers.Remove(handler);
        }
        else
        {
            Debug.LogError($"Remove 失败未找到相对应事件！");
        }
    }

    /// <summary>
    /// 无参数取消事件
    /// </summary>
    /// <param name="gameEvent"></param>
    /// <param name="handler"></param>
    public void CancelEvent(GameEvent gameEvent, UnityAction handler)
    {
        if (_eventHandlers.TryGetValue(gameEvent, out var handlers))
        {
            handlers.Remove(handler);
        }
        else
        {
            Debug.LogError($"Remove 失败未找到相对应事件！");
        }
    }

    /// <summary>
    /// 泛型执行参数
    /// </summary>
    /// <param name="gameEvent"></param>
    /// <param name="arg"></param>
    /// <typeparam name="T"></typeparam>
    public void Execute<T>(GameEvent gameEvent, T arg)
    {
        if (_eventHandlers.TryGetValue(gameEvent, out var handlers))
        {
            foreach (var handler in handlers)
            {
                if (handler is Action<T> typedHandler)
                {
                    typedHandler(arg);
                }
            }
        }
        else
        {
            Debug.LogError($"{gameEvent}未绑定 无法调用");
        }
    }

    /// <summary>
    /// 无参数执行参数
    /// </summary>
    /// <param name="gameEvent"></param>
    public void Execute(GameEvent gameEvent)
    {
        if (_eventHandlers.TryGetValue(gameEvent, out var handlers))
        {
            foreach (var handler in handlers)
            {
                if (handler is Action typedHandler)
                {
                    typedHandler();
                }
            }
        }
        else
        {
            Debug.LogError($"{gameEvent}未绑定 无法调用");
        }
    }
}