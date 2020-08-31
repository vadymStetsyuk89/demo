using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class GetTeamRequestsResponse : IResponse, ICollection<TeamAppointmentStatusDTO> {

        private ICollection<TeamAppointmentStatusDTO> _collection = new List<TeamAppointmentStatusDTO>();

        public int Count => _collection.Count;

        public bool IsReadOnly => _collection.IsReadOnly;

        public void Add(TeamAppointmentStatusDTO item) => _collection.Add(item);

        public void Clear() => _collection.Clear();

        public bool Contains(TeamAppointmentStatusDTO item) => _collection.Contains(item);

        public void CopyTo(TeamAppointmentStatusDTO[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        public IEnumerator<TeamAppointmentStatusDTO> GetEnumerator() => _collection.GetEnumerator();

        public bool Remove(TeamAppointmentStatusDTO item) => _collection.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }    
}
