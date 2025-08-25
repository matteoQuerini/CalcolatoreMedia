using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

//MonoBehaviour classe base di tutti gli script

public class GameManager: MonoBehaviour {
  private List<Corso> corsi;
  public Dictionary<Corso, double> medieCorsi;

  public Transform valutazioniContentPanel;
  public GameObject votoUIPrefab;
  private GridLayoutGroup gridLayoutGroup;

  //configurazione griglia
  public int colonne = 3;
  public Vector2 dimensioneCella = new Vector2(180, 100);
  public Vector2 spaziatura = new Vector2(10, 10);

  public GameObject inserimentoVotoPanel;
  public InputField valutazioneInputField;
  public InputField pesoInputField;
  public InputField dataInputField;
  public TMP_Dropdown tipoEsameDropdown;
  public Button Aggiungi;

  public TMP_Dropdown materiaDropdown;

  public GameObject mediePanel;
  public GameObject mediaUIPrefab;
  public Button mostraMedieButton;

  private string ordinamentoCorrente = "corso";
  
    //usato per inizializazione degli obj 
  void Awake() {
    corsi = new List<Corso>();
    //dictionary = hashmap
    medieCorsi = new Dictionary<Corso, double>();

    gridLayoutGroup = valutazioniContentPanel.GetComponent<GridLayoutGroup>();
    if (gridLayoutGroup == null) {
      gridLayoutGroup = valutazioniContentPanel.gameObject.AddComponent<GridLayoutGroup>();
    }

    ConfiguraGriglia();
  }

  void Start() {
    //controllano se gli obj sono assegnati al GameManager
    if (Aggiungi != null)
    {
      Aggiungi.onClick.AddListener(aggiungiVotoButtonClick);
    }

    if (inserimentoVotoPanel != null) {
      inserimentoVotoPanel.SetActive(false);
    }

    if (mediePanel != null) {
      mediePanel.SetActive(false);
    }

    InitializeTipoEsameDropdown();

    //aggiunta di tutte le materie preimpostate
    aggiungiCorso(new Corso("Matematica"));
    aggiungiCorso(new Corso("Italiano"));
    aggiungiCorso(new Corso("Inglese"));
    aggiungiCorso(new Corso("Storia"));
    aggiungiCorso(new Corso("Sistemi"));
    aggiungiCorso(new Corso("Informatica"));

    mostraMedieButton.onClick.AddListener(ToggleMediePanel);

    InitializeMateriaDropdown();
    RefreshValutazioniGrid();
  }

