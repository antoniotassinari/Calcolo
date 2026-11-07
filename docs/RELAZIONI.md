# Relazioni Word e PDF

Dopo **Calcola dettagli**, usare **Relazione Word** oppure **Relazione PDF**. I documenti sono generati dal progetto validato: una modifica ai dati disabilita l'esportazione fino al nuovo calcolo.

Il formato è A4 verticale, con margini di 15 mm. La relazione contiene:

1. Confronto dei totali prima, dopo muratura, contributo del telaio, totale finale e variazione percentuale; esito dei criteri numerici.
2. Percorso di calcolo subito dopo la sintesi: equazioni numerate, frazioni, definizioni e unità, contributi muratura/telaio e sostituzioni numeriche finali. Stessa sezione espandibile nella finestra. Le formule Word sono equazioni Office modificabili. La notazione ingegneristica non attribuisce al modello una conformità normativa non verificata.
3. Schede dei maschi, due affiancate per pagina, con geometria, materiale, coefficienti, proprietà corrette, carichi e risultati. Prima in blu, dopo in petrolio.
4. Tabelle riepilogative: una riga per maschio, separate per prima e dopo, con legenda.
5. Geometria, acciaio, profilo e contributo effettivo della cerchiatura, quando attivata.
6. Metodo, significato delle grandezze e limiti del confronto.

Il Word è un `.docx` con testo e tabelle modificabili. Il PDF contiene testo selezionabile e caratteri incorporati, senza passare da Word o da una stampante virtuale. Le modifiche manuali al Word non modificano il progetto e non ricalcolano i risultati.

`STA.Calcolo.Reporting` è una libreria portabile: il costruttore della relazione usa i Model di calcolo; due scrittori distinti producono DOCX e PDF dalla stessa rappresentazione. La ViewModel richiede l'esportazione attraverso `IMasonryReportService`; nessuna automazione Office o logica di impaginazione è inserita nelle View.

La verifica comprende esempio originale, venti maschi con tutti i tipi di muratura e progetto senza telaio. La lettura dei DOCX in Microsoft Word e l'ispezione delle pagine renderizzate hanno confermato impaginazione e margini. Il controllo PDF verifica pagine A4, numero delle schede e testo selezionabile. L'esempio originale produce 12 pagine; il caso con venti maschi e cerchiatura 19.
