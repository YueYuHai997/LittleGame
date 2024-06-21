using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GameEvent;


public class EventDemo : MonoBehaviour
{

    #region Demo1

    // 定义一个叫SomeEvt的事件
    public struct SomeEvt : IGameEvent
    {
    }

    //订阅事件
    [GameEvent]
    private void OnSomeEvt(SomeEvt evt)
    {
        Debug.Log("SomeEvt happened");
    }
    
    private void SomeMethod()
    {
        // 发起 SomeEvt 事件
        var evt = new SomeEvt();
        evt.Invoke();
    }


    #endregion

    #region Demo2

    public struct SyncEvt : GameEvent.IGameEvent
    {
        // 事件信息可自定义（可以没有）
        // 事件参数1
        public int param1;
        public string param2;
    }

    [GameEvent]
    private void OnSyncEvt(SyncEvt evt)
    {
        Debug.Log($"OnSyncEvt{evt.param1} And {evt.param2}");
    }

    [GameEvent]
    private static void OnSyncEvt_Static(SyncEvt evt)
    {
        Debug.Log($"OnSyncEvt_Static{evt.param1} And {evt.param2}");
    }
        
    #endregion

    
    #region Demo3

    public struct AsyncEvt : GameEvent.IGameTask
    {
        // 事件信息部分同上
    }

    [GameEvent]
    private async Task OnAsyncEvt2(AsyncEvt evt)
    {
        await Task.Delay(2000);
        Debug.Log("OnAsyncEvt 2 s");
    }
    
    [GameEvent]
    private async Task OnAsyncEvt1(AsyncEvt evt)
    {
        await Task.Delay(1000);
        Debug.Log("OnAsyncEvt 1 s");
    }
    
    
    #endregion


    #region Demo4
    
    //泛型事件

    public struct GenericEvt<T> : IGameEvent
    {
        public T Value;
    }
    
    // 监听确定类型的泛型事件
    [GameEvent]
    private void OnStringGenericEvt(GenericEvt<int> evt)
    {
        Debug.Log($"OnStringGenericEvt Message {evt.Value}");
    }
    
    // 监听确定类型的泛型事件
    [GameEvent]
    private void OnStringGenericEvt(GenericEvt<string> evt)
    {
        Debug.Log($"OnStringGenericEvt Message {evt.Value}");
    }

    #endregion

    
    async void  Start()
    {
        // 初始化API
        GameEvent.GameEventDriver.Initialize("Assembly-CSharp", true);
        
        //SomeMethod();

        // 同步事件
        // var evt = new SyncEvt();
        // evt.param1 = 101;
        // evt.param2 = "Evt-1";
        // evt.Invoke();
        //
        // var evt2 = new SyncEvt();
        // evt2.param1 = 202;
        // evt2.param2 = "Evt-2";
        // evt2.Invoke();
        
        //异步事件
        // var evt = new AsyncEvt();
        // await evt.InvokeTask();
        // Debug.Log("Finish!!");
        
        //泛型事件
        
        // 调起<int>事件
        var intEvt = new GenericEvt<int>() { Value = 1 };
        intEvt.Invoke();
        
        // 调起<string>事件
        var stringEvt = new GenericEvt<string>() { Value = "Hello" };
        stringEvt.Invoke();



    }
}