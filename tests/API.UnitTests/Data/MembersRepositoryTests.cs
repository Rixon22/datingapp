using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.UnitTests.Data;

public class MembersRepositoryTests
{
    private AppDbContext _context;
    private IMembersRepository _membersRepository;
    
    [SetUp]
    public void Setup()
    {
        _context = GlobalTestSetup.AppDbContext;
        _membersRepository = new MembersRepository(_context);
    }

    [Test]
    public async Task GetMembersAsync_Valid_ShouldReturnEntities()
    {
        // Arrange & Act
        var memberRequest = new MemberRequest {};
        var members = await _membersRepository.GetMembersAsync(memberRequest);

        // Assert
        Assert.That(members, Is.Not.Null);
        Assert.That(members.Items, Has.Count.EqualTo(10));    }
}
