# Recupero e migrazione del 4 novembre 2026

Base GitHub: commit `48b533d`, branch `master` di `antoniotassinari/Calcolo`.

Il prototipo non era stato pubblicato su GitHub. È stato ricostruito dai diff completi e dai comandi di scrittura registrati nella chat **Modifica Calcolo**, ID `01a1a2af-42c8-72f6-b377-df6947ae2071`, del 3 novembre 2026. Sono stati recuperati motore JavaScript, client API, interfaccia web, server locale, guida, test e abbozzo C#. Il file C# Models.cs è ricostruito dal generatore registrato. Non è stata recuperata una copia binaria dello ZIP originario, né i manuali MIDAS allegati.

Il prototipo web è in `archive/midas-plinti-web`; il port C# interrotto è conservato nella sua sottocartella `dotnet`. L'implementazione attiva è in `src/STA.Plinti.Core`, con reazioni spostate nel contratto comune e validazione condivisa. Non sviluppare ulteriormente il motore nell'archivio.

Tutte le cartelle dei progetti storici sono state trasferite in `legacy`, mantenendo i file non estratti. I modelli/calcoli sono stati spostati in `src`, non duplicati: i vecchi progetti li richiamano tramite ProjectReference. Corrette due referenze a progetti mancanti e le inclusioni Compile duplicate nel progetto vento. Materiali e sezioni rimangono utilizzabili dalle vecchie interfacce con ricompilazione.

Correzioni motivate dai test durante l'estrazione:

- Il baricentro accedeva a `VerticiIn[Count]`: corretto con la formula del poligono chiuso e rifiuto di sezioni degeneri. Test su rettangolo, ordine inverso e triangolo traslato.
- L'alias `FortranArea2D` usa ora `Area2D` gestito ed è deprecato; non serve `ForTranGeometria.dll` per le librerie portabili.
- Separati tre metodi di visualizzazione WinForms dal modello sismico.

Limiti conservati: formule storiche non riesaminate integralmente; interfacce Prism/Ribbon/COM ancora in `legacy`; nuova UI desktop dedicata ai plinti. La build originale della soluzione falliva su ResolveComReference prima della migrazione. La nuova soluzione non include quei componenti e non ne certifica il funzionamento. Il collaudo MIDAS reale e la verifica normativa completa sono attività distinte.

Controlli: build Release della soluzione, test di calcolo e contratti, test HTTP simulati, 16 test JavaScript, confronto numerico su nove scenari e smoke WPF con esempio, calcolo e invalidazione. Il confronto C#/JS dimostra la conservazione del comportamento del prototipo, non la correttezza normativa delle formule.
