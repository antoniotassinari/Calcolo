# Calcolo · utility STA

Librerie C# .NET 10 riutilizzabili per calcolo strutturale, con applicazione WPF e interfaccia a riga di comando. Il modulo plinti importa reazioni da CSV, JSON o API MIDAS GEN NX e usa un solo motore di calcolo.

## Avvio

Prerequisiti: SDK .NET 10; Windows per WPF. Node.js 24 serve ai test del prototipo e ai controlli di architettura.

```powershell
dotnet build Calcolo.sln -c Release
dotnet run --project apps/STA.Calcolo.Desktop
dotnet run --project apps/STA.Calcolo.Cli -- plinti examples/plinti-example.json risultati.json
```

Nell'app: **Carica esempio**, modifica i parametri, importa le reazioni oppure leggile da MIDAS, quindi **Calcola**. L'esempio contiene dati sintetici, inclusi resistenza e attrito del terreno. Le reazioni importate devono essere in kN e kN·m, con Z globale verso l'alto. I parametri hanno unità esplicite. Salva progetto e risultati in JSON per riutilizzarli da altri programmi.

## Struttura

| Cartella | Contenuto |
| --- | --- |
| `src` | 13 librerie portabili: contratti, materiali, geometria, sezioni, neve, vento, sisma, trefolatura, travi, plinti, I/O, MIDAS e base carichi |
| `apps` | App WPF e CLI, entrambe collegate allo stesso motore plinti |
| `tests` | Test C# senza pacchetti esterni e smoke WPF |
| `examples` | Contratto JSON e reazioni CSV dimostrative |
| `legacy` | Tutti i 28 progetti storici, con riferimenti aggiornati alle librerie estratte |
| `archive/midas-plinti-web` | Prototipo recuperato dalla precedente chat, conservato per tracciabilità e confronto |
| `tools` | Controllo dipendenze e confronto numerico C#/JavaScript |

La soluzione principale compila solo i moduli attivi e i due host. `legacy/Calcolo.Legacy.sln` conserva la vecchia soluzione: alcune interfacce storiche richiedono COM, RibbonControlsLibrary, FsTaskButton o dipendenze Prism e non fanno parte della build principale. Il porting delle interfacce storiche non è completo; i relativi motori sono stati separati per consentirne il riuso.

## Verifiche

```powershell
node tools/verify-architecture.mjs
dotnet build Calcolo.sln -c Release
dotnet tests/STA.Calcolo.Tests/bin/Release/net10.0/STA.Calcolo.Tests.dll
node --test archive/midas-plinti-web/tests/api.test.js archive/midas-plinti-web/tests/engine.test.js
node tools/verify-reference.mjs
dotnet tests/STA.Desktop.Smoke/bin/Release/net10.0-windows/STA.Desktop.Smoke.dll
```

GitHub Actions esegue build e test su Windows, testa le librerie su Linux e produce un pacchetto desktop. Per il pacchetto framework-dependent è necessario il runtime .NET Desktop 10 sul PC destinatario.

Il calcolo dei plinti mantiene l'ambito del prototipo: controlli parziali di equilibrio, pressioni, scorrimento, flessione, taglio e punzonamento preliminare. Non costituisce una verifica normativa completa. Il collegamento MIDAS ha test simulati; il collaudo su GEN NX reale è ancora da eseguire. Le formule dei moduli storici non sono state aggiornate né certificate rispetto alla normativa corrente.

Vedi [architettura](docs/ARCHITECTURE.md), [recupero e migrazione](docs/MIGRATION.md), [inventario progetti](docs/PROJECTS.md) e [contratto dati](docs/DATA-CONTRACT.md).
