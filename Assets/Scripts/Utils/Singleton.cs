using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{ 
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();

				if (_instance == null)
				{
					GameObject singletonObject = new GameObject(typeof(T).ToString());
					_instance = singletonObject.AddComponent<T>();

					// Crea un contenedor para el objeto singleton
					GameObject containerObject = new GameObject($"{typeof(T).ToString()}Container");
					_instance.transform.SetParent(containerObject.transform);

					// Buscar el objeto GameManager en la escena.
					GameObject parentObject = GameObject.Find("GameManager");
					if (parentObject != null)
					{
						containerObject.transform.SetParent(parentObject.transform);
					}
				}
			}
			return _instance;
		}
	}
}


