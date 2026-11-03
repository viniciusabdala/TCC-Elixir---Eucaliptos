using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EscolhaData
{
    public string texto;
    public string proximo;
}

[System.Serializable]
public class FalaData
{
    public string id;
    public string personagem;
    public string sprite;
    public string texto;
    public string proximo;
    public List<EscolhaData> escolhas;

    public bool TemEscolhas => escolhas != null && escolhas.Count > 0;
}

[System.Serializable]
public class DialogoData
{
    public string inicio;
    public List<FalaData> falas;

    private Dictionary<string, FalaData> indice;

    public static DialogoData Carregar(TextAsset arquivo)
    {
        DialogoData dialogo;

        try
        {
            dialogo = JsonUtility.FromJson<DialogoData>(arquivo.text);
        }
        catch (System.ArgumentException erro)
        {
            Debug.LogError($"[{arquivo.name}] JSON inválido: {erro.Message}");
            return null;
        }

        dialogo.Preparar(arquivo.name);
        return dialogo;
    }

    public FalaData BuscarFala(string id)
    {
        if (indice != null && indice.TryGetValue(id, out FalaData fala))
        {
            return fala;
        }

        Debug.LogError($"Fala '{id}' não encontrada.");
        return null;
    }

    private void Preparar(string nomeArquivo)
    {
        indice = new Dictionary<string, FalaData>();

        if (falas == null || falas.Count == 0)
        {
            Debug.LogError($"[{nomeArquivo}] O diálogo não tem nenhuma fala.");
            return;
        }

        foreach (FalaData fala in falas)
        {
            if (string.IsNullOrEmpty(fala.id))
            {
                Debug.LogError($"[{nomeArquivo}] Existe uma fala sem id.");
            }
            else if (!indice.TryAdd(fala.id, fala))
            {
                Debug.LogError($"[{nomeArquivo}] ID repetido: '{fala.id}'.");
            }
        }

        if (string.IsNullOrEmpty(inicio) || !indice.ContainsKey(inicio))
        {
            Debug.LogError($"[{nomeArquivo}] O 'inicio' aponta para uma fala que não existe: '{inicio}'.");
        }

        foreach (FalaData fala in falas)
        {
            if (!string.IsNullOrEmpty(fala.proximo) && !indice.ContainsKey(fala.proximo))
            {
                Debug.LogError($"[{nomeArquivo}] A fala '{fala.id}' aponta para '{fala.proximo}', que não existe.");
            }

            if (fala.TemEscolhas)
            {
                foreach (EscolhaData escolha in fala.escolhas)
                {
                    if (!string.IsNullOrEmpty(escolha.proximo) && !indice.ContainsKey(escolha.proximo))
                    {
                        Debug.LogError($"[{nomeArquivo}] Uma escolha da fala '{fala.id}' aponta para '{escolha.proximo}', que não existe.");
                    }
                }
            }

            if (!string.IsNullOrEmpty(fala.sprite) && Resources.Load<Sprite>("Talksprites/" + fala.sprite) == null)
            {
                Debug.LogError($"[{nomeArquivo}] A fala '{fala.id}' usa o sprite '{fala.sprite}', que não existe em Resources/Talksprites.");
            }
        }
    }
}