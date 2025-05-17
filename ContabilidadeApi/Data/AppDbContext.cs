using ContabilidadeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<ContaContabil> ContaContabil { get; set; }
        public DbSet<LancamentoContabil> LancamentoContabil { get; set; }
        public DbSet<LancamentoDebitoCredito> LancamentoDebitoCredito { get; set; }
        public DbSet<HistoricoContabil> HistoricoContabils { get; set; }
        public DbSet<RelatorioContas> RelatorioConta { get; set; }

       
            protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LancamentoDebitoCredito>()
                .HasOne(l => l.LancamentoContabil)
                .WithMany()
                .HasForeignKey(l => l.LancamentoContabilId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<LancamentoDebitoCredito>()
                .HasOne(l => l.ContaContabil)
                .WithMany()
                .HasForeignKey(l => l.LancamentoContabilId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RelatorioContas>()
                .HasOne(r => r.ContaContabil)
                .WithOne() 
                .HasForeignKey<RelatorioContas>(r => r.ContaContabilId);

            base.OnModelCreating(modelBuilder);
        }
    
    }
}
