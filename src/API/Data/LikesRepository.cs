using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(AppDbContext context) : ILikesRepository
{
    public void Add(MemberLike like) => context.Likes.Add(like);

    public void Delete(MemberLike like) => context.Likes.Remove(like);

    public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
    {
        return await context.Likes
            .Where(l => l.SourceMemberId == memberId)
            .Select(l => l.TargetMemberId)
            .ToListAsync();
    }

    public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
    {
        return await context.Likes.FindAsync(sourceMemberId, targetMemberId);
    }

    public async Task<PaginationResult<Member>> GetMemberLikes(LikesRequest request)
    {
        var query = context.Likes.AsQueryable();
        IQueryable<Member> result;

        switch(request.Predicate.ToLower())
        {
            case "liked":
                result = query
                    .Where(q => q.SourceMemberId == request.MemberId)
                    .Select(q => q.TargetMember);
                break;
            case "likedby":
                result = query
                    .Where(q => q.TargetMemberId == request.MemberId)
                    .Select(q => q.SourceMember);
                break;
            default: // Mutual like
                var likeIds = await GetCurrentMemberLikeIds(request.MemberId);
                result = query
                    .Where(q => q.TargetMemberId == request.MemberId && likeIds.Contains(q.SourceMemberId))
                    .Select(q => q.SourceMember);
                break;
        }

        return await Pagination.CreateAsync(result, request.PageNumber, request.PageSize);
    }

    public async Task<bool> SaveAllChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}