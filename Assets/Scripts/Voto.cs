using UnityEngine;
using System;

public class Voto {
  public double valutazione;
  public double peso;
  public DateTime data;
  public TipoEsame tipo;

  public Voto(double valutazione, double peso, DateTime data, TipoEsame tipo) {
    this.valutazione = valutazione;
    this.peso = peso;
    this.tipo = tipo;
    this.data = data;
  }

  public double GetValutazione() {
    return valutazione;
  }

  public void SetValutazione(double value) {
    valutazione = value;
  }

  public double GetPeso() {

    return peso;
  }

  public void SetPeso(double value) {

    peso = value;
  }

  public DateTime GetData() {
    return data;
  }

  public void SetData(DateTime value) {
    data = value;
  }

  public TipoEsame GetTipo() {
    return tipo;
  }

  public void SetTipo(TipoEsame value) {
    tipo = value;
  }

  public double getPunteggioEffettivo() {
    return GetValutazione() * GetPeso();
  }

}