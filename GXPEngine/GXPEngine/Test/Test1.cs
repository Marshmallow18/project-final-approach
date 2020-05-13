using System;
using System.Collections;
using GXPEngine;

public class Test1 : GameObject
{
    public Test1()
    {
        CoroutineManager.StartCoroutine(PlayRepeat(), this);
    }

    private IEnumerator PlayRepeat()
    {
        while (true) //use some statement here to exit this while and stop this routine
        {
            //Do Stuff here

            Console.WriteLine($"{this}: running each second: {Time.now}");
            yield return new WaitForMilliSeconds(1000); //wait for 1 second
        }
    }
}