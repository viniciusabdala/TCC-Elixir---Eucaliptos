using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private Transform pontoDoPrompt;

    private PlayerInteractor interactorAtual;

    public Vector3 PosicaoDoPrompt =>
        pontoDoPrompt != null ? pontoDoPrompt.position : transform.position;

    protected PlayerInteractor Interactor => interactorAtual;

    public abstract void Interagir();

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerInteractor interactor))
        {
            interactorAtual = interactor;
            interactor.Registrar(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerInteractor interactor))
        {
            interactor.Remover(this);
            interactorAtual = null;
        }
    }

    protected virtual void OnDisable()
    {
        if (interactorAtual != null)
        {
            interactorAtual.Remover(this);
            interactorAtual = null;
        }
    }
}