using LojaVirtual.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;

namespace WpfAppSistemaDeLoja;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // O projeto continua dependendo do SQL Server LocalDB instalado no Windows.
        try
        {
            using var db = new LojaContext();
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Nao foi possivel preparar o banco. Instale SQL Server LocalDB e confira a conexao.\n\n" + ex.Message,
                "Erro de inicializacao", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }
        base.OnStartup(e);
    }
}
