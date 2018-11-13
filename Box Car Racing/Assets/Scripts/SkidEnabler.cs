using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidEnabler : MonoBehaviour {


 //   public WheelCollider wheelCollider;
 //   public GameObject skidTrailRenderer;
 //   public float skidLife = 4f;
 //   private TrailRenderer skidMark;

	//// Use this for initialization
	//void Start () {
 //       skidMark = skidTrailRenderer.GetComponent<TrailRenderer>();
 //       //this avoids a visual bug on first use, ifthe artteam set the effects time to 0.
 //       skidMark.time = skidLife;
	//}
	
	//// Update is called once per frame
	//void Update () {


	//	if (wheelCollider.forwardFriction.stiffness < 0.1f && wheelCollider.isGrounded)
 //       {
 //           //if the skid marks time variable is set to 0 than we reset it previously and can now use it
 //           if(skidMark.time == 0)
 //           {
 //               skidMark.time = skidLife;
 //               skidTrailRenderer.transform.parent = wheelCollider.transform;
 //               skidTrailRenderer.transform.localPosition = wheelCollider.center + ((wheelCollider.radius-0.1f) + 
 //                   -wheelCollider.transform.up);
 //           }
 //       }
	//}
}
