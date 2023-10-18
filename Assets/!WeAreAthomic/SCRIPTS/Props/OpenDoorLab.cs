using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace _WeAreAthomic.SCRIPTS.Props
{

    public class OpenDoorLab : MonoBehaviour
    {
        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void OpenDoor(float value)
        {
            Invoke(nameof(InvokeOpenDoor), value);
        }
        public void CloseDoor(float value)
        {
            Invoke(nameof(InvokeCloseDoor), value);
        }

        private void InvokeOpenDoor()
        {
            _anim.SetTrigger(string.Format("openDoor"));
        }

        private void InvokeCloseDoor()
        {
            _anim.SetTrigger(string.Format("closeDoor"));
        }

    }
}
