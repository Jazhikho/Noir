using System;
using JFruit.Bolt;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AxisEvent : AotUnityEvent<float, float> {}

[Serializable]
public class NoIdeaAboutProperNameEvent : AotUnityEvent<string, float, GameObject, Transform> {}

public class HelloWorld : MonoBehaviour {
    public UnityEvent Simple;
    public AxisEvent Axis;
    public NoIdeaAboutProperNameEvent FullSupport;


    private void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Simple.Invoke();
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            
            Axis.Invoke(horizontal, vertical);
            
            FullSupport.Invoke(gameObject.name, Time.deltaTime, gameObject, transform);
        }
    }
}
