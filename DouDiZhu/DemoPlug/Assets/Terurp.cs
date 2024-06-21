using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;


public class Terurp : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        var p = new Person("John", "Quincy", "Adams", "Boston", "MA", 101);
        // Deconstruct the person object.
        // var (fName, lName, city, state) = p;
        // Debug.Log($"Hello {fName} {lName} of {city}, {state}!");
        var (fName, Age) = p;
        Debug.Log($"{fName} Age is {Age} !");
        
        // Type dateType = typeof(DateTime);
        // PropertyInfo prop = dateType.GetProperty("Now");
        // var (isStatic, isRO, isIndexed, propType) = prop;
        // Debug.Log($"   The {dateType.FullName}.{prop.Name} property:");
        // Debug.Log($"   PropertyType: {propType.Name}");
        // Debug.Log($"   Static:       {isStatic}");
        // Debug.Log($"   Read-only:    {isRO}");
        // Debug.Log($"   Indexed:      {isIndexed}");
        //
        // Type listType = typeof(List<>);
        // prop = listType.GetProperty("Item",
        //     BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        // var (hasGetAndSet, sameAccess, accessibility, getAccessibility, setAccessibility) = prop;
        // Debug.Log($" Accessibility of the {listType.FullName}.{prop.Name} property: ");
        //
        // if (!hasGetAndSet | sameAccess)
        // {
        //     Debug.Log(accessibility);
        // }
        // else
        // {
        //     Debug.Log($"   The get accessor: {getAccessibility}");
        //     Debug.Log($"   The set accessor: {setAccessibility}");
        // }
        
        //
        // Dictionary<string, int> snapshotCommitMap = new(StringComparer.OrdinalIgnoreCase)
        // {
        //     ["https://github.com/dotnet/docs"] = 16465,
        //     ["https://github.com/dotnet/runtime"] = 114223,
        //     ["https://github.com/dotnet/installer"] = 22436,
        //     ["https://github.com/dotnet/roslyn"] = 79484,
        //     ["https://github.com/dotnet/aspnetcore"] = 48386
        // };
        //
        // foreach (var (repo, commitCount) in snapshotCommitMap)
        // {
        //     Debug.Log(
        //         $"The {repo} repository had {commitCount:} commits as of November 10th, 2021.");
        // }
        //
        // foreach (var item in snapshotCommitMap)
        // {
        //     Debug.Log(
        //         $"The {item.Key} repository had {item.Value:N0} commits as of November 10th, 2021.");
        // }
        
        //
        // string a = null;
        // try
        // {
        //     Method(a);
        // }
        // catch (Exception e)
        // {
        //     Debug.Log(e);
        // }
        //

       await ExecuteAsyncMethods();
    }
    
    public static void Method(string arg)
    {
        _ = arg ?? throw new ArgumentNullException(paramName: nameof(arg), message: "arg can't be null");

        // Do work with arg.
    }

    private static async Task ExecuteAsyncMethods()
    {
        Debug.Log("About to launch a task...");
         Task.Run(() =>
        {
            var iterations = 0;
            for (int ctr = 0; ctr < int.MaxValue; ctr++)
                iterations++;
            Debug.Log("Completed looping operation...");
            throw new InvalidOperationException();
        });
        await Task.Delay(5000);
        Debug.Log("Exiting after 5 second delay");
    }

    // The example displays output like the following:
    //       About to launch a task...
    //       Completed looping operation...
    //       Exiting after 5 second delay
    
}


public class Person
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string City { get; set; }
    public string State { get; set; }

    public int _age;

    public Person(string fname, string mname, string lname,
                  string cityName, string stateName,int age)
    {
        FirstName = fname;
        MiddleName = mname;
        LastName = lname;
        City = cityName;
        State = stateName;
        _age = age;
    }

    // Return the first and last name. 
    // public void Deconstruct(out string fname, out string lname)
    // {
    //     fname = FirstName;
    //     lname = LastName;
    // }

    public void Deconstruct(out string fname, out string mname, out string lname)
    {
        fname = FirstName;
        mname = MiddleName;
        lname = LastName;
    }

    public void Deconstruct(out string fname, out string lname,
                            out string city, out string state)
    {
        fname = FirstName;
        lname = LastName;
        city = City;
        state = State;
    }

    // public void Deconstruct(out int Age, out string fname)
    // {
    //     fname = FirstName;
    //     Age = _age;
    // }
    
}


public static class ReflectionExtensions
{

    public static void Deconstruct(this Person p, out string FirstName, out int age)
    {
        FirstName = p.FirstName;
        age = p._age;
    }

    public static void Deconstruct(this PropertyInfo p, out bool isStatic,
                                   out bool isReadOnly, out bool isIndexed,
                                   out Type propertyType)
    {
        var getter = p.GetMethod;

        // Is the property read-only?
        isReadOnly = ! p.CanWrite;

        // Is the property instance or static?
        isStatic = getter.IsStatic;

        // Is the property indexed?   属性有索引嘛
        isIndexed = p.GetIndexParameters().Length > 0;

        // Get the property type.   获取属性类型
        propertyType = p.PropertyType;
    }

    public static void Deconstruct(this PropertyInfo p, out bool hasGetAndSet,
                                   out bool sameAccess, out string access,
                                   out string getAccess, out string setAccess)
    {
        hasGetAndSet = sameAccess = false;
        string getAccessTemp = null;
        string setAccessTemp = null;

        MethodInfo getter = null;
        if (p.CanRead)
            getter = p.GetMethod;

        MethodInfo setter = null;
        if (p.CanWrite)
            setter = p.SetMethod;

        if (setter != null && getter != null)
            hasGetAndSet = true;

        if (getter != null)
        {
            if (getter.IsPublic)
                getAccessTemp = "public";
            else if (getter.IsPrivate)
                getAccessTemp = "private";
            else if (getter.IsAssembly)
                getAccessTemp = "internal";
            else if (getter.IsFamily)
                getAccessTemp = "protected";
            else if (getter.IsFamilyOrAssembly)
                getAccessTemp = "protected internal";
        }

        if (setter != null)
        {
            if (setter.IsPublic)
                setAccessTemp = "public";
            else if (setter.IsPrivate)
                setAccessTemp = "private";
            else if (setter.IsAssembly)
                setAccessTemp = "internal";
            else if (setter.IsFamily)
                setAccessTemp = "protected";
            else if (setter.IsFamilyOrAssembly)
                setAccessTemp = "protected internal";
        }

        // Are the accessibility of the getter and setter the same?
        if (setAccessTemp == getAccessTemp)
        {
            sameAccess = true;
            access = getAccessTemp;
            getAccess = setAccess = String.Empty;
        }
        else
        {
            access = null;
            getAccess = getAccessTemp;
            setAccess = setAccessTemp;
        }
    }
}