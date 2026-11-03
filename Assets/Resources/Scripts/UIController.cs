using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private RectTransform promptE;

    private Camera cameraPrincipal;
    private Interactable alvoAtual;

    private void Awake()
    {
        cameraPrincipal = Camera.main;
        promptE.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        player.OnStaminaChanged += AtualizarStamina;
        playerInteractor.OnAlvoMudou += AtualizarAlvo;
    }

    private void OnDisable()
    {
        player.OnStaminaChanged -= AtualizarStamina;
        playerInteractor.OnAlvoMudou -= AtualizarAlvo;
    }

    private void LateUpdate()
    {
        if (alvoAtual == null) return;

        Vector3 posicaoNaTela = cameraPrincipal.WorldToScreenPoint(alvoAtual.PosicaoDoPrompt);

        bool visivel = posicaoNaTela.z > 0f;
        promptE.gameObject.SetActive(visivel);

        if (visivel)
        {
            promptE.position = posicaoNaTela;
        }
    }

    private void AtualizarStamina(float atual, float maxima)
    {
        staminaBarFill.fillAmount = atual / maxima;
    }

    private void AtualizarAlvo(Interactable novoAlvo)
    {
        alvoAtual = novoAlvo;
        promptE.gameObject.SetActive(novoAlvo != null);
    }
}