using Dexma_cpt_EncryptLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

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

        public DexmaDbContext(DbContextOptions<DexmaDbContext> options)
        : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
            //Database.Migrate();
        }

        //public DexmaDbContext()
        //{
       // }

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
       .UseIdentityAlwaysColumn();

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserKey)
                .WithOne(u => u.User)
                .HasForeignKey<UserKey>(fk => fk.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Relation
            modelBuilder.Entity<UserRelation>()
            .Property(u => u.UserRelationId)
            .UseIdentityAlwaysColumn();

            modelBuilder.Entity<UserRelation>()
                .HasMany(r => r.Messages)
                .WithOne(r => r.UserRelation)
                .HasForeignKey(r => r.UserRelationId)
                .OnDelete(DeleteBehavior.Cascade);

            // RType

            modelBuilder.Entity<RelationType>()
            .Property(u => u.RelationTypeId)
            .UseIdentityAlwaysColumn()
       ;

            modelBuilder.Entity<RelationType>()
                .HasMany(r => r.UserRelations)
                .WithOne(r => r.RelationType)
                .HasForeignKey(r => r.RelationTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Key

            modelBuilder.Entity<UserKey>()
                 .Property(u => u.UserKeyId)
                 .UseIdentityAlwaysColumn();

            modelBuilder.Entity<UserKey>()
                .HasOne(u => u.InternalKey)
                .WithOne(u => u.UserKey)
                .HasForeignKey<InternalKey>(fk => fk.UserKeyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Internal key

            modelBuilder.Entity<InternalKey>()
    .Property(u => u.InternalKeyId)
    .UseIdentityAlwaysColumn();

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

        #region help for me :))

        public async Task AddUser(string username, string phone, string nickname, bool status, string password)
        {

            User newUser = new()
            {
                Username = username,
                Nickname = nickname,
                Phone = phone,
                AccountStatus = status
            };

            await Users.AddAsync(newUser);
            await SaveChangesAsync();

            var searchUser = await Users.FirstOrDefaultAsync(u => u.Username == username);

            byte[] saltBytes = BaseGenerator.SaltGenerator();

            UserKey newUserKey = new (Pbkdf.PbkdfCreate(password, saltBytes),
                saltBytes, searchUser.UserId);

            await UsersKey.AddAsync(newUserKey);
            await SaveChangesAsync();

            var searchKey = await UsersKey.FirstOrDefaultAsync(uk => uk.UserId == searchUser.UserId);

            InternalKey newInternalKey = new()
            {
                InternalKeyData =
                Pbkdf.PbkdfCreate(BaseGenerator.GenerateRandomString(),
                BaseGenerator.SaltGenerator()),
                UserKeyId = searchKey.UserKeyId
            };

            await InternalKeys.AddAsync(newInternalKey);
            await SaveChangesAsync();
        }

        public async Task AddRelationType(string name)
        {
            RelationType newRelationType = new()
            {
                RelationName = name
            };

            RelationTypes.Add(newRelationType);
            await SaveChangesAsync();
        }

        public async Task AddRelation(int reltypeid, int internalfrom, int internalto)
        {
            UserRelation newRelation = new()
            {
                RelationTypeId = reltypeid,
                InternalFromId = internalfrom,
                InternalToId = internalto
            };

            await UserRelations.AddAsync(newRelation);
            await SaveChangesAsync();
        }

        /*public async Task AddMessage(int userrelation, string data, DateTime messageDateTime)
        {
            Message newMessage = new()
            {
                UserRelationId = userrelation,
                MessageData = Encoding.UTF8.GetBytes(data),
                SendingDateTime = messageDateTime,
                IsDeleted = false,
            };

            Messages.Add(newMessage);
            await SaveChangesAsync();
        }*/

        #endregion
    }
}
