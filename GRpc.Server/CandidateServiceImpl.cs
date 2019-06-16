using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GRpc.Messages;

// ReSharper disable once IdentifierTypo
namespace GRpc.Server
{
    public class CandidateServiceImpl:CandidateService.CandidateServiceBase
    {
        public override async Task<CreateCvResponse> CreateCv(IAsyncStreamReader<Candidate> requestStream, ServerCallContext context)
        {
            var result = new CreateCvResponse
            {
                IsSuccess = false
            };
            
            while (await requestStream.MoveNext())
            {
                var candidate = requestStream.Current;
                //save Cvs
                Console.WriteLine(candidate.Name);
            }
            
            return result;
        }
    }
}