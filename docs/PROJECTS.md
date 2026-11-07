# Inventario progetti

Generato da `node tools/update-catalog.mjs`. La compilazione principale comprende src/apps/tests; legacy conserva le interfacce e applicazioni storiche. Le dipendenze elencate sono riferimenti interni, non un giudizio di correttezza delle formule.

| Progetto | Percorso | Dipendenze interne |
| --- | --- | --- |
| STA.Calcolo.Cli | apps/STA.Calcolo.Cli/STA.Calcolo.Cli.csproj | STA.Calcolo.IO, STA.Calcolo.Application |
| STA.Calcolo.Desktop | apps/STA.Calcolo.Desktop/STA.Calcolo.Desktop.csproj | STA.Calcolo.IO, STA.Integrations.Midas, STA.Calcolo.Application, STA.Calcolo.Updates.Core, STA.Muratura.Core, STA.Calcolo.Reporting |
| STA.Calcolo.Updater | apps/STA.Calcolo.Updater/STA.Calcolo.Updater.csproj | STA.Calcolo.Updates.Core |
| Acciai | legacy/Acciai/Acciai.csproj | Calcestruzzo, STA.Materiali |
| Calcestruzzo | legacy/Calcestruzzo/Calcestruzzo.csproj | STA.Materiali |
| Utilità | legacy/FinestraTest/Utilità.csproj | controlloWPFVentoBase, STA.Carichi.Neve, STA.Carichi.Sisma, STA.Prefabbricato.TraveIsostatica, Vento, VisualizzaMateriali |
| STA.Geometria | legacy/Geometria/STA.Geometria.csproj | STA.Common, STA.Geometria.Core |
| STA.Modules.CaricoVento | legacy/ModuloLoadVento/STA.Modules.CaricoVento.csproj | controlloWPFVentoBase, STA.Interfaccia.Common, Vento |
| STA.Carichi.Neve | legacy/Neve/STA.Carichi.Neve.csproj | STA.Common, STA.Carichi.Neve.Core |
| Prefabbricato | legacy/Prefabbricato/Prefabbricato.csproj | STA.Geometria.Masse |
| STA.Common | legacy/STA.Common/STA.Common.csproj | STA.Common.Core |
| STA.Grafica | legacy/STA.Grafica/STA.Grafica.csproj | — |
| STA.Interfaccia.Common | legacy/STA.Interfaccia.Common/STA.Interfaccia.Common.csproj | — |
| STA.Interfaccia | legacy/STA.Interfaccia/STA.Interfaccia.csproj | STA.Modules.CaricoVento, STA.Interfaccia.Common, STA.Modules.CaricoNeve, STA.Modules.CaricoSisma |
| STA.Modules.CaricoNeve | legacy/STA.Modules.CaricoNeve/STA.Modules.CaricoNeve.csproj | STA.Carichi.Neve, STA.Common, STA.Interfaccia.Common |
| STA.Modules.CaricoSisma | legacy/STA.Modules.CaricoSisma/STA.Modules.CaricoSisma.csproj | STA.Carichi.Sisma, STA.Common, STA.Interfaccia.Common |
| STA.Modules.GestioneCarichi | legacy/STA.Modules.GestioneCarichi/STA.Modules.GestioneCarichi.csproj | — |
| STA.Prefabbricato.Interfaccia.Moduli.GestioneSezione | legacy/STA.Prefabbricato.Interfaccia.Moduli.GestioneSezione/STA.Prefabbricato.Interfaccia.Moduli.GestioneSezione.csproj | STA.Geometria.Masse, STA.Common, STA.Interfaccia.Common |
| STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica | legacy/STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica/STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica.csproj | STA.Common, STA.Interfaccia.Common, STA.Prefabbricato.Trefolatura, STA.Prefabbricato.TraveIsostatica |
| STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura | legacy/STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura/STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.csproj | STA.Common, STA.Interfaccia.Common, STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica, STA.Prefabbricato.Trefolatura, STA.Prefabbricato.TraveIsostatica |
| STA.Prefabbricato.Trave.Interfaccia | legacy/STA.Prefabbricato.Trave.Interfaccia/STA.Prefabbricato.Trave.Interfaccia.csproj | STA.Geometria.Masse, STA.Common, STA.Interfaccia.Common, STA.Prefabbricato.Trefolatura, STA.Prefabbricato.TraveIsostatica |
| STA.Prefabbricato.Trefolatura | legacy/STA.Prefabbricato.Trefolatura/STA.Prefabbricato.Trefolatura.csproj | STA.Geometria, STA.Common, STA.Prefabbricato.Trefolatura.Core |
| STA_Dimensionamento Plinti | legacy/STA_Dimensionamento Plinti/STA_Dimensionamento Plinti.csproj | STA.Geometria |
| STA_Prefabbricato | legacy/STA_Prefabbricato/STA_Prefabbricato.csproj | — |
| STA.Geometria.Masse | legacy/SezioneSemplice/STA.Geometria.Masse.csproj | STA.Geometria, STA.Common, STA.Geometria.Masse.Core |
| STA.Carichi.Sisma | legacy/Sismica/STA.Carichi.Sisma.csproj | STA.Common, STA.Grafica, STA.Carichi.Sisma.Core |
| Template_01 | legacy/Template_01/Template_01.csproj | STA_Dimensionamento Plinti |
| STA.Prefabbricato.TraveIsostatica | legacy/Trave Prefabbricata/STA.Prefabbricato.TraveIsostatica.csproj | STA.Geometria, STA.Geometria.Masse, STA.Common, STA.Prefabbricato.Trefolatura, STA.Prefabbricato.Trave.Core |
| Vento | legacy/Vento/Vento.csproj | STA.Carichi.Vento.Core |
| VisualizzaMateriali | legacy/VisualizzaMateriali/VisualizzaMateriali.csproj | Acciai, Calcestruzzo |
| controlloWPFVentoBase | legacy/controlloWPFVentoBase/controlloWPFVentoBase.csproj | Vento |
| STA.Calcolo.Application | src/STA.Calcolo.Application/STA.Calcolo.Application.csproj | STA.Materiali, STA.Carichi.Neve.Core, STA.Carichi.Vento.Core, STA.Carichi.Sisma.Core, STA.Geometria.Masse.Core, STA.Prefabbricato.Trefolatura.Core, STA.Prefabbricato.Trave.Core, STA.Calcolo.IO, STA.Plinti.Storici.Core |
| STA.Calcolo.Contracts | src/STA.Calcolo.Contracts/STA.Calcolo.Contracts.csproj | — |
| STA.Calcolo.IO | src/STA.Calcolo.IO/STA.Calcolo.IO.csproj | STA.Plinti.Core |
| STA.Calcolo.Reporting | src/STA.Calcolo.Reporting/STA.Calcolo.Reporting.csproj | STA.Muratura.Core |
| STA.Calcolo.Updates.Core | src/STA.Calcolo.Updates.Core/STA.Calcolo.Updates.Core.csproj | — |
| STA.Carichi.Neve.Core | src/STA.Carichi.Neve.Core/STA.Carichi.Neve.Core.csproj | STA.Common.Core |
| STA.Carichi.Sisma.Core | src/STA.Carichi.Sisma.Core/STA.Carichi.Sisma.Core.csproj | — |
| STA.Carichi.Vento.Core | src/STA.Carichi.Vento.Core/STA.Carichi.Vento.Core.csproj | — |
| STA.Common.Core | src/STA.Common.Core/STA.Common.Core.csproj | — |
| STA.Geometria.Core | src/STA.Geometria.Core/STA.Geometria.Core.csproj | — |
| STA.Geometria.Masse.Core | src/STA.Geometria.Masse.Core/STA.Geometria.Masse.Core.csproj | STA.Geometria.Core |
| STA.Integrations.Midas | src/STA.Integrations.Midas/STA.Integrations.Midas.csproj | STA.Calcolo.Contracts |
| STA.Materiali | src/STA.Materiali/STA.Materiali.csproj | — |
| STA.Muratura.Core | src/STA.Muratura.Core/STA.Muratura.Core.csproj | STA.Materiali, STA.Profilati.Core |
| STA.Plinti.Core | src/STA.Plinti.Core/STA.Plinti.Core.csproj | STA.Calcolo.Contracts |
| STA.Plinti.Storici.Core | src/STA.Plinti.Storici.Core/STA.Plinti.Storici.Core.csproj | STA.Geometria.Core |
| STA.Prefabbricato.Trave.Core | src/STA.Prefabbricato.Trave.Core/STA.Prefabbricato.Trave.Core.csproj | STA.Geometria.Core, STA.Geometria.Masse.Core, STA.Prefabbricato.Trefolatura.Core |
| STA.Prefabbricato.Trefolatura.Core | src/STA.Prefabbricato.Trefolatura.Core/STA.Prefabbricato.Trefolatura.Core.csproj | STA.Geometria.Core |
| STA.Profilati.Core | src/STA.Profilati.Core/STA.Profilati.Core.csproj | — |
| STA.Calcolo.Tests | tests/STA.Calcolo.Tests/STA.Calcolo.Tests.csproj | STA.Calcolo.IO, STA.Integrations.Midas, STA.Geometria.Masse.Core, STA.Carichi.Sisma.Core, STA.Materiali, STA.Calcolo.Application |
| STA.Desktop.Smoke | tests/STA.Desktop.Smoke/STA.Desktop.Smoke.csproj | STA.Calcolo.Desktop |
| STA.Muratura.Tests | tests/STA.Muratura.Tests/STA.Muratura.Tests.csproj | STA.Muratura.Core |
| STA.Reporting.Tests | tests/STA.Reporting.Tests/STA.Reporting.Tests.csproj | STA.Calcolo.Reporting |
| STA.Updates.Tests | tests/STA.Updates.Tests/STA.Updates.Tests.csproj | STA.Calcolo.Updates.Core |
