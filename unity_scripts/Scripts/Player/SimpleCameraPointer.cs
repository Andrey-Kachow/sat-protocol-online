using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleCameraPointer : MonoBehaviour
{
    public LayerMask CollisionMask;

    //private const float _maxDistance = 10;
    //private GameObject _gazedAtObject = null;

    //public void Update()
    //{
        //// Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        //// at.
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance, CollisionMask))
        //{
        //    // GameObject detected in front of the camera.
        //    if (_gazedAtObject != hit.transform.gameObject)
        //    {
        //        // New GameObject.
        //        if (_gazedAtObject != null)
        //        {
        //            _gazedAtObject.SendMessage("OnPointerExit");
        //        }
        //        _gazedAtObject = hit.transform.gameObject;
        //        _gazedAtObject.SendMessage("OnPointerEnter");
        //    }
        //}
        //else
        //{
        //    if (gameObject != null)
        //    {
        //        _gazedAtObject.SendMessage("OnPointerExit");
        //        _gazedAtObject = null;
        //    }
        //}
        

        
        //// WebGL check for input to activate button
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    _gazedAtObject?.SendMessage("OnPointerClick");
        //}
    //}
}
