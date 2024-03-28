# decaf-orm.sqlite
This package is the SQLite implementation for [decaf-orm](https://www.nuget.org/packages/decaf-orm/), it provides all the method implementations to build queries specifically for SQLite databases.

The package has two dependencies:
1. [`decaf-orm`](https://www.nuget.org/packages/decaf-orm/)
2. [`decaf-orm.db`](https://www.nuget.org/packages/decaf-orm.db/)

Both of these packages are .Net Standard 2.0 only, and thus are usable across almost all .Net Core, .Net Framework and other .Net Standard projects.

To use Npgsql:
```csharp
services.AddDecaf(o => o.UseSqlite(b => b.WithConnectionDetails(...)));
```  

Ideally your `IConnectionDetails` would be injected already and you can re-use it, or coming from Configuration.
At present decaf does not support 'delayed resolution' of the `IConnectionDetails` and it will need to be passed in at the time you setup decaf.

### Status:
**Build**  
[![Build](https://github.com/daniel-buchanan/decaf-orm/actions/workflows/sonar.yml/badge.svg)](https://github.com/daniel-buchanan/decaf-orm/actions/workflows/sonar.yml)
[![CodeQL](https://github.com/daniel-buchanan/decaf-orm/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/daniel-buchanan/decaf-orm/actions/workflows/codeql-analysis.yml)  
**Sonar Cloud**  
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=coverage)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)  
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=bugs)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=daniel-buchanan_pdq&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=daniel-buchanan_pdq)

## Documentation
Documentation can be found here: [README](https://github.com/daniel-buchanan/decaf-orm/blob/main/README.md).