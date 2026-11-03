using UnityEngine;

public class ExemploInteractable : Interactable
{
    public override void Interagir()
    {
        Debug.Log("Interagiu com: " + gameObject.name);
    }
}