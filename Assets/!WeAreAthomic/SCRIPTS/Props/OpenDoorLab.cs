using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorLab : MonoBehaviour
{
   private Animator _anim;

   private void Awake()
   {
      _anim = GetComponent<Animator>();
   }

   public void OpenDoor()
   {
      _anim.SetTrigger(string.Format("openDoor"));
   }
   public void CloseDoor()
   {
      _anim.SetTrigger(string.Format("closeDoor"));
   }
   
}
