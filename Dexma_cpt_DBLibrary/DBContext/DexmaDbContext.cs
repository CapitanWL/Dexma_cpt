using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dexma_cpt_DBLibrary
{
    public class DexmaDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath("C:/Users/capit/Desktop/CW/Dexma_cpt/Dexma_cpt_DBLibrary/DBContext/")
                    .AddJsonFile("appSettings.json")
                    .Build();

                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                var connectionString = config.GetConnectionString("DefaultConnection");

                optionsBuilder.UseNpgsql(connectionString).EnableSensitiveDataLogging();
            }
        }

        public DexmaDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            Database.Migrate();
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<UserRelation> UserRelations { get; set; } = null!;
        public DbSet<RelationType> RelationTypes { get; set; } = null!;
        public DbSet<UserKey> UsersKey { get; set; } = null!;
        public DbSet<InternalKey> InternalKeys { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User

            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .UseIdentityColumn();

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserKey)
                .WithOne(u => u.User)
                .HasForeignKey<UserKey>(fk => fk.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Relation

            modelBuilder.Entity<UserRelation>()
                .Property(r => r.UserRelationId)
                .UseIdentityColumn();

            modelBuilder.Entity<UserRelation>()
                .HasMany(r => r.Messages)
                .WithOne(r => r.UserRelation)
                .HasForeignKey(r => r.UserRelationId)
                .OnDelete(DeleteBehavior.Cascade);

            // RType

            modelBuilder.Entity<RelationType>()
                .Property(r => r.RelationTypeId)
                .UseIdentityColumn();

            modelBuilder.Entity<RelationType>()
                .HasMany(r => r.UserRelations)
                .WithOne(r => r.RelationType)
                .HasForeignKey(r => r.RelationTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Key

            modelBuilder.Entity<UserKey>()
                .Property(u => u.UserKeyId)
                .UseIdentityColumn();

            modelBuilder.Entity<UserKey>()
                .HasOne(u => u.InternalKey)
                .WithOne(u => u.UserKey)
                .HasForeignKey<UserKey>(fk => fk.UserKeyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Internal key

            modelBuilder.Entity<InternalKey>()
                .Property(u => u.InternalKeyId)
                .UseIdentityColumn();

            modelBuilder.Entity<InternalKey>()
                .HasMany(u => u.UserRelationsFrom)
                .WithOne(r => r.InternalFrom)
                .HasForeignKey(r => r.InternalFromId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InternalKey>()
                .HasMany(u => u.UserRelationsTo)
                .WithOne(r => r.InternalTo)
                .HasForeignKey(r => r.InternalToId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /*
        public async Task AddUser(string username, string phone, string nickname, bool status)
        {
            User newUser = new User()
            {
                Nickname = nickname,
                UserName = username,
                Phone = phone,
            };

            Users.Add(newUser);
            await SaveChangesAsync();
        }

        public async Task AddUserKey(int userId, string password)
        {
            byte[] saltBytes = PBKDF.GenerateSalt();

            UserKey newUserKey = new UserKey()
            {
                Password = PBKDF.Pbkdf2(password, saltBytes, 10000, HashAlgorithmName.SHA512, 64),
                PasswordSalt = saltBytes,
                UserId = userId
            };

            UsersKey.Add(newUserKey);
            await SaveChangesAsync();
        }

        public async Task AddRelationType(string name)
        {
            RelationType newRelationType = new RelationType()
            {
                RelationName = name
            };

            RelationTypes.Add(newRelationType);
            await SaveChangesAsync();
        }

        public async Task AddRelation(int reltypeid, int userfrom, int userto)
        {
            Relation newRelation = new Relation()
            {
                RelationTypeId = reltypeid,
                UserFromId = userfrom,
                UserToId = userto
            };

            Relations.Add(newRelation);
            await SaveChangesAsync();
        }

        public async Task AddMessage(int relation, string data, bool status, DateTime messageDateTime)
        {
            var currentrelation = Relations.FirstOrDefault(r => r.RelationId == relation);

            var currentUser = Users.FirstOrDefault(u => u.UserId == currentrelation.UserFromId);

            var currentKey = UsersKey.FirstOrDefault(uk => uk.UserId == currentUser.UserId);

            Message newMessage = new Message()
            {
                RelationId = relation,
                MessageData = Encoding.UTF8.GetBytes(data),
                MessageDT = messageDateTime,
            };

            Messages.Add(newMessage);
            await SaveChangesAsync();
        }

        */
    }
}
