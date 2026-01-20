using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkHub.DataAccess.Data;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;

namespace WorkHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.RecruitmentInfoRepo.GetAllAsync();
            var result = _mapper.Map<List<RecruitmentOverviewInfoDto>>(entities);
            return Ok(result);
        }
    }
}
