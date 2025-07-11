# Note Management Console Application

Un'applicazione console C# per la gestione di note utilizzando il pattern Command.

## Caratteristiche

- ✅ Gestione completa delle note (creazione, lettura, aggiornamento, eliminazione)
- ✅ Salvataggio automatico in formato JSON
- ✅ Ricerca per titolo e contenuto
- ✅ Interfaccia command-line intuitiva
- ✅ Pattern Command per estensibilità
- ✅ Supporto per .NET 8.0 e .NET 9.0

## Prerequisiti

- .NET 8.0 o .NET 9.0 SDK
- Windows, macOS, o Linux

## Installazione

1. Clona il repository:
```bash
git clone https://github.com/[tuo-username]/note.git
cd note
```

2. Compila il progetto:
```bash
dotnet build
```

3. Esegui l'applicazione:
```bash
dotnet run
```

## Utilizzo

L'applicazione fornisce un'interfaccia command-line con i seguenti comandi:

### Comandi disponibili

- `add` - Aggiungi una nuova nota
- `list` - Mostra tutte le note
- `show <id>` - Mostra una nota specifica
- `edit <id>` - Modifica una nota esistente
- `delete <id>` - Elimina una nota
- `search <termine>` - Cerca nelle note
- `help` - Mostra l'aiuto
- `exit` - Esci dall'applicazione

### Esempi

```bash
# Aggiungi una nuova nota
> add

# Lista tutte le note
> list

# Mostra la nota con ID 1
> show 1

# Cerca "importante" nelle note
> search importante

# Modifica la nota con ID 2
> edit 2

# Elimina la nota con ID 3
> delete 3
```

## Struttura del progetto

```
note/
├── Program.cs              # Punto di ingresso e logica principale
├── note.csproj            # File di progetto
├── note.sln               # Solution file
├── global.json            # Configurazione .NET
└── README.md              # Questo file
```

## Funzionalità tecniche

### Pattern Command
L'applicazione utilizza il pattern Command per una struttura modulare e facilmente estensibile.

### Salvataggio automatico
Le note vengono salvate automaticamente in un file JSON (`note.json`) ad ogni modifica.

### Gestione degli errori
L'applicazione gestisce gracefully gli errori di input e operazioni non valide.

## Sviluppo futuro

- [ ] Integrazione con database Oracle
- [ ] Supporto per categorie/tag
- [ ] Export in diversi formati (PDF, Word, etc.)
- [ ] Sincronizzazione cloud
- [ ] Interfaccia web

## Contribuire

1. Fork il progetto
2. Crea un branch per la tua feature (`git checkout -b feature/AmazingFeature`)
3. Commit le tue modifiche (`git commit -m 'Add some AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Apri una Pull Request

## Licenza

Questo progetto è distribuito sotto la licenza MIT. Vedi il file `LICENSE` per maggiori dettagli.

## Autore

**Daniele Franceschini**

## Supporto

Se hai domande o problemi, apri un issue su GitHub.
