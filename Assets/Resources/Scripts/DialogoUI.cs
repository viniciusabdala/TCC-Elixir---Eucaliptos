using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogoUI : MonoBehaviour
{
    [SerializeField] private GameObject painel;
    [SerializeField] private Image spriteEsquerda;
    [SerializeField] private Image spriteDireita;
    [SerializeField] private TMP_Text nomePersonagem;
    [SerializeField] private TMP_Text textoFala;

    [SerializeField] private GameObject painelEscolhas;
    [SerializeField] private TMP_Text[] opcoes;

    [SerializeField] private Color corNormal = Color.white;
    [SerializeField] private Color corSelecionada = Color.yellow;

    public int MaximoDeEscolhas => opcoes.Length;

    public void Abrir()
    {
        painel.SetActive(true);
        spriteEsquerda.gameObject.SetActive(false);
        spriteDireita.gameObject.SetActive(false);
        painelEscolhas.SetActive(false);
    }

    public void Fechar()
    {
        painel.SetActive(false);
    }

    public void MostrarFala(string personagem, string texto, bool ladoEsquerdo, Sprite sprite)
    {
        nomePersonagem.text = personagem;
        textoFala.text = texto;

        Image slot = ladoEsquerdo ? spriteEsquerda : spriteDireita;
        slot.sprite = sprite;
        slot.gameObject.SetActive(sprite != null);
    }

    public void MostrarEscolhas(List<EscolhaData> escolhas, int selecionada)
    {
        if (escolhas.Count > opcoes.Length)
        {
            Debug.LogError($"A fala tem {escolhas.Count} escolhas, mas só existem {opcoes.Length} espaços na UI.");
        }

        for (int i = 0; i < opcoes.Length; i++)
        {
            bool ativa = i < escolhas.Count;
            opcoes[i].gameObject.SetActive(ativa);

            if (ativa)
            {
                opcoes[i].text = escolhas[i].texto;
            }
        }

        painelEscolhas.SetActive(true);
        DestacarEscolha(selecionada);
    }

    public void EsconderEscolhas()
    {
        painelEscolhas.SetActive(false);
    }

    public void DestacarEscolha(int selecionada)
    {
        for (int i = 0; i < opcoes.Length; i++)
        {
            opcoes[i].color = i == selecionada ? corSelecionada : corNormal;
        }
    }
}