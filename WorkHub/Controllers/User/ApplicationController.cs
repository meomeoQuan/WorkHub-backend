using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.ApplicationDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApplicationController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications([FromQuery] ApplicationFilterDTO filter)
        {
            try
            {
                Expression<Func<Application, bool>> filterExpression = a => true;

                // 1. Filter by Status
                if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "All Status")
                {
                    filterExpression = filterExpression.And(a => a.Status == filter.Status);
                }

                // 2. Filter by Job (RecruitmentId)
                if (filter.JobId.HasValue && filter.JobId.Value != 0)
                {
                    filterExpression = filterExpression.And(a => a.RecruitmentId == filter.JobId.Value);
                }

                // 3. Search by Name or Email
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var term = filter.SearchTerm.ToLower();
                    filterExpression = filterExpression.And(a => a.User.FullName.Contains(term) || a.User.Email.Contains(term));
                }

                // Get All with Includes
                // Explicitly include User, UserDetail (for Avatar), and Recruitment (for Job Title)
                var applications = await _unitOfWork.ApplicationRepository.GetAllAsync(
                    filter: filterExpression,
                    includeProperties: "User,User.UserDetail,Recruitment"
                );

                var applicationDTOs = _mapper.Map<IEnumerable<ApplicationDTO>>(applications);

                return Ok(ApiResponse<IEnumerable<ApplicationDTO>>.Ok(applicationDTOs, "Applications retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetApplicationSummary()
        {
            try
            {
                var summary = new ApplicationSummaryDTO
                {
                    TotalApplications = await _unitOfWork.ApplicationRepository.CountAsync(),
                    New = await _unitOfWork.ApplicationRepository.CountAsync(a => a.Status == ApplicationStatus.New),
                    Reviewing = await _unitOfWork.ApplicationRepository.CountAsync(a => a.Status == ApplicationStatus.Reviewing),
                    Shortlisted = await _unitOfWork.ApplicationRepository.CountAsync(a => a.Status == ApplicationStatus.Shortlisted),
                    Interviewed = await _unitOfWork.ApplicationRepository.CountAsync(a => a.Status == ApplicationStatus.Interviewed)
                };

                return Ok(ApiResponse<ApplicationSummaryDTO>.Ok(summary, "Application summary retrieved successfully"));
            }
            catch (Exception ex)
            {
                 return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }
    }

    // Helper extension methods for Expression combining if not already present in Utility
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
