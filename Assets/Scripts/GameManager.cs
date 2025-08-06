using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    List<Corso> corsi;
    public Dictionary<Corso, double> medieCorsi;

    public TMP_Dropdown materiaDropdown;

    public InputField valutazioneInputField;
    public InputField pesoInputField;
    public InputField dataInputField;
    public TMP_Dropdown tipoEsameDropdown;

    public Button Aggiungi;
    public Transform valutazioniContentPanel;
    public Corso currentCorso;
    public GameObject votoUIPrefab;
    public GameObject inserimentoVotoPanel;
    
    //griglia modificabile nekll ispector
    private GridLayoutGroup gridLayoutGroup;
    
    public int colonne = 3;
    public Vector2 dimensioneCella = new Vector2(180, 100);
    public Vector2 spaziatura = new Vector2(10, 10);

    void Awake()
    {
        corsi = new List<Corso>();
        medieCorsi = new Dictionary<Corso, double>();
        
        gridLayoutGroup = valutazioniContentPanel.GetComponent<GridLayoutGroup>();
        if(gridLayoutGroup == null)
        {
            gridLayoutGroup = valutazioniContentPanel.gameObject.AddComponent<GridLayoutGroup>();
        }
        
        ConfiguraGriglia();
    }

    void Start()
    {
        if (Aggiungi != null)
        {
            Aggiungi.onClick.AddListener(aggiungiVotoButtonClick);
        }

        if (inserimentoVotoPanel != null)
        {
            inserimentoVotoPanel.SetActive(false);
        }
        InitializeTipoEsameDropdown();

        // Aggiungi tutte le materie preimpostate
        aggiungiCorso(new Corso("Matematica"));
        aggiungiCorso(new Corso("Italiano"));
        aggiungiCorso(new Corso("Inglese"));
        aggiungiCorso(new Corso("Storia"));
        aggiungiCorso(new Corso("Sistemi e Reti"));
        aggiungiCorso(new Corso("Informatica"));

        currentCorso = corsi[0];
        Debug.Log($"Corsi preimpostati aggiunti. Corso corrente: {currentCorso.materia}");
    
        InitializeMateriaDropdown();
}

    //metodo per configurare la griglia
    public void ConfiguraGriglia()
    {
        if(gridLayoutGroup != null)
        {
            gridLayoutGroup.cellSize = dimensioneCella;
            gridLayoutGroup.spacing = spaziatura;
            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = colonne;
        }
    }

    //cambia il numero di celle a run rime
    public void CambiaNumeroColonne(int nuoveColonne)
    {
        colonne = nuoveColonne;
        ConfiguraGriglia();
        AggiornaLayout();
    }

    //cambiare dimensione celle a runtime
    public void CambiaDimensioneCelle(float larghezza, float altezza)
    {
        dimensioneCella = new Vector2(larghezza, altezza);
        ConfiguraGriglia();
        AggiornaLayout();
    }

    private void AggiornaLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            valutazioniContentPanel.GetComponent<RectTransform>()
        );
    }

    public void aggiungiCorso(Corso corso)
    {
        corsi.Add(corso);
    }

    public void aggiungiVoto(Corso corso, Voto voto)
    {
        if (corsi.Contains(corso))
        {
            corso.AggiungiVoto(voto);
        }
        else
        {
            Debug.LogError("Corso non trovato.");
        }
    }

    public void CambiaVisibilitaVotoPanel()
    {
        if (inserimentoVotoPanel != null)
        {
            bool isActive = !inserimentoVotoPanel.activeSelf;
            inserimentoVotoPanel.SetActive(isActive);
        }
    }

    public void calcolaTutteLeMedie()
    {
        medieCorsi.Clear();
        foreach (Corso corso in corsi)
        {
            double somma = 0;
            double pesoTotale = 0;
            foreach (Voto voto in corso.voti)
            {
                somma += voto.getPunteggioEffettivo();
                pesoTotale += voto.peso;
            }

            double media;
            if (pesoTotale > 0)
            {
                media = somma / pesoTotale;
            }
            else
            {
                media = 0;
            }
            medieCorsi[corso] = media;
        }
    }


    void InitializeMateriaDropdown()
{
    if (materiaDropdown != null)
    {
        materiaDropdown.ClearOptions();
        List<string> opzioni = new List<string>();
        foreach (Corso corso in corsi)
        {
            opzioni.Add(corso.materia);
        }
        materiaDropdown.AddOptions(opzioni);
        materiaDropdown.value = 0;
        materiaDropdown.RefreshShownValue();
        
        //listener per il cambio di materia quando si clicca nel menui
        materiaDropdown.onValueChanged.AddListener(OnMateriaDropdownChanged);
    }
    else
    {
        Debug.LogError("Dropdown materie non assegnato");
    }
}

