# Componenti MVVM di TA-Calcolo

Ogni componente ha Model, ViewModel e View in file distinti. Le View sono controlli WPF riutilizzabili; la finestra del confronto li compone senza logica di calcolo nel code-behind.

| Componente | Model | ViewModel | View |
| --- | --- | --- | --- |
| Materiale murario | `STA.Materiali/MasonryMaterialState.cs`; definizioni in `MasonryMaterial.cs` | `MasonryMaterialViewModel.cs` | `MasonryMaterialView.xaml` |
| Maschio murario | `STA.Muratura.Core/MasonryPier.cs` | `MasonryPierViewModel.cs` | `MasonryPierView.xaml` |
| Profilo metallico | `STA.Profilati.Core/SteelProfile.cs` | `SteelProfileViewModel.cs` | `SteelProfileView.xaml` |
| Cerchiatura | `STA.Muratura.Core/FrameInput.cs` | `FrameViewModel.cs` | `FrameView.xaml` |

I ViewModel si trovano in `apps/STA.Calcolo.Desktop/ViewModels`, le View in `apps/STA.Calcolo.Desktop/Views`. Tutti i Model sono nelle librerie portabili, senza riferimenti a WPF.

`MasonryPierViewModel` contiene il ViewModel del proprio materiale; `FrameViewModel` contiene quello del profilo e gestisce ricerca e selezione delle proposte. Il coordinatore `MasonryViewModel` gestisce gli stati prima/dopo, i file e il confronto complessivo. Le dipendenze tra componenti passano dai modelli e da funzioni esplicite; nessun ViewModel conosce controlli o finestre WPF.

Il vecchio formato JSON conserva le proprietà del materiale sul maschio. `MasonryMaterialMapping` converte questo formato nel Model del materiale indipendente e viceversa: i progetti del sito e delle versioni precedenti restano compatibili, senza duplicare una seconda fonte di dati editabili.

`MasonryFieldsViewModel` è un supporto interno per i campi e la conversione dei numeri; non sostituisce i ViewModel dedicati ai quattro componenti. I motori `MasonryCalculator` e `FrameCalculator` rimangono separati dall'interfaccia.
