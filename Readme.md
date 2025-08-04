# MyAutoBadge

**MyAutoBadge** est une application WPF légère permettant d’automatiser des actions de badgeage web, avec interface native et configuration flexible

## Fonctionnalités

- Automatisation du badgeage quotidien à une heure définie
- Possibilité de lancer un badgeage manuel
- Blocage configurable pendant les week-ends ou jours fériés
- Interface utilisateur native en WPF
- Configuration simple via `.env` et `appsettings.json`
- Journaux d'exécution dans `/Logs/`

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/fr-fr/download)

  `.NET 9` n'est **pas encore supporté pour les applications WPF** dans un environnement stable.  
  Pour éviter les erreurs liées à des types non trouvés (`Application`, `Window`, etc.), un fichier `global.json` verrouille la version du SDK :

  ```json
  {
    "sdk": {
      "version": "8.0.412"
    }
  }
  ```

- [Playwright CLI](https://playwright.dev/dotnet/docs/intro)

  Les navigateurs nécessaires sont automatiquement installés avec :

  ```powershell
  powershell -ExecutionPolicy Bypass -File "bin\Debug\net8.0-windows\playwright.ps1" install
  ```

- **Windows uniquement** (application WPF native)

## Démarrage rapide

1. Cloner le projet :
   ```bash
   git clone https://github.com/AntoineBendafiSchulmann/MyAutoBadge.git
   cd MyAutoBadge
   ```

## Lancer l'application

```bash
dotnet build
dotnet run
```

## Structure

- `MainWindow.xaml` : Interface utilisateur principale
- `Services/` : Contient la logique de badgeage (`BadgeService`, `WebAutomationService`, etc.)
- `Models/` : Classes de configuration liées à `appsettings.json`
- `Helpers/` : Calcul des jours fériés variables (ex. Pâques)

## Sécurité et confidentialité

- Les informations sensibles comme l’URL de badgeage sont placées dans un fichier `.env` (non versionné).
- Aucun élément confidentiel ou spécifique à une entreprise / un service particulier n’est contenu dans ce projet.
- Aucune dépendance ou configuration n’est codée en dur dans ce projet
