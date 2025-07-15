using System.Collections.Generic;
using UnityEngine;

public class Corso
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
}