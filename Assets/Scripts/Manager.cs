using System.Collections.Generic;
using UnityEngine;

public class Manager
{
    List<Corso> corsi;
    public Dictionary<Corso, double> medieCorsi;

    public Manager()
    {
        corsi = new List<Corso>();
        medieCorsi = new Dictionary<Corso, double>();
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


   public void calcolaTutteLeMedie(){
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

    

}