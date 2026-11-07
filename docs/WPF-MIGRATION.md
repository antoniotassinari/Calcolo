# Suite WPF .NET 10 · 7 novembre 2026

La finestra principale apre 11 ambienti di lavoro. Le viste condividono modelli e servizi, non controlli WinForms incorporati. Le finestre già aperte vengono riattivate; chiusura e riapertura nello stesso processo conservano il documento. Salvare in JSON per conservarlo dopo la chiusura dell'applicazione.

| Interfaccia precedente | Ambiente attuale | Funzioni provate |
| --- | --- | --- |
| FinestraTest, STA.Interfaccia/Shell, shell Ribbon/Prism | Finestra principale | Apertura, riattivazione, conservazione dello stato |
| VisualizzaMateriali, ucPropCLS | Calcestruzzo | Tutte le classi, proprietà, JSON |
| VisualizzaMateriali, ucPropAcciaioArmatura | Acciaio per armature | Classi, proprietà, JSON |
| ucCaricoNeve e moduli carichi | Carico neve | Zona, topografia, quota, falda, qsk e qs con Ct=1 |
| ucVento e moduli carichi | Carico vento | Zona, rugosità, costa, quota, altezza, pressioni |
| ucSisma | Spettri sismici | Coordinate, vita nominale, classe d'uso, terreno, q, elastici SLO/SLD/SLV/SLC e progetto SLV, direzione H/V, grafico e risultati |
| Prefabbricato/MainWindow, SezioneEditView | Sezioni geometriche | Vertici, area, baricentro, controllo del contorno, grafico |
| ClasseGestioneTrefolaturaWorkSpaceView | Trefolature | Coordinate e aree, tiro, baricentro, N/M iniziali, grafico |
| STA_Prefabbricato/MainForm, STA.Prefabbricato.Trave.Interfaccia/MainWindow | Travi prefabbricate | Importazione/rimozione di sezioni e trefolature, documento comune, salvataggio/riapertura |
| plintoSingoloForm, ucCalcoloPlintoSingolo, editor plinto e combinazioni | Plinto singolo storico | Più combinazioni di un nodo, superficie/pali rettangolari/pali triangolari |
| formProgettoPlintiProgExt, Main dimensionamento plinti | Progetto plinti storici | Più nodi/combinazioni con la geometria impostata, risultati per riga |
| Applicazione plinti recuperata dalla chat | Plinti · reazioni e MIDAS | JSON, CSV, calcolo, export, MIDAS asincrono |
| STA.Grafica/ucGrafico2D | PlotView WPF | Grafico vettoriale sezioni, trefoli e spettri |

## Architettura e correzioni

Viste, ViewModel, comandi, servizi e modelli in classi/file separati. I calcoli restano nelle librerie portabili. I documenti dei moduli hanno schemaVersion 1 e identificativo modulo: un file di sezione può essere importato nel progetto trave senza conversioni manuali. CLI e desktop usano gli stessi adattatori.

Correzioni verificate: parsing del reticolo sismico con punto decimale indipendente dalla lingua Windows; gestione del punto esatto del reticolo senza divisione per zero e rifiuto di coordinate fuori copertura; notifiche vento senza sottoscrittori; elenco sezioni della trave inizializzato; massimo sui pali calcolato su tutti i pali, anziché sull'ultimo; peso del terreno soprastante calcolato escludendo lo spessore del calcestruzzo. L'adattatore dei plinti rifiuta torsione/sollevamento e basi efficaci negative. La geometria rifiuta poligoni degeneri, duplicati e autointersecanti.

I risultati e i grafici vengono invalidati immediatamente a ogni modifica. L'importazione MIDAS tardiva viene scartata se il progetto è cambiato nel frattempo. La chiave API non entra nei file di progetto o nei risultati.

## Verifiche e limiti

40 test C# comprendono esempi di tutti gli adattatori in it-IT/en-US, riferimenti di equilibrio indipendenti, input invalidi, contratti e scambio dati travi. Lo smoke WPF verifica apertura degli 11 ambienti, comandi, salvataggio/riapertura, esportazione, importazione/rimozione dei moduli trave, conservazione dello stato, invalidazione e rendering. Restano i 16 test JS e il confronto C#/JS su 9 scenari del motore plinti recuperato.

L'aggiornamento è tecnologico: le formule storiche non sono state adeguate integralmente alla normativa corrente. Ogni ambiente espone il proprio ambito. Il progetto originale delle travi non contiene un solutore completo: la nuova finestra organizza e controlla sezioni/trefolature, senza presentare una verifica strutturale della trave inesistente. I plinti storici sono controlli parziali; il taglio sui pali è esposto ma non verificato.

Compatibilità da distinguere dalle finestre: vecchi file .staxml, assegnazioni multiple di tipologie di plinto, esportazioni Word COM, esportazioni specifiche Robot/ETABS e integrazione VSTO/Excel non sono riprodotti nella nuova UI. Si usano i nuovi documenti JSON e CSV; non sovrascrivere i file storici. Le vecchie interfacce restano in legacy come riferimento. Le classi plinti estratte sono una copia adattata; per manutenzione del motore attivo usare src/STA.Plinti.Storici.Core, non legacy. Il collegamento MIDAS reale richiede ancora un modello GEN NX e risultati disponibili: i test presenti simulano il servizio HTTP.
