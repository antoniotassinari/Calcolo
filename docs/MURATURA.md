# Muratura e cerchiatura

Modulo WPF .NET 10, MVVM con classi separate, richiamabile dalla finestra principale. Porting del sito `http://192.168.5.124:8080/`, acquisito il 7 novembre 2026. La copia dei file pubblicati è in `archive/muratura-web-2026-11-07`. Non richiede il server o una connessione per calcolare.

## Uso

- Schede Prima/Dopo: da 1 a 10 maschi, geometria in cm, compressione N in kN oppure q in kN/m. Il comando Uniforma altezze copia una volta l'altezza del primo maschio; le successive modifiche restano individuali.
- Materiale per maschio oppure applicazione a tutti: scegliere catalogo e livello, quindi premere Applica. I campi numerici restano modificabili; modifiche ai valori tabellari producono un materiale personalizzato. Cambiare il solo campo LC mantiene i parametri inseriti e modifica FC; per ricaricare minimi/medie tabellari usare Applica materiale.
- Resistenze fm/τ prima di FC in daN/cm²; E/G in MPa non fessurati; peso specifico in daN/m³. Per pietra tenera il peso va inserito esplicitamente. LC1: minimi resistenti, FC 1,35; LC2: medie, FC 1,20. LC3 è disponibile per dati valutati dal professionista; non deriva una caratterizzazione sperimentale dal catalogo.
- Sette coefficienti nell'ordine indicato. Tutti influenzano le resistenze; ricorsi e connessione trasversale non moltiplicano E/G. Fessurazione dimezza E/G una sola volta.
- Cerchiatura: attivare con Sì; impostare colonne, profili per colonna, altezza, acciaio e portale. Progetta esamina HEA, HEB, IPE e propone la prima taglia con ΔK entro ±15% e V finale strettamente maggiore del prima. Una proposta vuota significa nessuna soluzione.
- Calcola popola la scheda Risultati. Modificare i dati annulla risultati e proposte precedenti. Relazione stampabile salva HTML con input e risultati; aprire nel browser e stampare o salvare PDF.
- Apri legge i JSON del sito (versione 1/2); Salva produce versione 2 compatibile con il sito. Le impostazioni grafiche del sito non vengono conservate. L'altezza efficace è sempre quella geometrica; dopo l'importazione viene ricordato di verificare eventuali altezze efficaci del progetto originale.

## Moduli interoperabili

- `STA.Materiali`: catalogo di otto murature e fattori di confidenza, senza dipendenze da interfaccia o calcolo.
- `STA.Profilati.Core`: catalogo separato di 66 profili OPPO, versione `oppo-2026-11-06`, con fonti e unità. Dati indicativi della copia originale, non aggiornati automaticamente.
- `STA.Muratura.Core`: contratti, importazione/migrazione JSON, calcolo maschi, totali, telaio e ricerca profili. Libreria .NET 10 portabile, senza WPF.
- Desktop: View, ViewModel, servizio di relazione e classi di modifica separate. Il code-behind contiene soltanto l'inizializzazione della finestra.

## Campo del confronto

Si riproducono le formule del sito/foglio: somma dei soli Vt per fessurazione diagonale, rigidezza K, interpolazione du del foglio e telaio delle sole colonne sull'asse forte. Non è una verifica strutturale completa: esclusi scorrimento, pressoflessione, traverso, nodi, ancoraggi, instabilità, interazione N-M, taglio e classe della sezione. Il portale aperto cambia K, non V, come nell'originale. W* conserva l'ipotesi di due colonne. Non si aggiungono automaticamente peso proprio o combinazioni di carico.

La parità numerica verifica la fedeltà al codice acquisito, non certifica le formule. `STA.Muratura.Tests` controlla 164 casi generati dai motori JavaScript originali, tutte le tipologie e tutti i profili nei due portali, ricerca, importazione legacy e dati non validi. `STA.Desktop.Smoke` verifica modifica, salvataggio/apertura, materiali, relazione e visualizzazione. Rigenerazione riferimenti: `node tools/generate-masonry-reference.cjs`.
