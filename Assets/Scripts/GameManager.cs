using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    List<Corso> corsi;
    public Dictionary<Corso, double> medieCorsi;

    public InputField valutazioneInputField;
    public InputField pesoInputField;
    public InputField dataInputField;
    public TMP_Dropdown tipoEsameDropdown;

    public Button aggiungiVotoButton;
    public Transform valutazioniContentPanel;
    public Corso currentCorso;
    public GameObject votoUIPrefab;

    //Chiamato una sola volta all'inizio prima di qualsiasi altro metodo, simile a start(inizializzare variabili/strutture dati)
    void Awake()
    {
        corsi = new List<Corso>();
        medieCorsi = new Dictionary<Corso, double>();
    }

    void Start()
    {
        if (aggiungiVotoButton != null)
        {
            aggiungiVotoButton.onClick.AddListener(OnAggiungiVotoButtonClick);
        }

        InitializeTipoEsameDropdown();

        Corso matematica = new Corso("Matematica");
        aggiungiCorso(matematica);
        currentCorso = matematica;
        Debug.Log("Corso fittizio 'Matematica' aggiunto e impostato come corrente.");
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

    public void OnAggiungiVotoButtonClick()
    {
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
            Debug.LogError("Invalid Peso input. Please enter a number.");
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

        valutazioneInputField.text = "";
        pesoInputField.text = "";
        dataInputField.text = "";
        tipoEsameDropdown.value = 0;
    }

    void DisplayVotoInUI(Voto voto)
    {
        if (valutazioniContentPanel == null)
        {
            Debug.LogError("Il Pannello Contenuto Valutazioni non è assegnato nell'Inspector.");
            return;
        }

        if (votoUIPrefab == null)
        {
            Debug.LogWarning("Il Prefab UI Voto non è assegnato. Visualizzazione con un elemento Text predefinito.");
            GameObject votoGO = new GameObject($"Voto_{voto.data.ToString("yyyyMMdd")}_{voto.valutazione}");
            votoGO.transform.SetParent(valutazioniContentPanel, false);
            Text votoText = votoGO.AddComponent<Text>();
            votoText.text = $"Voto: {voto.valutazione}, Peso: {voto.peso}, Data: {voto.data.ToShortDateString()}, Tipo: {voto.tipo}";
            votoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            votoText.color = Color.black;
            votoText.fontSize = 20;
            RectTransform rectTransform = votoGO.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(valutazioniContentPanel.GetComponent<RectTransform>().rect.width, 40);
            LayoutElement layoutElement = votoGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 40;
            layoutElement.flexibleHeight = 0;
            layoutElement.minHeight = 40;

        }
        else
        {
            GameObject votoGO = Instantiate(votoUIPrefab, valutazioniContentPanel);
            Text votoText = votoGO.GetComponentInChildren<Text>();
            if (votoText != null)
            {
                votoText.text = $"Voto: {voto.valutazione}, Peso: {voto.peso}, Data: {voto.data.ToShortDateString()}, Tipo: {voto.tipo}";
            }
            else
            {
                Debug.LogWarning("VotoUIPrefab does not have a Text component in its children. Please add one or modify DisplayVotoInUI.");
            }
        }
    }
}