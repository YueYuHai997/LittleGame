using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Task = System.Threading.Tasks.Task;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class Defer : MonoBehaviour
{
    public Button button;
    
    async void Start()
    {
        // using var _ = new MyDisposable(() => Debug.Log("Hello Motor!"));
        // Debug.Log("DoSomething");
        // await Task.Delay(5000);
        
        button.onClick.AddListener(() =>
        {
            SearchMovieAsync("Hello!");
            Debug.Log("11111");
        });
    }

    private bool _isbusy;
    public bool IsBusy
    {
        get => _isbusy;
        set
        {
            _isbusy = value;
            button.interactable = !value;
        }
    }


    private MyDisposable NewMyDisposable => new MyDisposable(value => IsBusy = value);
    
    async Task SearchMovieAsync(string movieName)
    {
        using var _ = NewMyDisposable;

        if (!CanSearch())
        { 
            return;
        }

        var resList = await SearchMoviesFromInternetAsync(movieName);
        if (resList == null || resList.Count == 0)
        {
            return;
        }

        if (resList is not { Count: > 0 })
        {
            return;
        }

        foreach (var res in resList)
        {
            // Do something
        }
    }

    private bool CanSearch() => true;


    async Task<List<string>> SearchMoviesFromInternetAsync(string movieName)
    {
        await Task.Delay(5000);
        List<string> MoveName = new List<string>()
        {
            "The 1",
            "The 2",
            "The 3",
            "The 4",
        };
        return MoveName;
    }
}


public class MyDisposable : IDisposable
{
    private readonly Action<bool> _callBack;

    public MyDisposable(Action<bool> callBack)
    {
        _callBack = callBack;
        _callBack.Invoke(true);
    }

    public void Dispose() => _callBack.Invoke(false);
}