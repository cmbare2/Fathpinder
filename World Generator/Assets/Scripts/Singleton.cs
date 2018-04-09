using UnityEngine;
using System.Collections;


public class Singleton<T> : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected void Awake()
    {
        if (Instance != null)
        {
            Debug.LogErrorFormat("Second instance of {0} instantiated and replaced {1}", typeof(T).ToString(), (Instance as MonoBehaviour).name);
        }
        Instance = GetComponent<T>();
    }
}
