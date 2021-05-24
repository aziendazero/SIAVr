# OnVac - Anagrac Vaccinale

## Author

ONIT Group Srl - supportosanita@onit.it

## Introduzione

Applicativo per la gestione dell'Anagrafe Vaccinale. 
E' possibile gestire schede anagrafiche dei pazienti, chiamate attive in base al calendario vaccinale (sia pediatrico che adulti), appuntamenti sia singoli che massivi, campagne vaccinali.
Sono incluse funzionalità di magazzino e gestione giacenze, reportistica e statistiche varie. 
E' prevista la possibilità di integrarsi ad un'anagrafe assistiti aziendale o regionale, al Fascicolo Sanitario Elettronico per l'invio dei certificati vaccinali

## Tecnlogie 

Questo software è web-based e utilizza diversi elementi Bootstrap insieme ad altre tecnologie:
- ASP.NET Framework (3.5, 4.0, 4.5.2)
- Visual Basic
- C#
- DBMS Oracle v. 19c

## Struttura del repository

Il repository è strutturato come segue: 
- OnVac.Biz: Logiche di business
- OnVac.Common.Utility: Funzioni di utilità
- OnVac.DAL: Contiene i sorgenti del DATA ACCESS LAYER
- OnVac.ObjectModel: Contiene le classi che rispecchiano le entità del DB
- OnVac.Queries: Queries
- OnVac.Report: Contiene alcuni tipi di stampe
- OnVac.Settings: Parametri applicativi
- OnVac.Wcf: Servizio utilizzato internamente dall'applicativo Web
- OnVac.Web: Applicativo Web di gestione delle vaccinazioni

## Dipendenze

````
	open:
        - name: ASP.NET Framework
          version: 4.5.2
		- name: Bootstrap
		  version: 4
	proprietary:
        - name: Oracle
          version: 19
````

## DB

Il DB utilizzato è Oracle versione 19c

## API utilizzate

## Contatti

*ONIT Group Srl
Via dell'Arrigoni, 308
47522 - Cesena (FC) - Italy
Tel: +39 0547 313110
Help desk sanità territoriale Tel: +39 0547 1760103 - Mail: supportosanita@onit.it

## Versione del repository

Versione 4.24.00

## Copyleft and License

This software use an AGPL-3.0 License.