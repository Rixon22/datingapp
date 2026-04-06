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

    public async Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId)
    {
        var query = context.Likes.AsQueryable();

        switch(predicate)
        {
            case "liked":
                return await query
                    .Where(q => q.SourceMemberId == memberId)
                    .Select(q => q.TargetMember)
                    .ToListAsync();
            case "likedBy":
                return await query
                    .Where(q => q.TargetMemberId == memberId)
                    .Select(q => q.SourceMember)
                    .ToListAsync();
            default: // Mutual like
                var likeIds = await GetCurrentMemberLikeIds(memberId);
                return await query
                    .Where(q => q.TargetMemberId == memberId && likeIds.Contains(q.SourceMemberId))
                    .Select(q => q.SourceMember)
                    .ToListAsync();
        }
    }

    public async Task<bool> SaveAllChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}