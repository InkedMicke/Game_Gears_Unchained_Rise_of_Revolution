using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

namespace _WeAreAthomic.SCRIPTS.Props
{

    public class OpenDoorLab : MonoBehaviour
    {
        public bool openDoor;
        private Animator _anim;
        [SerializeField] private AudioSource openDoorSound;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        private void Update()
        {
            Test();
        }

        private void Test()
        {
            if (openDoor)
            {
                OpenDoor(0);
                openDoor = false;
            }
        }

        public void OpenDoor(float value)
        {
            Invoke(nameof(InvokeOpenDoor), value);
            PlaySound();
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

        private void PlaySound()
        {
            openDoorSound.Play();
        }

    }
}
