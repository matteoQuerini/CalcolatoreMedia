using System;
using UnityEngine;
using System.Collections.Generic;

public class Manager : Ordinabile<Corso>
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


    public void BubbleSort(Comparison<Corso> t)
    {
        for (int i = 0; i < corsi.Count - 1; i++)
        {
            for (int j = 0; j < corsi.Count - i - 1; j++)
            {
                if (t(corsi[j], corsi[j + 1]) > 0)
                {
                    Corso temp = corsi[j];
                    corsi[j] = corsi[j + 1];
                    corsi[j + 1] = temp;
                }
            }
        }
    }

    public void OrdinaPerMateria()
    {
        BubbleSort((c1, c2) => string.Compare(c1.materia, c2.materia));
    }

    public void OrdinaPerMedia()
    {
        calcolaTutteLeMedie();
        BubbleSort((c1, c2) => medieCorsi[c1].CompareTo(medieCorsi[c2]));
    }

}