  public void ConfiguraGriglia()
  {
    if (gridLayoutGroup != null)
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

//apre o chiude il pannello delle medie
  public void ToggleMediePanel()
  {
    bool isActive = !mediePanel.activeSelf;
    mediePanel.SetActive(isActive);

    if (isActive)
    {
      AggiornaMedieUI();
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

  //forza la ricostruzione del layout in unity
  private void AggiornaLayout()
  {
    LayoutRebuilder.ForceRebuildLayoutImmediate(valutazioniContentPanel.GetComponent<RectTransform>());
  }

  public void aggiungiCorso(Corso corso) {
    corsi.Add(corso);
    AggiornaMedieUI();
    InitializeMateriaDropdown();
    RefreshValutazioniGrid();
  }

  public void aggiungiVoto(Corso corso, Voto voto) {
    if (corsi.Contains(corso)) {
      corso.AggiungiVoto(voto);
      RefreshValutazioniGrid();
    }
  }

  public void CambiaVisibilitaVotoPanel() {
    if (inserimentoVotoPanel != null) {
      bool isActive = !inserimentoVotoPanel.activeSelf;
      inserimentoVotoPanel.SetActive(isActive);
    }
  }

  public void calcolaTutteLeMedie() {
    medieCorsi.Clear();
    foreach(Corso corso in corsi) {
      double somma = 0;
      double pesoTotale = 0;
      foreach(Voto voto in corso.voti) {
        somma += voto.getPunteggioEffettivo();
        pesoTotale += voto.peso;
      }

      double media;
      if (pesoTotale > 0) {
        media = somma / pesoTotale;
      } else {
        media = 0;
      }
      medieCorsi[corso] = media;
    }
  }

  public void AggiornaMedieUI() {
     //controlla se i mediaPanel e mediaUIPrefab sono assegnati
    if (mediePanel == null || mediaUIPrefab == null)
    {
      Debug.LogError("Componenti UI medie non assegnati");
      return;
    }

    calcolaTutteLeMedie();

    //transform è il ocmponente che ogni gameObject ha in unity
    //indica tutti i figli di mediePanel
    foreach (Transform child in mediePanel.transform)
    {
      //rimuovo dalla memoria i suuoi figli per non avere valori duplicati o sbagliati
      Destroy(child.gameObject);
    }

    foreach(Corso corso in corsi) {
      //controlla se il corso ha una media calcolata
      if (medieCorsi.ContainsKey(corso))
      {
        double media = medieCorsi[corso];
        //con Instantiate crea una copia di un obj esistente in mem
        GameObject mediaUI = Instantiate(mediaUIPrefab, mediePanel.transform);

        //forza il ridimensionamento per il grid layout per impostare uno standard di dimensioni
        LayoutElement layout = mediaUI.GetComponent<LayoutElement>();
        if (layout == null) layout = mediaUI.AddComponent<LayoutElement>();
        layout.preferredWidth = 150;
        layout.preferredHeight = 70;

        TextMeshProUGUI testo = mediaUI.GetComponentInChildren<TextMeshProUGUI>();
        if (testo != null)
        {
          testo.text = $"{corso.materia}\n{media.ToString("0.00")}";
        }
      }
    }
    //ricalcola il layout per aggiornare la visualizzazione
    LayoutRebuilder.ForceRebuildLayoutImmediate(mediePanel.GetComponent<RectTransform>());
  }

  void InitializeMateriaDropdown() {
    if (materiaDropdown != null) {
      materiaDropdown.ClearOptions();
      List<string> opzioni = new List<string>();
      foreach(Corso corso in corsi) {
        opzioni.Add(corso.materia);
      }

      materiaDropdown.AddOptions(opzioni);

      //non accede al dropdown se non ci sono opzioni
      if (corsi.Count > 0)
      {
        materiaDropdown.value = 0;
        materiaDropdown.RefreshShownValue();
      }
    }
  }

  void InitializeTipoEsameDropdown() {
    if (tipoEsameDropdown != null) {
      tipoEsameDropdown.ClearOptions();
      List<string> opzioni = new List<string>();
      //cicla i valori dell'enum e vengono convertiti in testo dal toString
      foreach (TipoEsame tipo in Enum.GetValues(typeof(TipoEsame)))
      {
        opzioni.Add(tipo.ToString());
      }
      tipoEsameDropdown.AddOptions(opzioni);
      tipoEsameDropdown.RefreshShownValue();
    }
  }

  public void aggiungiVotoButtonClick() {
    Debug.Log("Valore Valutazione: " + valutazioneInputField.text);
    Debug.Log("Valore Peso: " + pesoInputField.text);
    Debug.Log("Valore Data: " + dataInputField.text);

    if (!inserimentoVotoPanel.activeSelf) {
      Debug.LogWarning("Il pannello di inserimento è disattivato");
      return;
    }

    if (materiaDropdown.value < 0 || materiaDropdown.value >= corsi.Count) {
      Debug.LogError("Nessuna materia selezionata");
      return;
    }
    
    Corso corsoSelezionato = corsi[materiaDropdown.value];

    //TryParse serve per convertire stringhe in numeri decimali 
    // out double valutazione crea una variasbile con il risultato
    if (!double.TryParse(valutazioneInputField.text, out double valutazione))
    {
      Debug.LogError("Input Valutazione non valido. Inserisci un numero");
      return;
    }
    if (!double.TryParse(pesoInputField.text, out double peso)) {
      Debug.LogError("Input Peso non valido. Inserisci un numero");
      return;
    }

    DateTime data;
    //TryParseExact controlla se una stringa cossisponde ad un formato specifico
    //in questo caso AAAA-MM-GG
    if (!DateTime.TryParseExact(dataInputField.text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
    {
      Debug.LogError("Input Data non valido. Utilizza il formato AAAA-MM-GG");
      return;
    }

    //cast esplicito
    TipoEsame tipo = (TipoEsame)tipoEsameDropdown.value;
    Voto nuovoVoto = new Voto(valutazione, peso, data, tipo);
    aggiungiVoto(corsoSelezionato, nuovoVoto);

    Debug.Log($"Added Voto: {nuovoVoto.valutazione} (Peso: {nuovoVoto.peso}) to {corsoSelezionato.materia}");

    ResetInputFields();
    inserimentoVotoPanel.SetActive(false);
  }

  void ResetInputFields() {
    if (valutazioneInputField != null) {
      valutazioneInputField.text = "";
    }

    if (pesoInputField != null) {
      pesoInputField.text = "";
    }

    if (dataInputField != null) {
      dataInputField.text = "";
    }

    if (tipoEsameDropdown != null) {
      tipoEsameDropdown.value = 0;
    }
  }

  void DisplayVotoInUI(Voto voto, Corso corso) {
    if (valutazioniContentPanel == null) {
      Debug.LogError("Il Pannello Contenuto Valutazioni non è assegnato");
      return;
    }

    //controlla se il prefab votoUIPrefab è assegnato
    if (votoUIPrefab != null){
      //Instantiate crea una copia dei un obj
      GameObject votoUI = Instantiate(votoUIPrefab, valutazioniContentPanel);

      //creazione del contenitore del votop
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
        testo.text = $"Materia: {corso.materia}\nVoto: {voto.valutazione}\nPeso: {voto.peso}\nData: {voto.data:yyyy-MM-dd}\nTipo: {voto.tipo}";
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

  public void OrdinaPerDataUI() {
    ordinamentoCorrente = "data";
    RefreshValutazioniGrid();
  }

  public void OrdinaPerValutazioneUI() {
    ordinamentoCorrente = "valutazione";
    RefreshValutazioniGrid();
  }

  public void OrdinaPerCorsoUI() {
    ordinamentoCorrente = "corso";
    RefreshValutazioniGrid();
  }

  private void RefreshValutazioniGrid() {
    //distrugge tutti gli elementi esistenti
    foreach (Transform child in valutazioniContentPanel)
    {
      Destroy(child.gameObject);
    }

    //controlla se c'è un corso selezionaro
    if (corsi == null || corsi.Count == 0)
    {
      Debug.LogWarning("Nessun corso disponibile");
      return;
    }

    //crea una lista dei corsi con i propri voti per ordinarli insieme
    List<(Voto voto, Corso corso)> tuttiVoti = new List<(Voto, Corso)>();
    
    foreach (Corso corso in corsi)
    {
      if (corso.voti != null)
      {
        foreach (Voto voto in corso.voti)
        {
          tuttiVoti.Add((voto, corso));
        }
      }
    }

    //seleziona l'ordinamento ed ordina
    switch (ordinamentoCorrente) {
      //OrderByDescending è una funzione LINQ che ordina valori in modo decrescente datagli uan chiavr
      //tuttiVoti è una lista di tuple voto-corso
      //per ogni elemento si estrae dal voto o la data o la valutazione
      //usato il toList per convertire la lista che da un output OrderByDescending (IEnumerable<T>) in una lista normale
      case "data":
        tuttiVoti = tuttiVoti.OrderByDescending(x => x.voto.data).ToList();
        break;
      case "valutazione":
        tuttiVoti = tuttiVoti.OrderByDescending(x => x.voto.valutazione).ToList();
        break;
      case "corso":
      default:
        break;
    }

    //cicla le tuple e le visualizza mell interfaccia
    foreach ((Voto voto, Corso corso) elemento in tuttiVoti) {
    DisplayVotoInUI(elemento.voto, elemento.corso);
  }

    AggiornaLayout();
  }
}