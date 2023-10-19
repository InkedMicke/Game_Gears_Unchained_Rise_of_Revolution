using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _WeAreAthomic.SCRIPTS.Props;

public class HijoDeInteractuableButton : ButtonInteractable
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Debug.Log("He comenzado en el hijo");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
