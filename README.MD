# FoxyBlueLight

![FoxyBlueLight Logo](https://github.com/Tiger-Foxx/BlueLightFox/raw/main/FoxyBlueLight/Resources/logo.png)

## Protection avancée des yeux contre la lumière bleue pour Windows

[![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)](https://github.com/Tiger-Foxx/BlueLightFox/releases)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)

**FoxyBlueLight** est une application de filtrage de lumière bleue sophistiquée pour Windows, conçue pour réduire la fatigue oculaire lors de l'utilisation prolongée d'écrans. L'application propose plusieurs modes de filtrage, des options de personnalisation avancées et une interface utilisateur moderne développée avec WPF et MaterialDesign.

![Screenshot](https://github.com/Tiger-Foxx/BlueLightFox/raw/main/Screenshots/main-interface.png)

## 📋 Table des matières

- [Fonctionnalités](#fonctionnalités)
- [Installation](#installation)
- [Utilisation](#utilisation)
- [Architecture technique](#architecture-technique)
- [Implémentation des filtres](#implémentation-des-filtres)
- [Personnalisation](#personnalisation)
- [Performances et considérations](#performances-et-considérations)
- [Dépendances](#dépendances)
- [Contribution](#contribution)
- [Licence](#licence)
- [Remerciements](#remerciements)

## ✨ Fonctionnalités

### Filtres avancés de lumière bleue
- **Multi-mode**: Température, Chaud, Très chaud, Sépia, Niveaux de gris, Rouge nuit et Mode personnalisé
- **Ajustement de la température de couleur**: De 1900K à 6500K pour une adaptation précise
- **Personnalisation RGB**: Contrôle exact des composantes Rouge, Vert et Bleu en mode personnalisé
- **Sélecteurs de couleurs prédéfinies**: Choix rapide parmi des teintes éprouvées pour le confort visuel

### Techniques de filtrage multiples
- **Mode Gamma**: Ajustement traditionnel des courbes gamma pour une modification précise des couleurs
- **Mode Superposition douce**: Filtre transparent qui préserve mieux les couleurs d'origine (similaire à Windows Night Light)
- **Mode Préservation de luminosité**: Réduction de la lumière bleue tout en maintenant la luminosité perçue

### Interface utilisateur
- **Design moderne Material Design**: Interface élégante et intuitive
- **Options de personnalisation avancées**: Intensité, luminosité et transparence ajustables
- **Fenêtre redimensionnable**: S'adapte à vos préférences d'affichage
- **Icône dans la barre des tâches**: Accès rapide aux fonctions essentielles

### Programmation et automatisation
- **Activation programmée**: Configuration des heures d'activation et de désactivation automatiques
- **Sauvegarde des paramètres**: Restauration de vos préférences au redémarrage

## 💻 Installation

### Prérequis
- Windows 10 ou Windows 11
- Droits d'administrateur pour l'installation (nécessaire pour modifier les paramètres d'affichage)

### Méthodes d'installation

#### Installeur standard
1. Téléchargez le dernier installateur depuis la [page des versions](https://github.com/Tiger-Foxx/BlueLightFox/releases)
2. Exécutez `FoxyBlueLightSetup.exe`
3. Suivez les instructions d'installation
4. Sélectionnez "Exécuter au démarrage" si vous souhaitez que l'application démarre automatiquement

#### Fichier exécutable portable
1. Téléchargez `FoxyBlueLight-Portable.zip` depuis la [page des versions](https://github.com/Tiger-Foxx/BlueLightFox/releases)
2. Extrayez l'archive vers n'importe quel dossier
3. Exécutez `FoxyBlueLight.exe`

#### Compilation depuis la source
1. Clonez ce dépôt: `git clone https://github.com/Tiger-Foxx/BlueLightFox.git`
2. Ouvrez la solution dans Visual Studio 2022 ou JetBrains Rider
3. Restaurez les packages NuGet
4. Compilez le projet en mode Release
5. L'exécutable se trouve dans le dossier `bin/Release/net8.0-windows`

## 🚀 Utilisation

### Démarrage rapide
1. Lancez FoxyBlueLight depuis le menu Démarrer ou le bureau
2. Utilisez l'interrupteur principal en haut pour activer/désactiver le filtre
3. Sélectionnez un mode de filtre dans la liste déroulante
4. Ajustez l'intensité selon vos préférences

### Modes de filtrage
- **Température**: Ajustez la température de couleur de 1900K (très chaud) à 6500K (neutre)
- **Chaud**: Teinte orangée légère pour une utilisation quotidienne
- **Très chaud**: Teinte orangée plus prononcée pour le soir
- **Sépia**: Mode lecture avec teinte sépia pour réduire la fatigue oculaire lors de la lecture
- **Niveaux de gris**: Retire toutes les couleurs pour un mode monochrome
- **Rouge nuit**: Mode de vision nocturne préservant la vision dans l'obscurité
- **Personnalisé**: Définissez vos propres valeurs RGB pour un contrôle total

### Types d'atténuation
- **Gamma (Standard)**: Modifie directement les courbes gamma du système d'affichage
- **Superposition douce**: Ajoute une couche de couleur semi-transparente sur l'écran, permettant des ajustements plus subtils
- **Luminosité préservée**: Mode spécial qui réduit la lumière bleue tout en maintenant la luminosité perçue

### Planification
1. Activez l'option "Activation programmée"
2. Définissez une heure d'activation (généralement le soir)
3. Définissez une heure de désactivation (généralement le matin)
4. FoxyBlueLight s'activera et se désactivera automatiquement aux heures spécifiées

### Raccourcis et fonctionnalités rapides
- **Icône de la barre des tâches**: Clic droit pour accéder aux options rapides
- **Double-clic sur l'icône**: Affiche/masque l'interface principale
- **Bouton "Appliquer"**: Enregistre et applique immédiatement les modifications
- **Bouton "Restaurer écran"**: Désactive temporairement le filtre sans perdre vos réglages

## 🏗️ Architecture technique

FoxyBlueLight suit le pattern d'architecture MVVM (Model-View-ViewModel) pour séparer clairement la logique métier, l'interface utilisateur et la logique de présentation.

### Structure du projet
```
FoxyBlueLight/
├── Models/                     # Modèles de données
│   ├── FilterSettings.cs       # Paramètres du filtre
│   └── ColorInfo.cs            # Informations sur les couleurs prédéfinies
├── ViewModels/                 # ViewModels (logique de présentation)
│   └── MainViewModel.cs        # ViewModel principal
├── Views/                      # Vues (interfaces utilisateur)
│   ├── ModernWidget.xaml       # Interface principale
│   ├── SplashScreen.xaml       # Écran de démarrage
│   └── ColorSelector.xaml      # Sélecteur de couleurs personnalisé
├── Services/                   # Services métier
│   └── FilterService.cs        # Service de filtrage d'écran
├── Native/                     # API natives et interopération
│   └── DisplayAPI.cs           # API de gestion d'affichage Windows
├── Styles/                     # Styles et ressources XAML
│   └── ModernSliders.xaml      # Style personnalisé pour les sliders
└── Resources/                  # Ressources (images, icônes)
    └── logo.png                # Logo de l'application
```

### Composants principaux

#### Models
- **FilterSettings**: Contient tous les paramètres configurables du filtre (mode, intensité, température, multiplicateurs RGB, planification)
- **ColorInfo**: Représente une couleur prédéfinie avec son nom et sa valeur hexadécimale

#### ViewModels
- **MainViewModel**: Gère l'état de l'application, les commandes utilisateur et la coordination entre l'interface et le service de filtrage

#### Views
- **ModernWidget**: Interface principale avec tous les contrôles
- **SplashScreen**: Écran de démarrage animé
- **ColorSelector**: Contrôle personnalisé pour la sélection visuelle des couleurs

#### Services
- **FilterService**: Implémente les différentes techniques de filtrage (gamma, overlay, préservation de luminosité)

#### Native
- **DisplayAPI**: Wrapper pour les API Windows de gestion des couleurs d'affichage

## 🔬 Implémentation des filtres

### Mode Gamma (Traditionnel)

Le mode Gamma utilise l'API Win32 `SetDeviceGammaRamp` pour modifier directement les courbes gamma de la carte graphique. Cette méthode permet un ajustement précis des couleurs mais peut parfois créer des effets indésirables sur certaines nuances.

```csharp
// Extrait de code: Création d'une rampe gamma
RAMP ramp = new RAMP
{
    Red = new ushort[256],
    Green = new ushort[256],
    Blue = new ushort[256]
};

// Calcul des valeurs de la rampe
for (int i = 0; i < 256; i++)
{
    double value = i / 255.0;
    double correctedValue = Math.Pow(value, 1.0 / 2.2); // Gamma 2.2
    
    ramp.Red[i] = (ushort)(correctedValue * redMultiplier * 65535.0);
    ramp.Green[i] = (ushort)(correctedValue * greenMultiplier * 65535.0);
    ramp.Blue[i] = (ushort)(correctedValue * blueMultiplier * 65535.0);
}

// Application de la rampe
SetDeviceGammaRamp(hDC, ref ramp);
```

### Mode Superposition douce

Ce mode crée une fenêtre WPF transparente qui recouvre l'écran entier avec une couleur semi-transparente. Cette approche permet:
- Des ajustements plus subtils
- Une transition plus douce entre les états
- Une meilleure préservation des couleurs d'origine

La fenêtre est rendue "transparente aux clics" grâce à l'utilisation des styles de fenêtre avancés de Win32, permettant à l'utilisateur d'interagir normalement avec les applications sous-jacentes.

```csharp
// Extrait de code: Création d'une fenêtre d'overlay
_overlayWindow = new Window
{
    WindowStyle = WindowStyle.None,
    AllowsTransparency = true,
    Background = Brushes.Transparent,
    Topmost = true,
    WindowState = WindowState.Maximized,
    ShowInTaskbar = false,
    ResizeMode = ResizeMode.NoResize,
    IsHitTestVisible = false
};

// Rendre la fenêtre transparente aux clics
IntPtr hwnd = new WindowInteropHelper(_overlayWindow).Handle;
int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
```

### Mode Préservation de luminosité

Cette technique avancée utilise une formule spéciale pour réduire la lumière bleue tout en préservant la perception de luminosité. Elle s'inspire des algorithmes utilisés dans f.lux et d'autres applications professionnelles de filtrage de lumière bleue.

```csharp
// Extrait de code: Calcul préservant la luminosité
double avgMultiplier = (redMultiplier + greenMultiplier + blueMultiplier) / 3.0;
double brightnessAdjust = settings.Brightness * (1.0 / avgMultiplier);

for (int i = 0; i < 256; i++)
{
    double normalizedValue = i / 255.0;
    
    double redAdjusted = Math.Pow(normalizedValue, 1.0 / redMultiplier) * redMultiplier * brightnessAdjust;
    double greenAdjusted = Math.Pow(normalizedValue, 1.0 / greenMultiplier) * greenMultiplier * brightnessAdjust;
    double blueAdjusted = Math.Pow(normalizedValue, 1.0 / blueMultiplier) * blueMultiplier * brightnessAdjust;
    
    // Appliquer l'intensité comme mélange
    redAdjusted = normalizedValue * (1.0 - intensity) + redAdjusted * intensity;
    // [... autres calculs]
}
```

### Conversion de température de couleur

Le filtre de température utilise une formule complexe pour convertir une valeur en Kelvin (de 1900K à 6500K) vers des valeurs RGB équivalentes. Cette formule est basée sur des recherches en colorimétrie pour une reproduction précise des températures de couleur.

```csharp
// Extrait de code: Conversion Kelvin vers RGB
private void GetColorForTemperature(int temp, out byte r, out byte g, out byte b)
{
    double temperature = temp / 100.0;
    double red, green, blue;
    
    if (temperature <= 66)
    {
        red = 255;
        green = 99.4708025861 * Math.Log(temperature) - 161.1195681661;
        
        if (temperature <= 19)
            blue = 0;
        else
            blue = 138.5177312231 * Math.Log(temperature - 10) - 305.0447927307;
    }
    else
    {
        red = 329.698727446 * Math.Pow(temperature - 60, -0.1332047592);
        green = 288.1221695283 * Math.Pow(temperature - 60, -0.0755148492);
        blue = 255;
    }
    
    r = (byte)Math.Max(0, Math.Min(255, red));
    g = (byte)Math.Max(0, Math.Min(255, green));
    b = (byte)Math.Max(0, Math.Min(255, blue));
}
```

## 🎨 Personnalisation

### Interface utilisateur

L'interface de FoxyBlueLight est construite avec WPF et utilise la bibliothèque MaterialDesignInXAML pour un design moderne et élégant. L'interface prend en charge:

- Thème sombre pour éviter l'éblouissement
- Contrôles de matériel design (sliders, boutons, toggles)
- Animations fluides pour une expérience utilisateur agréable
- Disposition responsive qui s'adapte aux différentes tailles de fenêtre
- Sélecteurs de couleur visuels pour une sélection intuitive

### Sélecteurs de couleur personnalisés

FoxyBlueLight implémente un contrôle personnalisé `ColorSelector` qui permet aux utilisateurs de sélectionner visuellement des couleurs prédéfinies sous forme de cercles colorés:

```xml
<!-- Extrait de XAML: Sélecteur de couleur personnalisé -->
<Button Width="36" Height="36" Margin="4" 
       Background="#FFE3B9"
       Command="{Binding SelectColorCommand}" 
       CommandParameter="#FFE3B9">
    <Button.Template>
        <ControlTemplate TargetType="Button">
            <Grid>
                <Ellipse Fill="{TemplateBinding Background}"/>
                <Ellipse x:Name="highlight" Stroke="White" StrokeThickness="0"/>
            </Grid>
            <ControlTemplate.Triggers>
                <Trigger Property="IsMouseOver" Value="True">
                    <Setter TargetName="highlight" Property="StrokeThickness" Value="2"/>
                </Trigger>
            </ControlTemplate.Triggers>
        </ControlTemplate>
    </Button.Template>
</Button>
```

### Sliders personnalisés

Les sliders standard ont été remplacés par des sliders personnalisés avec un design moderne et une meilleure ergonomie:

```xml
<!-- Extrait de XAML: Style de slider personnalisé -->
<Style x:Key="ModernSliderStyle" TargetType="Slider">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Slider">
                <Grid>
                    <!-- Track background -->
                    <Border Height="4" CornerRadius="2" Background="{TemplateBinding Background}" Margin="7,0"/>
                    
                    <!-- Thumb (curseur) personnalisé avec animation au survol -->
                    <Track.Thumb>
                        <Thumb>
                            <Thumb.Template>
                                <ControlTemplate TargetType="Thumb">
                                    <Grid>
                                        <Ellipse x:Name="OuterCircle" Width="18" Height="18" Fill="#FF6700"/>
                                        <Ellipse Width="16" Height="16" Fill="White"/>
                                    </Grid>
                                    <ControlTemplate.Triggers>
                                        <Trigger Property="IsMouseOver" Value="True">
                                            <Setter TargetName="OuterCircle" Property="Width" Value="20"/>
                                            <Setter TargetName="OuterCircle" Property="Height" Value="20"/>
                                        </Trigger>
                                    </ControlTemplate.Triggers>
                                </ControlTemplate>
                            </Thumb.Template>
                        </Thumb>
                    </Track.Thumb>
                </Grid>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

## ⚙️ Performances et considérations

### Optimisations de performance

FoxyBlueLight a été optimisé pour utiliser un minimum de ressources système:

- **Faible empreinte mémoire**: L'application utilise environ 30-40 MB de RAM
- **Charge CPU minimale**: Le filtrage utilise principalement les API graphiques matérielles
- **Mode superposition efficace**: Utilise DirectComposition pour un rendu accéléré par le GPU
- **Mise en cache des paramètres**: Évite les recalculs inutiles lors des ajustements

### Gestion avancée des événements

Pour garantir une expérience utilisateur fluide, l'application gère efficacement:

- Les changements de résolution d'écran
- Les connexions/déconnexions de moniteurs
- La suspension/reprise du système
- Le changement d'utilisateur

### Compatibilité multi-écrans

FoxyBlueLight supporte les configurations multi-écrans:

```csharp
// Extrait de code: Détection et application sur tous les écrans connectés
private void ApplyToAllScreens()
{
    // Obtenir les informations de tous les écrans connectés
    foreach (var screen in System.Windows.Forms.Screen.AllScreens)
    {
        // Application spécifique à chaque écran si nécessaire
    }
    
    // Application du filtre au niveau système
    ApplyFilter(settings);
}
```

## 📦 Dépendances

FoxyBlueLight utilise les bibliothèques et technologies suivantes:

- **.NET 8.0**: Framework d'exécution et bibliothèques de base
- **WPF (Windows Presentation Foundation)**: Framework d'interface utilisateur
- **MaterialDesignInXAML**: Bibliothèque de composants UI Material Design
- **Win32 API**: Pour l'accès aux fonctions système de gestion des couleurs d'affichage

## 🤝 Contribution

Les contributions au projet sont les bienvenues! Voici comment participer:

1. **Forker le dépôt**: `https://github.com/Tiger-Foxx/BlueLightFox`
2. **Créer une branche**: `git checkout -b feature/ma-fonctionnalite`
3. **Valider vos modifications**: `git commit -am 'Ajout d'une fonctionnalité'`
4. **Pousser vers la branche**: `git push origin feature/ma-fonctionnalite`
5. **Ouvrir une Pull Request**

### Guidelines pour les contributions

- Suivre les conventions de codage C#/WPF standard
- Ajouter des commentaires pour le code complexe
- S'assurer que le code est testé sur différentes versions de Windows
- Créer des issues pour les bugs ou les suggestions de fonctionnalités

## 📄 Licence

Ce projet est sous licence MIT - voir le fichier [LICENSE](LICENSE) pour plus de détails.

## 👏 Remerciements

Ce logiciel a été inspiré par plusieurs projets similaires:

- **f.lux**: Pionnier dans le domaine du filtrage de la lumière bleue
- **Windows Night Light**: Pour les concepts d'intégration système
- **MaterialDesignInXAML**: Pour la magnifique bibliothèque d'interface utilisateur

---

Développé avec ❤️ par TheTigerFox - © 2025

---

*Note: FoxyBlueLight n'est pas un dispositif médical et ne prétend pas traiter, guérir ou diagnostiquer des problèmes de santé. Consultez un professionnel de la santé si vous avez des préoccupations concernant votre vision ou votre santé oculaire.*