using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogoUI ui;
    [SerializeField] private string nomeProtagonista = "Carlos Dias";

    private InputAction interactAction;
    private InputAction moveAction;

    private DialogoData dialogoAtual;
    private FalaData falaAtual;
    private PlayerInteractor interactor;
    private PlayerController player;

    private int escolhaSelecionada;
    private float ultimoEixoY;
    private int frameDeInicio;

    public bool EmDialogo { get; private set; }

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        moveAction = InputSystem.actions.FindAction("Move");
    }

    public void Iniciar(TextAsset arquivo, PlayerInteractor quemInteragiu)
    {
        if (EmDialogo) return;

        DialogoData dialogo = DialogoData.Carregar(arquivo);
        if (dialogo == null) return;

        FalaData primeiraFala = dialogo.BuscarFala(dialogo.inicio);
        if (primeiraFala == null) return;

        dialogoAtual = dialogo;
        interactor = quemInteragiu;
        player = interactor.GetComponent<PlayerController>();

        EmDialogo = true;
        frameDeInicio = Time.frameCount;
        interactor.DefinirOcupado(true);
        player.podeMover = false;

        ui.Abrir();
        MostrarFala(primeiraFala);
    }

    private void Update()
    {
        if (!EmDialogo || Time.frameCount == frameDeInicio) return;

        if (falaAtual.TemEscolhas)
        {
            ControlarEscolhas();
        }
        else if (interactAction.WasPressedThisFrame())
        {
            Avancar(falaAtual.proximo);
        }
    }

    private void MostrarFala(FalaData fala)
    {
        falaAtual = fala;

        bool ladoEsquerdo = fala.personagem == nomeProtagonista;
        Sprite sprite = string.IsNullOrEmpty(fala.sprite)
            ? null
            : Resources.Load<Sprite>("Talksprites/" + fala.sprite);

        ui.MostrarFala(fala.personagem, fala.texto, ladoEsquerdo, sprite);

        if (fala.TemEscolhas)
        {
            escolhaSelecionada = 0;
            ultimoEixoY = moveAction.ReadValue<Vector2>().y;
            ui.MostrarEscolhas(fala.escolhas, escolhaSelecionada);
        }
        else
        {
            ui.EsconderEscolhas();
        }
    }

    private void ControlarEscolhas()
    {
        float y = moveAction.ReadValue<Vector2>().y;

        if (y > 0.5f && ultimoEixoY <= 0.5f)
        {
            escolhaSelecionada--;
        }
        else if (y < -0.5f && ultimoEixoY >= -0.5f)
        {
            escolhaSelecionada++;
        }

        ultimoEixoY = y;

        int total = Mathf.Min(falaAtual.escolhas.Count, ui.MaximoDeEscolhas);
        escolhaSelecionada = Mathf.Clamp(escolhaSelecionada, 0, total - 1);
        ui.DestacarEscolha(escolhaSelecionada);

        if (interactAction.WasPressedThisFrame())
        {
            Avancar(falaAtual.escolhas[escolhaSelecionada].proximo);
        }
    }

    private void Avancar(string proximoId)
    {
        if (string.IsNullOrEmpty(proximoId))
        {
            Encerrar();
            return;
        }

        FalaData proxima = dialogoAtual.BuscarFala(proximoId);

        if (proxima == null)
        {
            Encerrar();
            return;
        }

        MostrarFala(proxima);
    }

    private void Encerrar()
    {
        EmDialogo = false;
        ui.Fechar();
        StartCoroutine(LiberarJogador());
    }

    private IEnumerator LiberarJogador()
    {
        yield return null;

        player.podeMover = true;
        interactor.DefinirOcupado(false);
    }
}