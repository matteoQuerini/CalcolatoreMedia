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

    public Button Aggiungi;
    public Transform valutazioniContentPanel;
    public Corso currentCorso;
    public GameObject votoUIPrefab;
    public GameObject inserimentoVotoPanel;

    //Chiamato una sola volta all'inizio prima di qualsiasi altro metodo, simile a start(inizializzare variabili/strutture dati)
    void Awake()
    {
        corsi = new List<Corso>();
        medieCorsi = new Dictionary<Corso, double>();
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

        Corso matematica = new Corso("Matematica");
        aggiungiCorso(matematica);
        currentCorso = matematica;
        Debug.Log("Corso Matematica aggiunto e impostato come corrente");
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
            //aggiunge le stringhe della lista opzioni al menu a tendina del dropdown
            tipoEsameDropdown.AddOptions(opzioni);

            //aggiuorna la visualizazione del menu a tendina
            tipoEsameDropdown.RefreshShownValue();
        }
    }

    public void aggiungiVotoButtonClick()
    {


        Debug.Log("Valore Valutazione: " + valutazioneInputField.text);
        Debug.Log("Valore Peso: " + pesoInputField.text);
        Debug.Log("Valore Data: " + dataInputField.text);

        //debug per controllare se il pannello dell inserimento voit è attivo

        if (!inserimentoVotoPanel.activeSelf)
        {
            Debug.LogWarning("Il pannello di inserimento è disattivato");
            return;
        }



        //controlla se è stato selezionato un corso a cui aggiungere il voto
        if (currentCorso == null)
        {
            Debug.LogError("Nessun Corso selezionato a cui aggiungere un Voto.");
            return;
        }

        //converte l'input della valutazione in numero, se non riesce stampa un errore (string to doublòe)
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
        //converte l'input della data in DateTime, se non riesce stampa un errore (string to DateTime)
        if (!DateTime.TryParseExact(dataInputField.text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
        {
            Debug.LogError("Input Data non valido. Utilizza il formato AAAA-MM-GG.");
            return;
        }

        //controlla se il tipo di esame è stato selezionato
        TipoEsame tipo = (TipoEsame)tipoEsameDropdown.value;

        //crea un nuovo oggetto Voto con i dati inseriti
        Voto nuovoVoto = new Voto(valutazione, peso, data, tipo);

        //aggiunge il voto al corso corrente
        aggiungiVoto(currentCorso, nuovoVoto);

        //$ usato per usare le {}
        Debug.Log($"Added Voto: {nuovoVoto.valutazione} (Peso: {nuovoVoto.peso}) to {currentCorso.materia}");

        //chiama la funzione per visualizzare il voto nella UI
        DisplayVotoInUI(nuovoVoto);

        //resetta i campi di input
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

            TextMeshProUGUI[] testi = votoUI.GetComponentsInChildren<TextMeshProUGUI>();

            if (testi.Length > 0)
            {
                 testi[0].text = $"voto: {voto.valutazione}\npeso: {voto.peso}\ndata: {voto.data:yyyy-MM-dd}";
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
    }
}