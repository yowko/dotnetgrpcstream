using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions;
using Google.Protobuf.Collections;
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
                using (var creator = candidateServiceClient.CreateCv())
                {
                    foreach (var createRequest in createRequests)
                    {
                        await creator.RequestStream.WriteAsync(createRequest);
                    }

                    await creator.RequestStream.CompleteAsync();

                    var summary = await creator.ResponseAsync;
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
            var result = new Candidate();
            try
            {
                using (var client = candidateServiceClient.DownloadCv(new DownloadByName()
                {
                    Name = "test"
                }))
                {
                    while (await client.ResponseStream.MoveNext())
                    {
                        result = client.ResponseStream.Current;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        [HttpGet]
        public async Task<RepeatedField<Candidate>> CreateAndDownload()
        {
            var result = new RepeatedField<Candidate>();

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
                using (var call = candidateServiceClient.CreateDownloadCv())
                {
                    var responseReaderTask = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            var candidate = call.ResponseStream.Current;
                            result.AddRange(candidate.Candidates_);
                        }
                    });

                    foreach (var request in createRequests)
                    {
                        await call.RequestStream.WriteAsync(request);
                    }

                    await call.RequestStream.CompleteAsync();
                    await responseReaderTask;
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