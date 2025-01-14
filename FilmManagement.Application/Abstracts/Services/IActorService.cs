﻿using FilmManagement.Application.Common.Responses;
using FilmManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace FilmManagement.Application.Abstracts.Services
{
    public interface IActorService
    {
        Task<ApiResponse<Actor?>> GetAsync(
        Expression<Func<Actor, bool>> predicate,
        Func<IQueryable<Actor>, IIncludableQueryable<Actor, object>>? include = null,
        bool enableTracking = true,
        bool withDeleted = false
        );

        Task<ApiListResponse<Actor>> GetListAsync(
        Expression<Func<Actor, bool>>? predicate = null,
        Func<IQueryable<Actor>, IIncludableQueryable<Actor, object>>? include = null,
        bool enableTracking = true,
        bool withDeleted = false
        );

        Task<bool> AnyAsync(
        Expression<Func<Actor, bool>>? predicate = null,
        bool enableTracking = true,
        bool withDeleted = false
        );

        Task<ApiResponse<Actor>> AddAsync(Actor actor);

        Task<ApiResponse<Actor>> UpdateAsync(Actor actor);

        Task<ApiResponse<Actor>> DeleteAsync(Actor actor);

        Task<ApiListResponse<Actor>> AddRangeAsync(IList<Actor> actors);
        Task<ApiListResponse<Actor>> UpdateRangeAsync(IList<Actor> actors);
        Task<ApiListResponse<Actor>> DeleteRangeAsync(IList<Actor> actors);
    }
}
