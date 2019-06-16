using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions;
using GRpc.Messages;
using Microsoft.AspNetCore.Mvc;

namespace GRpc.Client.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        public readonly CandidateService.CandidateServiceClient candidateServiceClient;

        public CandidateController(CandidateService.CandidateServiceClient candidateServiceClient)
        {
            this.candidateServiceClient = candidateServiceClient;
        }

        [HttpGet]
        public async Task<ActionResult<bool>> TestCreate()
        {
            var result = false;

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
                }).Generate(5);


            try
            {
                var creator = candidateServiceClient.CreateCv();
                foreach (var createRequest in createRequests)
                {
                    await creator.RequestStream.WriteAsync(createRequest);
                }

                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        [HttpGet]
        public async Task<Candidate> GetCandidate()
        {
            var result= new Candidate();
            try
            {
                var client = candidateServiceClient.DownloadCv(new DownloadByName()
                {
                    Name = "test"
                });
                var iter = client.ResponseStream;
                while (await iter.MoveNext())
                {
                    result = iter.Current;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }
    }
}