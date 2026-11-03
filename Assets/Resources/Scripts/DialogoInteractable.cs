using UnityEngine;

public class DialogoInteractable : Interactable
{
    [SerializeField] private TextAsset arquivoDialogo;

    private DialogueManager manager;

    private void Awake()
    {
        manager = FindFirstObjectByType<DialogueManager>();
    }

    public override void Interagir()
    {
        if (arquivoDialogo == null)
        {
            Debug.LogWarning("Diálogo sem arquivo JSON: " + gameObject.name);
            return;
        }

        if (manager == null)
        {
            Debug.LogWarning("Nenhum DialogueManager na cena.");
            return;
        }

        manager.Iniciar(arquivoDialogo, Interactor);
    }
}