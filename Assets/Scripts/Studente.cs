using UnityEngine;

public class Studente: Persona {
  public string matricola;

  //: è il super
  public Studente(string nome, string cognome, string email, string matricola): base(nome, cognome, email) {
    this.matricola = matricola;
  }

  public string GetMatricola() {
    return matricola;
  }
  public void SetMatricola(string value) {
    matricola = value;
  }
}