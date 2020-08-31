using PeakMVP.Models.Rests.Requests.Contracts;
using PeakMVP.Models.Rests.Responses.Contracts;
using System.Threading.Tasks;

namespace PeakMVP.Services.RequestProvider {
    public interface IRequestProvider {
        Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest
            where TResponse : class, IResponse, new();

        Task<TResponse> GetAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest
            where TResponse : class, IResponse;

        Task<TResponse> PutAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest
            where TResponse : class, IResponse, new();

        Task<TResponse> DeleteAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest
            where TResponse : class, IResponse, new();

        Task<TResponse> PatchAsync<TRequest, TResponse>(TRequest request)
                where TRequest : class, IRequest
                where TResponse : class, IResponse, new();

        void ClientTokenReset();
    }
}
