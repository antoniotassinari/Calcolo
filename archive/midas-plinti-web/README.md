# STA Plinti per MIDAS GEN NX

Plugin web locale per verificare e predimensionare plinti superficiali rettangolari dalle reazioni dei nodi assegnati. Un plinto per nodo e tutte le combinazioni importate per quel nodo. Versione 0.1.0.

## Avvio

Richiede Python 3 e un browser Chromium/Edge/Chrome aggiornato, senza librerie da installare. Su Windows avviare `Avvia_Plugin.bat`, oppure eseguire nella cartella:

```sh
python serve.py
```

Aprire http://127.0.0.1:8765. Il server ascolta solo su localhost. Chiuderlo con Ctrl+C. Non aprire direttamente index.html con file://: il browser deve caricare i moduli JavaScript via HTTP.

È una web app collegata alle API di GEN NX. Non è un installer né una pubblicazione nel catalogo MIDAS Apps: quel passaggio richiede il flusso di distribuzione supportato dalla versione MIDAS e non è stato eseguito. I file statici sono separati dal server locale e possono essere adattati a tale distribuzione.

## Uso

1. In GEN NX aprire il modello, terminare l'analisi e abilitare il collegamento API. Copiare Base URL e MAPI-Key dalle impostazioni effettive del programma.
2. Inserire gli ID dei nodi e i nomi esatti dei casi/combinazioni, uno per riga, includendo il suffisso richiesto da MIDAS, per esempio `SLU01(CB)`. Le sei componenti devono essere concomitanti e riferite agli assi globali. Il plugin non lancia l'analisi.
3. Confermare risultati aggiornati e importare le reazioni. Il programma controlla l'esistenza dei nodi e la presenza di ogni coppia nodo/combinazione. Richiede `REACTIONG` con unità `KN` e `M`; conserva richiesta e risposta per la relazione.
4. Inserire geometria, quota del nodo rispetto alla base, resistenza del terreno e attrito di progetto. Il terreno non ha valori presunti; il pulsante esempio inserisce valori puramente dimostrativi.
5. Assegnare materiali, armatura inferiore, coefficienti e stato limite. Le azioni MIDAS si assumono già combinate; il fattore dei pesi aggiunti va scelto coerentemente con ciascuna combinazione. Elaborare separatamente combinazioni che richiedono fattori o resistenze diversi.
6. Verificare la geometria oppure cercare la base quadrata minima su una griglia. La ricerca varia solo B e L, mantenendo altezza, armatura e quota nodo assegnate. Non applica alcuna modifica al modello MIDAS.
7. Consultare tutte le righe e le eccezioni. Esportare JSON o stampare in PDF con il browser. La relazione JSON contiene reazioni, dati, formule risultanti, origine e data, senza MAPI-Key. Il JSON esportato è una relazione; l'importazione JSON nell'interfaccia accetta invece la risposta grezza HEAD/DATA di MIDAS.

In alternativa è possibile incollare/importare CSV (punto decimale; virgola o punto e virgola come separatore):

```csv
node,case,FX,FY,FZ,MX,MY,MZ
101,SLU01(CB),20,10,900,30,45,0
101,SLU02(CB),-15,5,750,-25,35,0
```

L'importazione manuale assume kN e kN·m: verificare queste unità prima del calcolo. Non sono gestiti nomi con separatori CSV. I valori mancanti e non finiti sono rifiutati. Gli inviluppi CB:max/min/all, risultati spettrali, moving load e time history sono rifiutati: occorre fornire sestuple concomitanti già valutate dal progettista. Non si sommano reazioni di nodi differenti né si ricombinano casi elementari.

## Metodo

Il riferimento di partenza è `STA_Dimensionamento Plinti/verifiche/verificaMeyerhof.cs`, dal quale si riprende il metodo dell'area efficace. Il motore è stato riscritto in JavaScript con unità esplicite e controlli sull'area positiva, senza le dipendenze Windows Forms/WPF e Office del progetto storico. Non viene riutilizzata ciecamente la precedente aritmetica in cm/daN.

