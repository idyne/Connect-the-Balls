
using UnityEngine;

public class NoInternetPanel : MonoBehaviour
{
    [SerializeField] private Canvas canvas;


    public void Retry()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            canvas.enabled = false;
        }
    }

    public void CheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            canvas.enabled = true;
        }
    }
}
