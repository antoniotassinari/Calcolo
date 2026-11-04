# Scambio dati v1

Il file `examples/plinti-example.json` è il modello completo del contratto. I nomi JSON sono case-sensitive: `schemaVersion`, `parameters`, `reactions`; tutti i parametri e le sei componenti sono obbligatori. Campi inattesi e versioni diverse da 1 sono rifiutati. Il formato è utilizzabile da C#, Python, Qt o processi esterni tramite CLI senza dipendere da WPF.

- Reazioni del vincolo **sulla struttura**: FX/FY/FZ in kN, MX/MY/MZ in kN·m; Z verso l'alto. FZ positiva comprime il plinto. Il motore effettua l'inversione dei momenti e il trasporto alla quota indicata.
- Geometria e quota nodo in m, pressione terreno in kPa, materiali in MPa, copriferro/diametro/passo in mm; armatura risultante in mm²/m.
- `limitState`: `ULS` oppure `SLS`. Il terreno è un dato del progettista, non viene dedotto automaticamente.
- Un record per nodo e combinazione concomitante. Inviluppi e duplicati sono rifiutati.
- Non trasferire direttamente coordinate storiche `Punto2D` nei plinti: il contratto storico documenta cm; il contratto plinti usa m. La conversione deve essere esplicita.

CSV semplice: intestazione `node,case,FX,FY,FZ,MX,MY,MZ`, separatore virgola o punto e virgola, punto decimale. Non sono supportati campi quotati o nomi contenenti il separatore.

L'output CLI e WPF contiene `input` e `report`, con versione del motore, unità, ambito e risultati per combinazione. Grandezze non applicabili vengono serializzate come `null`; leggere sempre esiti e messaggi. La riuscita dell'esecuzione CLI (exit code 0) significa elaborazione riuscita, non esito strutturale favorevole. Errori input: exit code 1; sintassi CLI errata: 2.

L'adattatore MIDAS richiede il Base URL HTTPS da API Settings e la MAPI-Key in memoria. Esegue GET `db/NODE` e POST `post/TABLE` con `REACTIONG`, `KN`, `M`. Rifiuta redirect, colonne ambigue, risultati incompleti o fuori selezione. Il formato è derivato dal prototipo: va confrontato con la versione GEN NX effettivamente usata. Nessuna chiave viene salvata nei progetti o nelle relazioni.
