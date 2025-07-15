using System.Collections.Generic;
using UnityEngine;
using System;

public class Corso : Ordinabile<Voto>
{
    public string materia;
    public List<Voto> voti;
    public Corso(string materia)
    {
        this.materia = materia;
        this.voti = new List<Voto>();
    }

    public void AggiungiVoto(Voto voto)
    {
        voti.Add(voto);
    }


    public double CalcolaMedia()
    {
        double sommaPonderata = 0;
        double pesoTotale = 0;

        foreach (Voto v in voti)
        {
            sommaPonderata += v.getPunteggioEffettivo();
            pesoTotale += v.peso;
        }

        if (pesoTotale > 0)
        {
            return sommaPonderata / pesoTotale;
        }
        else
        {
            return 0;
        }
    }

    public void BubbleSort(Comparison<Voto> t)
    {
        for (int i = 0; i < voti.Count - 1; i++)
        {
            for (int j = 0; j < voti.Count - i - 1; j++)
            {
                if (t(voti[j], voti[j + 1]) > 0)
                {
                    Voto temp = voti[j];
                    voti[j] = voti[j + 1];
                    voti[j + 1] = temp;
                }
            }
        }
    }

    public void OrdinaPerData()
    {
        BubbleSort((v1, v2) => v2.data.CompareTo(v1.data));
    }

    public void OrdinaPerValutazione()
    {
        BubbleSort((v1, v2) => v2.valutazione.CompareTo(v1.valutazione));
    }
}