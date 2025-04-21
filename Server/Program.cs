using Microsoft.Extensions.DependencyInjection.Extensions;
using Server.Services.Certificate;
using Server.Services.DataService;
using Server.Services.TenantAccessor;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => {
    //    options.ListenAnyIP(1551);
    //    options.ListenAnyIP(1552);
    //    options.ListenAnyIP(1553, x =>
    //    {
    //        var certificatePath = "D:\\Programming\\Naqshava\\MultiTenantService\\Server\\certs\\cert.pem";
    //        var keyPath = "D:\\Programming\\Naqshava\\MultiTenantService\\Server\\certs\\key.pem";
    //        //var c = X509Certificate2.CreateFromPemFile(certificatePath, keyPath);

    //        //var c = CreateCertificateFromPem(certificatePath, keyPath);
    //        //////////////////////////////

    //        var certificate = new X509Certificate2(certificatePath);

    //        // Suggest create method of this code block
    //        var rsa = RSA.Create();
    //        var keyContent = File.ReadAllText(keyPath);

    //        rsa.ImportFromPem(keyContent);

    //        var cspParameters = new CspParameters()
    //        {
    //            KeyContainerName = "QAMSARI",
    //            Flags = CspProviderFlags.UseNonExportableKey,
    //        };
    //        var rsaPersistent = new RSACryptoServiceProvider(cspParameters);

    //        rsaPersistent.ImportParameters(rsa.ExportParameters(true));
    //        rsa.Dispose();
    //        rsa = rsaPersistent;
    //        // End of the code block

    //        var c = certificate.CopyWithPrivateKey(rsa);

    //        ////////////////////


    //        //var certificatePath = "D:\\Programming\\Naqshava\\MultiTenantService\\Server\\certs\\cert.pfx";
    //        //var c = new X509Certificate2(certificatePath, "123456");
    //        x.UseHttps(c);
    //});
    /////
    options.ConfigureHttpsDefaults(x =>
    {
        var cerManager = options.ApplicationServices.GetRequiredService<ICertificateManager>();
        var logger = options.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("Qam");
        x.ServerCertificateSelector = (_, hostName) =>
        {
            try
            {
                var cert = cerManager.GetHostRelatedCertificate(hostName);
                logger.LogInformation("{hostName}, cert {action}", hostName, cert != null ? "founded":"not founded");
                return cert;
            }catch(Exception ex)
            {
                logger.LogError(ex, "Error in get cert for {host}", hostName);
            }
            return null;
        };
    });
    ///
    //options.ListenAnyIP(80);
    ////options.ListenAnyIP(1552);

    //options.ListenAnyIP(443, listenOptions =>
    //{
    //    var cerManager = listenOptions.ApplicationServices.GetRequiredService<ICertificateManager>();
    //    var logger = listenOptions.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("Qam");
    //    listenOptions.UseHttps(httpsOptions =>
    //    {


    //        httpsOptions.ServerCertificateSelector = (connectionContext, hostName) =>
    //        {
    //                var cert = cerManager.GetHostRelatedCertificate(hostName);
    //                logger.LogInformation("{hostName}, cert is null == {cert}", hostName, cert is null);
    //                return cert;
    //        };
    //    });
    //});
});
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTenantAccessor();
builder.Services.AddDbService(builder.Configuration);
builder.Services.AddCertificateManager(builder.Configuration);
builder.Services.AddRazorPages();

var app = builder.Build();
app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ISeedData>().SeedData();
}
app.Run();
//X509Certificate2 CreateCertificateFromPem(string certificatePath, string privateKeyPath)
//{
//    string pass = Guid.NewGuid().ToString();
//    var certificate = X509Certificate2.CreateFromPemFile(certificatePath, privateKeyPath);
//    var collection = new X509Certificate2Collection(certificate);
//    var extention = Path.GetExtension(certificatePath);
//    var files = Directory.GetFiles(Path.GetDirectoryName(certificatePath), $"*{extention}")
//        .Where(x => x != certificatePath);
//    foreach (var file in files)
//    {
//        collection.Import(file);
//    }
//    return new X509Certificate2(collection.Export(X509ContentType.Pfx, pass), pass);
//}