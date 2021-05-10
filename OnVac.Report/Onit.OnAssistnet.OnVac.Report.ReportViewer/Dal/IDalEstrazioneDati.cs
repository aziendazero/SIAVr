using System;
using System.Collections.Generic;

namespace Onit.OnAssistnet.OnVac.Report.ReportViewer.Dal
{
    public interface IDalEstrazioneDati : IDisposable
    {
        IEnumerable<Reports.EstrazioneControlliScuole.DettagliScuole> GetDettagliEstrazioneScuole(long idProcesso);
        IEnumerable<Reports.EstrazioneControlliCentri.DettagliCentri> GetDettagliEstrazioneCentri(long idProcesso);
        IEnumerable<Reports.EstrazioneElencoConsulenze.DettaglioConsulenza> GetDettagliEstrazioneConsulenze(string filtroReport);
    }
}
