using Maxi.Services.SouthSide.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.Services.SouthSide.Data
{
    public class ApplicationDbContext : DbContext
    {
        private DbSet<ServiceConfiguration> _servicesConfiguration;
        private DbSet<ServiceAttribute> _attributes;
        private DbSet<GlobalAttributes> _globalAttributes;

        public DbSet<ServiceConfiguration> ServicesConfiguration
        {
            get
            {
                if (_servicesConfiguration == null)
                    _servicesConfiguration = Set<ServiceConfiguration>();
                return _servicesConfiguration;
            }
        }

        public DbSet<ServiceAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                    _attributes = Set<ServiceAttribute>();
                return _attributes;
            }

        }

        public DbSet<GlobalAttributes> GlobalAttributes
        {
            get
            {
                if (_globalAttributes == null)
                    _globalAttributes = Set<GlobalAttributes>();
                return _globalAttributes;
            }
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder != null)
            {
                // base.OnModelCreating(modelBuilder);
                //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                

                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    // equivalent of modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                    entityType.GetForeignKeys()
                        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                        .ToList()
                        .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
                }

                // TODO
                //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();


                //modelBuilder.Configurations.Add(new ServiceConfiguationScheduleTypeConfiguration());
                //modelBuilder.Configurations.Add(new ServiceConfigurationTickTypeConfiguration());
                //modelBuilder.Configurations.Add(new ServiceConfigurationTypeConfiguration());
                //modelBuilder.Configurations.Add(new ServiceAttributeTypeConfiguration());
                //modelBuilder.Configurations.Add(new GlobalAttributesTypeConfiguration());
                //modelBuilder.Configurations.Add(new ServiceScheduleTypeConfiguration());

                modelBuilder.Entity<ServiceConfiguration>().ToTable("ServiceConfiguration", "Services");
                modelBuilder.Entity<ServiceConfiguration>().HasKey(c => c.Code);
                modelBuilder.Entity<ServiceAttribute>().ToTable("ServiceAttributes", "Services");
                modelBuilder.Entity<ServiceAttribute>().HasKey(c => new { c.Code, c.Key});
                modelBuilder.Entity<GlobalAttributes>().ToTable("GlobalAttributes");
                modelBuilder.Entity<GlobalAttributes>().HasKey(c => c.Name);

                
            }
        }
    }
}
