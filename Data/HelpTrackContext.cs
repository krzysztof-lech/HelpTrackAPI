using HelpTrackAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpTrackAPI.Data
{
    public class HelpTrackContext : DbContext
    {
        public HelpTrackContext(DbContextOptions<HelpTrackContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<TicketMessage> TicketMessages { get; set; }


    }
}
