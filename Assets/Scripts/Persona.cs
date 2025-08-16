using UnityEngine;

public abstract class Persona {

  public string nome;
  public string cognome;
  public string email;

  public Persona(string nome, string cognome, string email) {
    this.nome = nome;
    this.cognome = cognome;
    this.email = email;
  }

  public string GetNome() {
    return nome;
  }
  public void SetNome(string value) {
    nome = value;
  }

  public string GetCognome() {
    return cognome;
  }
  public void SetCognome(string value) {
    cognome = value;
  }

  public string GetEmail() {
    return email;
  }
  public void SetEmail(string value) {
    email = value;
  }

}