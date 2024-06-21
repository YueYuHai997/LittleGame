using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Task = System.Threading.Tasks.Task;


/// <summary>
/// 倒计时
/// </summary>
public class CountDown : MonoBehaviour
{

    //倒计时时长
    public float CountDownTime;
    
    
    

    private Animation[] Anis;
    
    private async void Awake()
    {
        Anis = this.GetComponentsInChildren<Animation>();
    }

    private CancellationTokenSource _cts;
    public async void Play(Transform transform)
    {
        this.transform.position = transform.position;
        
        _cts?.Cancel();
        
        foreach (var VARIABLE in Anis)
        {
            VARIABLE.Play();
        }
        
        CountDownFun();
    }

    private async Task CountDownFun()
    {
        _cts = new CancellationTokenSource();
        
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(CountDownTime), _cts.Token);
            Debug.Log("倒计时结束");
        }
        catch (TaskCanceledException e)
        {
            Debug.Log("倒计时中止");
        }
    }
    
    public void Stop()
    {
        _cts?.Cancel();

        foreach (var VARIABLE in Anis)
        {
            VARIABLE.Stop();
        } 
    }
    
}
