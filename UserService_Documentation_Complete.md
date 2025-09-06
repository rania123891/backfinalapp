# 👥 USERSERVICE - DOCUMENTATION COMPLÈTE

## 🎯 Table des Matières
1. [Architecture Globale](#architecture-globale)
2. [Structure Détaillée](#structure-détaillée)
3. [Analyse des Composants](#analyse-des-composants)
4. [Questions & Réponses Techniques](#questions--réponses-techniques)
5. [Questions Supplémentaires](#questions-supplémentaires)

---

## 🏗️ Architecture Globale

### Vue d'Ensemble
Le UserService suit une **architecture Clean Architecture** en 4 couches distinctes :

```
UserService.Api/          🌐 API (Controllers, Endpoints)
├── UserService.Domain/   🧠 MÉTIER (Models, Commands, Queries)
├── UserService.Data/     💾 DONNÉES (DbContext, Repositories)
└── UserService.Infra/    🔧 INFRASTRUCTURE (Services, Email, Files)
```

### Ports et Configuration
- **Port HTTPS :** 7155
- **Port HTTP :** 5182
- **Base de données :** SQL Server (UserDb)
- **Framework :** .NET 8

---

## 📊 Structure Détaillée

### 🌐 UserService.Api
**Responsabilités :** Point d'entrée HTTP, contrôleurs REST

**Fichiers principaux :**
- `Controllers/UtilisateurController.cs` - 8 endpoints REST
- `Program.cs` - Configuration DI, JWT, CORS
- `Filters/FormFileSchemaFilter.cs` - Support Swagger pour upload

### 🧠 UserService.Domain
**Responsabilités :** Logique métier pure, indépendante de la technologie

**Structure :**
- **Models/** - Entités métier
  - `Utilisateur.cs` - Modèle principal utilisateur
  - `EmailVerification.cs` - Tokens de vérification
- **Commands/** - Actions d'écriture
  - `CreateUtilisateurCommand.cs` - Création utilisateur
- **Queries/** - Actions de lecture
  - `GetAllUtilisateursQuery.cs` - Récupération tous utilisateurs
  - `GetUtilisateurByIdQuery.cs` - Récupération par ID
- **DTOs/** - Objets de transfert
  - `RegisterDto.cs` - Données d'inscription
  - `LoginDto.cs` - Données de connexion
  - `UtilisateurDto.cs` - Données utilisateur (sortie)
- **Handlers/** - Logique métier CQRS
  - `CreateUtilisateurCommandHandler.cs` - Traitement création
  - `GetAllUtilisateursQueryHandler.cs` - Traitement récupération
- **Interfaces/** - Contrats
  - `IUtilisateurRepository.cs` - Contrat repository
  - `IAuthService.cs` - Contrat authentification

### 💾 UserService.Data
**Responsabilités :** Accès aux données, persistance

**Structure :**
- **Context/** - Configuration Entity Framework
  - `UserDbContext.cs` - DbContext principal
- **Repositories/** - Implémentation accès données
  - `UtilisateurRepository.cs` - CRUD utilisateurs
- **Migrations/** - Évolution schéma base (17 fichiers)

### 🔧 UserService.Infra
**Responsabilités :** Services externes, implémentations techniques

**Services :**
- `AuthService.cs` - JWT, BCrypt, Claims
- `EmailService.cs` - SendGrid (désactivé actuellement)

---

## 🔍 Analyse des Composants

### Modèle Utilisateur
```csharp
public class Utilisateur
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public RoleUtilisateur Role { get; set; } = RoleUtilisateur.User;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public string? ProfilePhotoUrl { get; set; }
}

public enum RoleUtilisateur
{
    Admin,
    User
}
```

### Fonctionnalités Principales

#### ✅ Inscription (Register)
- **Endpoint :** `POST /api/utilisateur/register`
- **Validation :** Email, mot de passe (6+ caractères), confirmation
- **Sécurité :** Hachage BCrypt du mot de passe
- **Rôles :** Admin ou User
- **Champs :** Email, Nom, Prénom, Rôle

#### 🔐 Connexion (Login)
- **Endpoint :** `POST /api/utilisateur/login`
- **Authentification :** Email + mot de passe
- **Token :** JWT avec claims (id, email, role, nom, prénom)
- **Durée :** 2 heures d'expiration

#### 👤 Gestion Utilisateurs
- **Récupération :** Par ID, par email, tous les utilisateurs
- **Autorisation :** Admin peut voir tous, User seulement soi-même
- **Photos :** Upload et gestion des photos de profil

### Architecture CQRS avec MediatR

#### Commands (Écriture)
```csharp
public class CreateUtilisateurCommand : IRequest<Utilisateur>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
}
```

#### Queries (Lecture)
```csharp
public class GetAllUtilisateursQuery : IRequest<IEnumerable<Utilisateur>>
{
    // Pas de propriétés - récupération simple
}
```

#### Handlers
```csharp
public class CreateUtilisateurCommandHandler : IRequestHandler<CreateUtilisateurCommand, Utilisateur>
{
    // Logique de création avec validation et hachage mot de passe
}
```

### Sécurité

#### Hachage des Mots de Passe
```csharp
public string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}
```

#### JWT Token
**Claims inclus :**
- ID utilisateur (NameIdentifier)
- Email
- Rôle (Admin/User)
- Nom et Prénom
- **Expiration :** 2 heures

#### Autorisation
```csharp
[Authorize(Policy = "AdminOnly")]  // Seuls les Admin
[Authorize(Roles = "Admin,User")]  // Admin OU User
```

### Base de Données

#### Tables
1. **Utilisateurs**
   - id, email, passwordHash, nom, prenom, role, dateCreation, profilePhotoUrl
   - Index unique sur email
2. **EmailVerifications** (actuellement inutilisée)
   - id, email, token, createdAt

---

## 🎯 Questions & Réponses Techniques

### 🏛️ Architecture & Patterns

**Q1. Quelles sont les 4 couches de l'architecture UserService et leur rôle ?**

**R1 :**
- **API Layer :** Point d'entrée HTTP, contrôleurs REST
- **Domain Layer :** Logique métier pure, indépendante de la technologie
- **Data Layer :** Accès aux données, persistance
- **Infrastructure Layer :** Services externes, implémentations techniques

**Q2. Qu'est-ce que le pattern CQRS et comment est-il implémenté avec MediatR ?**

**R2 :** CQRS = Command Query Responsibility Segregation
- **Commands** : Opérations d'écriture (CreateUtilisateurCommand)
- **Queries** : Opérations de lecture (GetAllUtilisateursQuery)
- **MediatR** : Médiateur entre Controller et Handler, découplage complet

**Q3. Quelle est la différence entre une Command et une Query ?**

**R3 :**
| Command | Query |
|---------|-------|
| Modifie les données | Lit les données |
| Retourne peu/pas de données | Retourne beaucoup de données |
| Side effects | Pas de side effects |

**Q4. Pourquoi utilise-t-on le pattern Repository ?**

**R4 :**
- **Abstraction :** Sépare logique métier de l'accès données
- **Testabilité :** Mock facile pour tests unitaires
- **Flexibilité :** Changement de base sans impact métier
- **Centralisation :** Toutes les requêtes dans un endroit

### 🔐 Sécurité

**Q5. Comment les mots de passe sont-ils sécurisés dans l'application ?**

**R5 :**
- **Hachage BCrypt** avec salt automatique
- **Jamais stocké en clair** dans la base
- **Résistant aux attaques** rainbow table
- **Coût computationnel** élevé contre brute force

**Q6. Quelles informations sont stockées dans le JWT token ?**

**R6 :**
- ID utilisateur (NameIdentifier)
- Email
- Rôle (Admin/User)
- Nom et Prénom

**Q7. Quelle est la durée de vie d'un JWT token ?**

**R7 :** 2 heures (`DateTime.UtcNow.AddHours(2)`)

**Q8. Comment fonctionne l'autorisation basée sur les rôles ?**

**R8 :**
- **Décoration endpoints :** `[Authorize(Policy = "AdminOnly")]`
- **Vérification code :** `User.FindFirst(ClaimTypes.Role)?.Value`
- **Claims-based :** Informations dans le token JWT

### 💾 Base de Données

**Q9. Combien de tables contient la base UserDb et lesquelles ?**

**R9 :** 2 tables
- **Utilisateurs** : Données principales
- **EmailVerifications** : Tokens vérification (inutilisée)

**Q10. Pourquoi y a-t-il un index unique sur l'email ?**

**R10 :**
- **Unicité :** Empêche doublons d'email
- **Performance :** Recherche rapide
- **Intégrité :** Contrainte niveau base

**Q11. À quoi sert la table EmailVerifications ?**

**R11 :** (Actuellement désactivée)
- Stocker tokens de vérification d'email
- Sécuriser l'inscription
- Expiration des tokens (24h)

**Q12. Que sont les migrations Entity Framework ?**

**R12 :**
- **Évolution du schéma** de base
- **Versioning** de la structure
- **Up/Down :** Appliquer ou annuler changements
- **17 fichiers** dans votre projet

### 🌐 API & Endpoints

**Q13. Combien d'endpoints expose le UtilisateurController ?**

**R13 :** 8 endpoints principaux
1. `POST /api/utilisateur/register`
2. `POST /api/utilisateur/login`
3. `GET /api/utilisateur/{id}`
4. `GET /api/utilisateur/by-email/{email}`
5. `GET /api/utilisateur` (tous)
6. `GET /api/utilisateur/for-selection`
7. `POST /api/utilisateur/upload-photo`
8. `GET /api/utilisateur/profile-photo/{fileName}`

**Q14. Quelle est la différence entre [FromBody] et [FromForm] ?**

**R14 :**
| [FromBody] | [FromForm] |
|------------|------------|
| JSON dans le body | Form data (multipart) |
| application/json | multipart/form-data |
| Données complexes | Fichiers + données |

**Q15. Comment fonctionne l'upload de photos de profil ?**

**R15 :**
1. **Validation :** Type, taille du fichier
2. **Nom unique :** Guid + extension
3. **Stockage :** Dossier wwwroot/profile-photos
4. **URL :** Sauvegardée dans ProfilePhotoUrl
5. **Retour :** URL d'accès à l'image

**Q16. Qu'est-ce que Swagger et à quoi sert FormFileSchemaFilter ?**

**R16 :**
- **Swagger :** Documentation API automatique + interface test
- **FormFileSchemaFilter :** Corrige l'affichage des uploads dans Swagger

### 🔧 Configuration & Services

**Q17. Comment l'injection de dépendances est-elle configurée ?**

**R17 :**
```csharp
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddDbContext<UserDbContext>();
builder.Services.AddMediatR();
```

**Q18. Qu'est-ce que CORS et pourquoi est-il configuré ?**

**R18 :**
- **CORS :** Cross-Origin Resource Sharing
- **Problème :** Navigateur bloque requêtes cross-origin
- **Solution :** Autoriser explicitement certaines origines

**Q19. Comment MediatR est-il enregistré dans le container DI ?**

**R19 :**
```csharp
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CreateUtilisateurCommand>());
```

**Q20. Quelle est la différence entre Scoped, Transient et Singleton ?**

**R20 :**
| Lifetime | Durée de vie | Usage |
|----------|--------------|-------|
| Singleton | Toute l'application | Configuration, Cache |
| Scoped | Une requête HTTP | DbContext, Repositories |
| Transient | Chaque injection | Services légers |

---

## 🎯 Questions Supplémentaires Importantes

### Questions Avancées

**Q21. Comment fonctionne Entity Framework Code First ?**
**R21 :** Models → Migrations → Base de données, Convention over Configuration

**Q22. Qu'est-ce que l'AsNoTracking() dans les requêtes ?**
**R22 :** Performance améliorée, pas de suivi des changements, lecture seule

**Q23. Comment fonctionne la validation avec Data Annotations ?**
**R23 :** Validation automatique dans Controller, `[Required]`, `[EmailAddress]`

**Q24. Qu'est-ce que le pattern Unit of Work ?**
**R24 :** Transaction globale sur plusieurs repositories, cohérence des données

**Q25. Comment gérer les erreurs globalement ?**
**R25 :** Middleware d'exception, Try-catch dans Controllers, Logging structuré

**Q26. Qu'est-ce que AutoMapper et pourquoi l'utiliser ?**
**R26 :** Mapping automatique Entity → DTO, réduction code boilerplate

**Q27. Comment implémenter la pagination ?**
**R27 :** `Skip((page - 1) * size).Take(size)`

**Q28. Qu'est-ce que les Specifications Pattern ?**
**R28 :** Requêtes complexes réutilisables, composition de critères

**Q29. Comment implémenter le Soft Delete ?**
**R29 :** Propriété `IsDeleted`, Global Query Filter

**Q30. Comment gérer les connexions multiples bases ?**
**R30 :** DbContext séparés, Connection strings différentes

---

## 📋 Endpoints Détaillés

### POST /api/utilisateur/register
**Description :** Inscription d'un nouvel utilisateur
**Body :**
```json
{
  "email": "user@example.com",
  "password": "password123",
  "confirmPassword": "password123",
  "nom": "Dupont",
  "prenom": "Jean",
  "role": "User"
}
```
**Réponse :**
```json
{
  "message": "Inscription réussie ! Vous pouvez maintenant vous connecter.",
  "success": true
}
```

### POST /api/utilisateur/login
**Description :** Connexion utilisateur
**Body :**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```
**Réponse :**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "id": 1,
  "nom": "Dupont",
  "prenom": "Jean",
  "email": "user@example.com",
  "role": "User"
}
```

---

## 🎯 Évaluation des Connaissances

**Sur ces 30 questions, votre niveau :**
- **25-30** : Expert UserService ⭐⭐⭐
- **20-24** : Très bon niveau ⭐⭐
- **15-19** : Bon niveau ⭐
- **<15** : À approfondir 📚

---

*Document généré automatiquement - UserService Documentation Complète*
*Date : $(Get-Date)*




