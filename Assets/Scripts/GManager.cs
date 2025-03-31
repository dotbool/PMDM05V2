using UnityEngine;

public class GManager : MonoBehaviour
{

    public static GManager Instance;
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


//When you use Action, you are not passing the Sender object to the event
//handler. Sometimes it is useful for the event handler to know what object
//triggered the event.