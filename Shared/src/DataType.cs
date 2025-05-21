namespace Shared;



public struct DataState
{
    DState TypeState;

    public string message { get; private set; }

    public DataState(DState TypeState)
    {
        this.TypeState = TypeState;
        if (TypeState == DState.waiting)
        {
            message = "STATE=WAITING";
        }
        else if (TypeState == DState.playing)
        {
            message = "STATE=PLAYING";
        }
        else
        {
            message = "";
        }
        
        
    }



}

public enum DState
{
    playing,
    waiting,
    
}


