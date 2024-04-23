using UnityEngine;

public static class ServiceLocator
{
    private static EventManager _eventManager;

    public static EventManager EventManager
    {
        get
        {
            if (_eventManager == null)
            {
                // You may need to adjust this based on how you instantiate your EventManager
                //_eventManager = Object.FindObjectOfType<EventManager>();
                _eventManager = EventManager.Instance;
            }
            return _eventManager;
        }
    }
}