using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance != null)
                return instance;

            // try find existing
            instance = FindObjectOfType<T>();

            // try create from prefab in ressources
            if (instance == null)
            {
                GameObject singletonPrefab = Resources.Load<GameObject>(typeof(T).Name);
                if (singletonPrefab)
                    instance = Instantiate(singletonPrefab).GetComponent<T>();

                if(instance == null)
                    Destroy(singletonPrefab);
            }

            // try and add a new gameobject with the component
            if (instance == null)
                instance = new GameObject(typeof(T).Name).AddComponent<T>();


            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
            Debug.Log("<color=yellow>"+instance.name+" Created</color>" );
        }
        else
        {
            if(this != instance)
                Destroy(instance);
        }
    }

    private void OnDestroy() => instance = instance == this ? null : instance;
}