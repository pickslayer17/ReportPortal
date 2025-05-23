﻿using AutoMapper;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly ITestRepository _testRepository;
        private readonly ITestResultRepository _testResultRepository;
        private readonly IMapper _mapper;

        public TestResultService(ITestRepository testRepository, ITestResultRepository testResultRepository, IMapper mapper)
        {
            _testRepository = testRepository;
            _testResultRepository = testResultRepository;
            _mapper = mapper;
        }

        public async Task<int> AddTestResultToTestAsync(int testId, TestResultDto testDto, CancellationToken cancellationToken = default)
        {
            var test = await _testRepository.GetByAsync(t => t.Id == testId, cancellationToken);
            if (test == null)
            {
                throw new Exception();
            }
            else
            {
                var testResultToInsert = _mapper.Map<TestResult>(testDto);
                testResultToInsert.Test = test;

                return await _testResultRepository.InsertAsync(testResultToInsert);
            }
        }

        public async Task<IEnumerable<TestResultDto>> GetTestTestResultsAsync(int testId, CancellationToken cancellationToken = default)
        {
            var testResults = await _testResultRepository.GetAllByAsync(tr => tr.TestId == testId, cancellationToken);
            return testResults.Select(tr => _mapper.Map<TestResultDto>(tr));
        }

        public async Task<TestResultDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var testResult = await _testResultRepository.GetByAsync(tr => tr.Id == id, cancellationToken);
            return _mapper.Map<TestResultDto>(testResult);
        }

        public Task<TestResultDto> CreateAsync(TestResultDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestResultDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestResultDto>> GetAllByAsync(Expression<Func<TestResultDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestResultDto> GetByAsync(Expression<Func<TestResultDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
