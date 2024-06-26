using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    Task<User> Register(RegisterDTO dto);
    Task<User> Login(LoginDTO dto);
}

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> Register(RegisterDTO registerDto)
    {
        // Check if the username already exists
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
        {
            throw new Exception("Username already exists");
        }

        // Create the user
        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = ComputeSha256Hash(registerDto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> Login(LoginDTO loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null || user.PasswordHash != ComputeSha256Hash(loginDto.Password))
        {
            return null; // Invalid username or password
        }

        return user;
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
