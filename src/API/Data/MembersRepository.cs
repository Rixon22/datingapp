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

    public async Task<PaginationResult<Member>> GetMembersAsync(PaginationRequest paginationRequest)
    {
        var query = context.Members;

        return await Pagination.CreateAsync(query, paginationRequest.PageNumber, paginationRequest.PageSize);    
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
