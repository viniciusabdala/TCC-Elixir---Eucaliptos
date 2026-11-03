using UnityEngine;

public class TeleporteInteractable : Interactable
{
    [SerializeField] private Transform destino;

    public override void Interagir()
    {
        if (destino == null)
        {
            Debug.LogWarning("Teleporte sem destino: " + gameObject.name);
            return;
        }

        Transform jogador = Interactor.transform;
        CharacterController controller = jogador.GetComponent<CharacterController>();

        controller.enabled = false;
        jogador.SetPositionAndRotation(destino.position, Quaternion.Euler(0f, destino.eulerAngles.y, 0f));
        controller.enabled = true;

        Interactor.Limpar();
    }
}