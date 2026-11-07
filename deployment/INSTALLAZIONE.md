# Installazione e aggiornamenti di TA-Calcolo

Scaricare `TA-Calcolo-Setup-<versione>-win-x64.exe` dalle [release ufficiali](https://github.com/studiotassinari/Calcolo/releases/latest), eseguirlo e avviare **TA-Calcolo** dal menu Start. Windows 10/11 x64; il runtime .NET 10 è incluso. L'installazione è per utente e non richiede amministratore.

Come Commesse, il programma scarica i pacchetti dalle release GitHub, verifica SHA-256, crea un backup e sostituisce i file dell'applicazione con ripristino in caso di errore. Calcolo esegue questa operazione all'avvio, prima di aprire le finestre di lavoro. Non chiude progetti già aperti. Il pulsante **Verifica aggiornamenti** controlla la disponibilità: salvare e riaprire l'applicazione per applicare una nuova versione.

In assenza di rete o con download non valido si avvia la versione già installata. Una transazione interrotta viene ripristinata al successivo avvio. Se l’applicazione principale non riesce ad avviarsi, usare **Ripristina TA-Calcolo** nel menu Start: il ripristino è un eseguibile autonomo, escluso dalla sostituzione dei file. Un ripristino non riuscito impedisce l'avvio di file parzialmente aggiornati: i backup rimangono in `%LOCALAPPDATA%\StudioTassinari\Calcolo\Updates`.

I file applicativi sono in `%LOCALAPPDATA%\Programs\TA-Calcolo`; salvare i propri progetti nella cartella Documenti o nella cartella di lavoro. Non usare la directory di installazione per i documenti personali. Disinstallare da **Impostazioni → App → TA-Calcolo**; i progetti esterni e i backup degli aggiornamenti non vengono rimossi.

L'aggiornamento segue soltanto release stabili complete del repository `studiotassinari/Calcolo`; non segue i commit o le prerelease. La prima release è 1.0.0. I pacchetti non sono firmati Authenticode: Windows può mostrare il controllo del produttore sconosciuto.

## Pubblicare una versione

Aggiornare `version.json` e le versioni dei due progetti desktop/updater, integrare le modifiche in master, quindi creare e pubblicare il tag `vX.Y.Z` sul commit verificato. Il workflow Installer Windows compila con runtime incluso, esegue i test, verifica installazione/aggiornamento/disinstallazione e pubblica setup, ZIP di aggiornamento e SHA256SUMS. Il nome e l'AppId dell'installazione restano invariati.

Il codice di staging e transazione deriva dall'aggiornatore dell'applicazione Commesse, adattato al solo client Calcolo. La parte di presentazione segue MVVM, con classi separate.

Dalla versione 1.2.0 il nome visibile è TA-Calcolo. Identificativo di installazione, nomi interni degli eseguibili e pacchetto STA-Calcolo-Update restano stabili per permettere l’aggiornamento dalle versioni precedenti.