- Le reazioni sono del vincolo sulla struttura: sul plinto agisce la sestupla opposta. Con Z positivo verso l'alto, N = Rz + γG·W. Alla base Mx = −RMx + z·Ry e My = −RMy − z·Rx. Pilastro e nodo centrati sulla pianta.
- W = B·L·h·γcls + altri pesi centrati. Evitare duplicazioni con i pesi già modellati. Il fattore γG si applica solo a W. Non vengono applicati coefficienti ulteriori alle reazioni importate.
- B′ = B − 2|My/N|, L′ = L − 2|Mx/N|; qeff = N/(B′L′). Il valore qRd è una resistenza di progetto **assegnata**, non ricavata automaticamente da φ, c, falda, profondità o inclinazione dei carichi. La sua validità per le dimensioni provate deve essere confermata dal progettista.
- qmax/min = N/(BL) ± 6|My|/(LB²) ± 6|Mx|/(BL²). Il programma richiede contatto completo. In SLU confronta qeff con qRd; in SLE confronta qmax con il limite assegnato.
- Scorrimento: H/(μN), con μ già di progetto; nessun contributo di coesione o spinta passiva. Torsione non supportata.
- Flessione conservativa delle mensole con qmax uniforme lordo: m = qmax·a²/2 per metro. As = max[m/(0,9d·fyd), As,min]; As,min = max(0,26 fctm/fyk, 0,0013)·b·d. Limite m ≤ 0,167·fcd·b·d² per il campo semplificato; nessuna armatura superiore o compressa.
- Armatura inferiore uguale in X/Y. Altezza utile conservativa comune: d = h − copriferro − 1,5φ. Il passo è limitato a min(3h,400 mm). I dettagli di ancoraggio non sono verificati.
- Taglio monodirezionale a d senza armatura trasversale, EC2 6.2.2, σcp = 0. k ≤ 2, ρ ≤ 0,02 e minimo vmin.
- Screening del punzonamento sul perimetro a 2d, interamente interno alla base, con angoli arrotondati: u1 = 2(cx+cy)+4πd. Area interna = cx·cy+4d(cx+cy)+4πd². Si sottrae conservativamente qmin·A dalla risultante N. Verifica aggiuntiva alla faccia con vRd,max = 0,5νfcd. Il β assegnato non può essere inferiore alla stima 1+1,8√[(My/(N·cx))²+(Mx/(N·cy))²]. Non viene eseguita la ricerca di tutti i perimetri intermedi richiesta per una verifica completa di fondazione: l'esito è quindi uno screening, non l'attestazione completa di resistenza a punzonamento.

Formule CA nel campo fck 12–50 MPa, fyk 400–600 MPa. Riferimenti: EN 1992-1-1:2004 §§6.1, 6.2.2, 6.4, 9.3; EN 1997-1 §6.5.4. NTC 2018 cap. 6 come contesto progettuale: il software non implementa una verifica completa NTC né seleziona automaticamente approcci, coefficienti o combinazioni.

## Limiti espliciti

L'esito favorevole riguarda esclusivamente i controlli implementati. Non comprende cedimenti, fessurazione SLE, stabilità globale, liquefazione, verifiche sismiche specifiche, punzonamento su tutti i perimetri intermedi, ancoraggi, armature superiori, collegamento pilastro-plinto, plinti eccentrici o su pali, interazione tra plinti, torsione, contatto parziale o azioni concomitanti ricostruite da risultati non concomitanti. I dati geotecnici, l'orientamento, i segni e i coefficienti devono essere validati sul progetto reale.

## API e fonti

Schema consultato nel repertorio allegato `MIDAS_REPERTORIO.json`, voce **Reaction - Analysis Result Table**:

- https://support.midasuser.com/hc/en-us/articles/36009349748249-Reaction-Analysis-Result-Table
- `GET /db/NODE`: https://support.midasuser.com/hc/en-us/articles/35806845654169-Node
- `POST /post/TABLE`, `Argument.TABLE_TYPE = REACTIONG`, `NODE_ELEMS.KEYS`, `LOAD_CASE_NAMES`, `UNIT`.

La presenza di una voce nella raccolta comune CIVIL/GEN non garantisce compatibilità con ogni versione GEN NX. Il parser richiede una risposta HEAD/DATA con intestazioni riconoscibili, fallisce esplicitamente su formati diversi e non inventa colonne mancanti. Il primo collegamento reale va confrontato con una tabella esportata da GEN NX. Eventuali restrizioni CORS o di accesso devono essere risolte nella configurazione supportata dal fornitore, senza disattivare TLS.

La chiave resta in memoria nella pagina; nessun localStorage, analytics o dipendenza CDN. Le chiamate vanno solo al Base URL immesso dall'utente; il modello non viene scritto, cancellato o rianalizzato.

## Verifica del software

```sh
npm test
python tests/smoke_browser.py
```

Node.js è richiesto soltanto per i test. Lo smoke test richiede Chromium e il modulo Python websockets (solo per i test: python -m pip install websockets) e verifica caricamento dell'interfaccia, esempio, calcolo, invalidazione dei risultati e ricerca della base. Le prove numeriche confrontano equilibrio e pressioni con risultati manuali, casi non validi e payload API simulati. Nessuna prova su un programma GEN NX reale è stata eseguita in questo ambiente; serve un collaudo sul modello dello studio prima dell'uso progettuale.
