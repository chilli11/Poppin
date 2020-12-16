using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Poppin.Models.Identity;

namespace Poppin.Data
{
	public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
	}
	public class TokenDbContext : DbContext
	{
		public TokenDbContext(DbContextOptions<TokenDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}

		public DbSet<RefreshToken> RefreshTokens { get; set; }
	}
}