private void OnMateriaDropdownChanged(int index)
{
    if (index >= 0 && index < corsi.Count)
    {
        currentCorso = corsi[index];
        Debug.Log("Corso corrente cambiato a: " + currentCorso.materia);
    }
}

    void InitializeTipoEsameDropdown()
    {
        if (tipoEsameDropdown != null)
        {
            tipoEsameDropdown.ClearOptions();
            List<string> opzioni = new List<string>();
            foreach (TipoEsame tipo in Enum.GetValues(typeof(TipoEsame)))
            {
                opzioni.Add(tipo.ToString());
            }
            tipoEsameDropdown.AddOptions(opzioni);
            tipoEsameDropdown.RefreshShownValue();
        }
    }

    public void aggiungiVotoButtonClick()
    {
        Debug.Log("Valore Valutazione: " + valutazioneInputField.text);
        Debug.Log("Valore Peso: " + pesoInputField.text);
        Debug.Log("Valore Data: " + dataInputField.text);

        if (!inserimentoVotoPanel.activeSelf)
        {
            Debug.LogWarning("Il pannello di inserimento è disattivato");
            return;
        }

        if (currentCorso == null)
        {
            Debug.LogError("Nessun Corso selezionato a cui aggiungere un Voto.");
            return;
        }

        if (!double.TryParse(valutazioneInputField.text, out double valutazione))
        {
            Debug.LogError("Input Valutazione non valido. Inserisci un numero.");
            return;
        }
        if (!double.TryParse(pesoInputField.text, out double peso))
        {
            Debug.LogError("Input Peso non valido. Inserisci un numero.");
            return;
        }

        DateTime data;
        if (!DateTime.TryParseExact(dataInputField.text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
        {
            Debug.LogError("Input Data non valido. Utilizza il formato AAAA-MM-GG.");
            return;
        }

        TipoEsame tipo = (TipoEsame)tipoEsameDropdown.value;
        Voto nuovoVoto = new Voto(valutazione, peso, data, tipo);
        aggiungiVoto(currentCorso, nuovoVoto);

        Debug.Log($"Added Voto: {nuovoVoto.valutazione} (Peso: {nuovoVoto.peso}) to {currentCorso.materia}");

        DisplayVotoInUI(nuovoVoto);
        ResetInputFields();
        inserimentoVotoPanel.SetActive(false);
    }

    void ResetInputFields()
    {
        if (valutazioneInputField != null)
        {
            valutazioneInputField.text = "";
        }

        if (pesoInputField != null)
        {
            pesoInputField.text = "";
        }

        if (dataInputField != null)
        {
            dataInputField.text = "";
        }

        if (tipoEsameDropdown != null)
        {
            tipoEsameDropdown.value = 0;
        }
    }

    void DisplayVotoInUI(Voto voto)
{
    if (valutazioniContentPanel == null)
    {
        Debug.LogError("Il Pannello Contenuto Valutazioni non è assegnato");
        return;
    }

    if (votoUIPrefab != null)
    {
        GameObject votoUI = Instantiate(votoUIPrefab, valutazioniContentPanel);
        
        LayoutElement layoutElement = votoUI.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = votoUI.AddComponent<LayoutElement>();
        }
        layoutElement.preferredWidth = dimensioneCella.x;
        layoutElement.preferredHeight = dimensioneCella.y;

        TextMeshProUGUI testo = votoUI.GetComponentInChildren<TextMeshProUGUI>();
        if (testo != null)
        {
            testo.text = $"Voto: {voto.valutazione}\nPeso: {voto.peso}\nData: {voto.data:yyyy-MM-dd}\nTipo: {voto.tipo}\n Materia: {currentCorso.materia}";
        }
        else
        {
            Debug.LogWarning("Nessun componente TextMeshPro trovato nel prefab");
        }
    }
    else
    {
        Debug.LogWarning("Prefab UI Voto non assegnato");
    }

    AggiornaLayout();
}

}