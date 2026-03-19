using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MembersRepository(AppDbContext context) : IMembersRepository
{
    public async Task<Member?> GetMemberAsync(string id)
    {
        return await context.Members.FindAsync(id);
    }

        public async Task<Member?> GetMemberForUpdateAsync(string id)
    {
        return await context.Members
                            .Include(m => m.User)
                            .Include(m => m.Photos)
                            .SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<PaginationResult<Member>> GetMembersAsync(MemberRequest request)
    {
        var query = context.Members.AsQueryable();

        query = query.Where(x => x.Id != request.CurrentMemberId);

        if (!string.IsNullOrEmpty(request.Gender))
        {
            query = query.Where(x => x.Gender == request.Gender);
        }

        var minAgeDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-request.MaxAge - 1));
        var maxAgeDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-request.MinAge));
        query = query.Where(x => x.BirthDay >= minAgeDate && x.BirthDay <= maxAgeDate);

        return await Pagination.CreateAsync(query, request.PageNumber, request.PageSize);
    }
    
    public async Task<IReadOnlyList<Photo>> GetPhotosAsync(string memberId)
    {
        return await context.Members
            .Where(m => m.Id == memberId)
            .SelectMany(m => m.Photos)
            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        context.Entry(member).State = EntityState.Modified;
    }
}
