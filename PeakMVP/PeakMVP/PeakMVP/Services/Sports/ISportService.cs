using PeakMVP.Models.Rests.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Sports {
    public interface ISportService {
        Task<List<SportDTO>> GetSportsAsync(CancellationToken cancellationToken);
    }
}
