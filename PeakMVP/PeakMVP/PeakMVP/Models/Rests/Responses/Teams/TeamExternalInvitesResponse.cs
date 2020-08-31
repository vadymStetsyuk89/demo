using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System.Collections;
using System.Collections.Generic;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class TeamExternalInvitesResponse : IResponse, ICollection<ExternalInvite> {

        private ICollection<ExternalInvite> _collection = new List<ExternalInvite>();

        public int Count => _collection.Count;

        public bool IsReadOnly => _collection.IsReadOnly;

        public void Add(ExternalInvite item) => _collection.Add(item);

        public void Clear() => _collection.Clear();

        public bool Contains(ExternalInvite item) => _collection.Contains(item);

        public void CopyTo(ExternalInvite[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        public IEnumerator<ExternalInvite> GetEnumerator() => _collection.GetEnumerator();

        public bool Remove(ExternalInvite item) => _collection.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
