using UnityEngine;
using System;

public abstract class Voto{
    public double valutazione;
    public double peso;
    public DateTime data;

    public string descrizione;

    public Voto(double valutazione, double peso, DateTime data, string descrizione)
    {
        this.descrizione = descrizione;
        this.valutazione = valutazione;
        this.peso = peso;
        this.data = data;
    }

    public abstract double getPunteggioEffettivo();
}