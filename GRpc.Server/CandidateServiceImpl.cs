using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions;
using Grpc.Core;
using GRpc.Messages;

// ReSharper disable once IdentifierTypo
namespace GRpc.Server
{
    public class CandidateServiceImpl : CandidateService.CandidateServiceBase
    {
        public override async Task<CreateCvResponse> CreateCv(IAsyncStreamReader<Candidate> requestStream,
            ServerCallContext context)
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

        public override async Task DownloadCv(DownloadByName request, IServerStreamWriter<Candidate> responseStream,
            ServerCallContext context)
        {
            var fakeJobs = new Faker<Job>()
                .RuleFor(a => a.Title, (f, u) => f.Company.Bs())
                .RuleFor(a => a.Salary, (f, u) => f.Commerce.Random.Int(1000, 2000))
                .RuleFor(a => a.JobDescription, (f, u) => f.Lorem.Text());

            var createRequests = new Faker<Candidate>()
                .RuleFor(a => a.Name, (f, u) => f.Name.FullName())
                .RuleFor(a => a.Jobs, (f, u) =>
                {
                    u.Jobs.AddRange(fakeJobs.GenerateBetween(3, 5));

                    return u.Jobs;
                }).Generate();

           await  responseStream.WriteAsync(createRequests);
            
        }
    }
}