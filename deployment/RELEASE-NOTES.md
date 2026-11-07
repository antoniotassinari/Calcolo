STA Calcolo 1.1.0: aggiunto il modulo Muratura e cerchiatura alla suite WPF .NET 10 con MVVM e classi separate.

- Stati prima/dopo, carichi N o q e materiali per ciascun maschio.
- Catalogo murature nella libreria materiali condivisa; 66 profili OPPO in una libreria indipendente.
- Cerchiatura e ricerca preliminare HEA/HEB/IPE, importazione JSON del sito e relazione stampabile.
- 164 confronti numerici con il codice originale e prove della finestra WPF.

- Setup Windows x64 per utente, con runtime .NET incluso.
- Aggiornamento automatico dalle release GitHub prima dell'apertura dei progetti.
- Pacchetti verificati con SHA-256, backup e ripristino in caso di interruzione.
- Controllo manuale degli aggiornamenti dalla finestra principale.

Scaricare **STA-Calcolo-Setup-1.1.0-win-x64.exe**. Il file ZIP Update è destinato all'aggiornatore, non all'installazione manuale. Windows può mostrare il controllo del produttore sconosciuto perché il setup non è firmato Authenticode.

La migrazione tecnologica conserva i limiti dei calcoli storici. Per le funzioni e le compatibilità residue consultare `docs/WPF-MIGRATION.md`; istruzioni di installazione in `deployment/INSTALLAZIONE.md`.
