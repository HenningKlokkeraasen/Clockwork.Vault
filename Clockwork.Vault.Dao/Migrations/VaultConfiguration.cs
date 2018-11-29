using System.Data.Entity.Migrations;

namespace Clockwork.Vault.Dao.Migrations
{

    internal sealed class VaultConfiguration : DbMigrationsConfiguration<VaultContext>
    {
        public VaultConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true; // TODO post-dev-phase
        }

        /// <summary>
        /// Will be called after migrating to the latest version.
        /// Use the DbSet<T>.AddOrUpdate() helper extension method 
        /// to avoid creating duplicate seed data.
        /// </summary>
        protected override void Seed(VaultContext context)
        {
        }
    }
}
