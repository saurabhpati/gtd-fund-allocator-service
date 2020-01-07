using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using GTDFundAllocatorService.Domain.Shared;
using GTDFundAllocatorService.Domain.Shared.Contracts;
using GTDFundAllocatorService.Foundation.Common;
using GTDFundAllocatorService.Repository.Shared;
using GTDFundAllocatorService.Repository.Shared.Contracts;

namespace GTDFundAllocatorService.Domain.Implementation.Concretes
{
    public class FundManager : IFundManager
    {
        private readonly IGeneralRepository<Fund> _fundRepository;
        private readonly IMapper _mapper;

        public FundManager(IGeneralRepository<Fund> fundRepository, IMapper mapper)
        {
            _fundRepository = fundRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FundModel>> GetFund(int? offset, int? limit, bool? disabled)
        {
            Expression<Func<Fund, bool>> condition = fund => fund.StatusId != (int)FundStatus.Denied;

            if (!disabled.HasValue || !disabled.Value)
            {
                condition = condition.And(fund => !fund.Disabled);
            }

            Task<List<Fund>> fundTask = null;

            if (offset.HasValue && limit.HasValue)
            {
                fundTask = _fundRepository
                    .List(condition)
                    .Skip(offset.Value)
                    .Take(limit.Value)
                    .Result();
            }
            else
            {
                fundTask = _fundRepository.List(condition).Result();
            }

            var funds = await fundTask;

            return _mapper.Map<IEnumerable<FundModel>>(funds);
        }

        public async Task AddFund(FundModel fund)
        {
            await _fundRepository.AddAsync(_mapper.Map<Fund>(fund));
        }

        public async Task UpdateFund(FundModel fund)
        {
            var entity = await _fundRepository.GetById(fund.Id).Result();
            entity.Amount = fund.Amount;
            entity.UpdatedBy = fund.UpdatedBy;
            entity.UpdatedOn = DateTime.UtcNow;
            entity.ApprovedBy = fund.ApprovedBy;
            entity.ApprovedOn = entity.ApprovedBy.HasValue ? (DateTime?)DateTime.UtcNow : null;
            entity.StatusId = fund.StatusId;
            await _fundRepository.EditAsync(entity);
        }

        public async Task DeleteFund(int fundId)
        {
            var fund = await _fundRepository.GetById(fundId).Result();
            fund.Disabled = true;
            await _fundRepository.EditAsync(fund);
        }
    }
}
