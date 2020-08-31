using PeakMVP.Models.Rests.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Family {
    public interface IFamilyService {

        Task<FamilyDTO> GetFamilyAsync(CancellationTokenSource cancellationTokenSource);
    }
}
