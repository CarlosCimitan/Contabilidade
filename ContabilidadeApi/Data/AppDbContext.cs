using ContabilidadeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ContaContabil> ContasContabeis { get; set; }
        public DbSet<HistoricoContabil> HistoricosContabeis { get; set; }
        public DbSet<LancamentoContabil> LancamentosContabeis { get; set; }
        public DbSet<LancamentoDebitoCredito> DebitosCreditos { get; set; }
        public DbSet<RelatorioContas> RelatoriosContas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RelatorioContas>()
                .HasOne(rc => rc.ContaContabil)
                .WithMany(c => c.Relatorios)
                .HasForeignKey(rc => rc.ContaContabilId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Cargo)
                .HasConversion<string>();

            modelBuilder.Entity<ContaContabil>()
                .Property(c => c.TipoConta)
                .HasConversion<string>();

            modelBuilder.Entity<ContaContabil>()
                .Property(c => c.Natureza)
                .HasConversion<string>();

            modelBuilder.Entity<LancamentoDebitoCredito>()
                .Property(l => l.TipoAcao)
                .HasConversion<string>();

            modelBuilder.Entity<RelatorioContas>()
                .Property(r => r.Relatorio)
                .HasConversion<string>();
            modelBuilder.Entity<LancamentoDebitoCredito>()
                .HasOne(dc => dc.LancamentoContabil)
                .WithMany(lc => lc.DebitosCreditos)
                .HasForeignKey(dc => dc.LancamentoContabilId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LancamentoDebitoCredito>()
                .HasOne(dc => dc.ContaContabil)
                .WithMany(c => c.DebitosCreditos)
                .HasForeignKey(dc => dc.ContaContabilId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
