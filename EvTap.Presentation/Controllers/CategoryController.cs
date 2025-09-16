﻿using EvTap.Contracts.DTOs;
using EvTap.Contracts.Services;
using EvTap.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvTap.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGenericService<CategoryDTO,Category> _genericService;

        public CategoryController(IGenericService<CategoryDTO, Category> genericService)
        {
            _genericService = genericService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            var categories = await _genericService.GetAllAsync();
            return Ok(categories);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var category = await _genericService.GetByIdAsync(id);
            return Ok(category);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> AddAsync(CategoryDTO categoryDTO)
        {
            var createdCategory = await _genericService.AddAsync(categoryDTO);
            return Ok(createdCategory);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(CategoryDTO categoryDTO)
        {
            var updatedCategory = await _genericService.UpdateAsync(categoryDTO);
            return Ok(updatedCategory);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _genericService.DeleteAsync(id);
            return Ok();
        }
        

    }
}
