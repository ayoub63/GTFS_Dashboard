# GTFS Dashboard

Ein Full-Stack-Projekt zur automatisierten Verarbeitung, Visualisierung und Analyse von ÖPNV-Fahrplandaten im GTFS (General Transit Feed Specification) Format.

Das Projekt besteht aus einem  .NET 8 C# Backend, das Millionen von Fahrplandatensätzen effizient in eine SQLite-Datenbank importiert, und einem Python Streamlit Frontend zur Datenexploration.

## Features

* Automatisierter GTFS Import: Liest `.txt` GTFS-Dateien (Stops, Routes, Trips, StopTimes, Calendar) über `CsvHelper` ein und speichert sie relational über Entity Framework Core.
* Map Explorer: Geografische Suche nach Haltestellen mittels Bounding-Box (Koordinaten) und interaktiver 3D-Kartendarstellung (PyDeck).
* Station Hierarchy: Visualisierung von übergeordneten Stationen (Parent Stations) und deren zugehörigen Gleisen/Bahnsteigen (Child Stops).
* Advanced Analytics: Top Haltestellen nach Verkehrsvolumen.
  * Stoßzeiten-Analyse (Peak Hours).
  * Filterbar nach spezifischem Datum oder Wochentagen unter Berücksichtigung von Feiertagen/Ausnahmen (`calendar.txt` & `calendar_dates.txt`).
* REST API: Dokumentierte Schnittstelle mittels Swagger/OpenAPI.

## Tech Stack

Backend:
* C# / .NET 8.0 Web API
* Entity Framework Core (ORM)
* SQLite (Datenbank)
* CsvHelper (Datenimport)
* Swagger (API Dokumentation)

Frontend:
* Python 3.x
* Streamlit (Web Framework)
* Pandas (Datenverarbeitung)
* PyDeck (Geografische Visualisierung)
* Plotly Express (Diagramme)
* Requests (API Kommunikation)

---

## Setup & Start

### 1. Voraussetzungen
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) muss installiert sein.
* [Python 3.9+](https://www.python.org/downloads/) muss installiert sein.

### 2. GTFS Daten vorbereiten
1. Lade einen GTFS-Datensatz deiner Wahl herunter (meist als `.zip` Datei).
2. Erstelle im Ordner `backend` einen neuen Ordner namens `data`.
3. Entpacke die GTFS `.txt` Dateien (z.B. `stops.txt`, `routes.txt`, `stop_times.txt`, etc.) in den Pfad `backend/data/`.

### 3. Backend starten
Das Backend importiert die GTFS-Daten beim ersten Start automatisch in eine SQLite-Datenbank (`gtfs.db`). **Achtung: Der erste Importvorgang kann je nach Datengröße einige Minuten dauern.**

Öffne ein Terminal und führe folgende Befehle aus:
```bash
cd backend
dotnet run