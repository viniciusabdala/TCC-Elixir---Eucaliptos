using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    private readonly List<Interactable> interagiveisAoAlcance = new List<Interactable>();
    private InputAction interactAction;

    public Interactable AlvoAtual { get; private set; }
    public bool Ocupado { get; private set; }

    public event System.Action<Interactable> OnAlvoMudou;

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        AtualizarAlvo();

        if (AlvoAtual != null && interactAction.WasPressedThisFrame())
        {
            AlvoAtual.Interagir();
        }
    }

    public void Registrar(Interactable interagivel)
    {
        if (!interagiveisAoAlcance.Contains(interagivel))
        {
            interagiveisAoAlcance.Add(interagivel);
        }
    }

    public void Remover(Interactable interagivel)
    {
        interagiveisAoAlcance.Remove(interagivel);
    }

    public void Limpar()
    {
        interagiveisAoAlcance.Clear();
    }

    public void DefinirOcupado(bool valor)
    {
        Ocupado = valor;
    }

    private void AtualizarAlvo()
    {
        Interactable maisProximo = null;

        if (!Ocupado)
        {
            float menorDistancia = float.MaxValue;

            foreach (Interactable item in interagiveisAoAlcance)
            {
                float distancia = (item.transform.position - transform.position).sqrMagnitude;

                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    maisProximo = item;
                }
            }
        }

        if (maisProximo != AlvoAtual)
        {
            AlvoAtual = maisProximo;
            OnAlvoMudou?.Invoke(AlvoAtual);
        }
    }
}