using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvTap.Application.Exceptions;
using EvTap.Contracts.Services;
using EvTap.Domain.Entities;
using EvTap.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace EvTap.Application.Services
{

    public class GenericService<TVM, TEntity> : IGenericService<TVM, TEntity> where TVM : class where TEntity : BaseEntity, new()
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly IMapper _mapper;
        private readonly IUnityOfWork _unityOfWork;
        private readonly ILogger<GenericService<TVM, TEntity>> _logger;

        public GenericService(IMapper mapper, IGenericRepository<TEntity> repository, IUnityOfWork unityOfWork, ILogger<GenericService<TVM, TEntity>> logger)
        {
            _mapper = mapper;
            _repository = repository;
            _unityOfWork = unityOfWork;
            _logger = logger;
        }




        public async Task<TVM> AddAsync(TVM entity)
        {
            if (entity == null)
            {
                throw new NotNullExceptions();
            }

            var entityToAdd = _mapper.Map<TEntity>(entity);
            var result = await _repository.AddAsync(entityToAdd);
            await _unityOfWork.SaveChangesAsync();
            _logger.LogInformation($"{typeof(TEntity).Name} entity added with ID: {result.Id}");
            return _mapper.Map<TVM>(result);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null)
            {
                throw new NotNullExceptions();
            }
            await _repository.DeleteAsync(id);
            await _unityOfWork.SaveChangesAsync();
            _logger.LogInformation($"{typeof(TEntity).Name} entity deleted with ID: {id}");
            return true;

        }

        public async Task<IEnumerable<TVM>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            if (data == null)
            {
                throw new NotNullExceptions();
            }
            var result = _mapper.Map<IEnumerable<TVM>>(data);
            _logger.LogInformation($"Retrieved all {typeof(TEntity).Name} entities.");
            return result;
        }

        public async Task<TVM> GetByIdAsync(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null)
            {
                throw new NotNullExceptions();
            }

            var result = _mapper.Map<TVM>(data);
            _logger.LogInformation($"Retrieved {typeof(TEntity).Name} entity with ID: {id}");
            return result;
        }

        public async Task<TVM> UpdateAsync(TVM entity)
        {
            var data = _mapper.Map<TEntity>(entity);
            if (data == null)
            {
                throw new NotNullExceptions();
            }
            await _repository.UpdateAsync(data);
            await _unityOfWork.SaveChangesAsync();
            _logger.LogInformation($"{typeof(TEntity).Name} entity updated with ID: {data.Id}");
            var result = _mapper.Map<TVM>(data);
            return result;
        }
    }
}

