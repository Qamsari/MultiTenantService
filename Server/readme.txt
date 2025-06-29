dotnet ef migrations add create_db --project server --context AdminAppDbContext
dotnet ef database update --project server --context AdminAppDbContext



openssl req -x509 -newkey rsa:4096 -keyout codementors.ir.key -out codementors.ir.pem -sha256 -days 3650 -nodes -subj "/CN=codementors.ir"

openssl req -x509 -newkey rsa:4096 -sha256 -days 3650 -nodes -keyout localhost.key -out localhost.crt -subj "/CN=localhost"  -addext "subjectAltName=DNS:example.com,DNS:*.example.com,IP:10.0.0.1"

openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -sha256 -days 3650 -nodes -subj "/CN=localhost"

openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -sha256 -days 3650 -nodes -subj "/C=XX/ST=StateName/L=CityName/O=CompanyName/OU=CompanySectionName/CN=CommonNameOrHostname"


openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.pem
openssl pkcs12 -export -out codementors.ir.pfx -inkey codementors.ir.key -in codementors.ir.pem
openssl pkcs12 -export -out bitbug.ir.pfx -inkey bitbug.ir.key -in bitbug.ir.pem

openssl pkcs12 -in chat.basispanel.ai.pfx -nocerts -out chat.basispanel.ai.key -nodes -passin pass:chat.basispanel.ai
openssl pkcs12 -in chat.basispanel.ai.pfx -clcerts -nokeys -out chat.basispanel.ai.pem -passin pass:chat.basispanel.ai

==================================
To configure Nginx on Windows to use multiple domain names instead of just localhost, you need to create separate server blocks in your Nginx configuration file for each domain, and then configure your system's hosts file to map those domain names to your local machine's IP address (usually 127.0.0.1). 
Here's a step-by-step guide:
1. Configure the hosts file:

    Open the hosts file (located at C:\Windows\System32\drivers\etc\hosts) with administrator privileges (e.g., using Notepad with "Run as administrator").
    Add lines for each domain, mapping them to 127.0.0.1. For example: 



    127.0.0.1       example.com
    127.0.0.1       www.example.com
    127.0.0.1       sub.example.com

This tells your computer to direct traffic for these domains to your local machine. 
2. Create or modify the Nginx configuration file:

    Nginx configurations are typically found in the conf directory within your Nginx installation.
    You might have a nginx.conf file or a sites-available directory with separate configuration files for each site.
    Create a separate server block for each domain, specifying the server_name and the root directory for that domain's files. For example: 



    server {
        listen 80;
        server_name example.com www.example.com;
        root C:/path/to/example/website;
        index index.html;
    }

    server {
        listen 80;
        server_name sub.example.com;
        root C:/path/to/sub/website;
        index index.html;
    }

Replace C:/path/to/example/website and C:/path/to/sub/website with the actual paths to your website files.
3. Enable the configuration:

    If you're using a sites-available directory, you'll need to create symbolic links to the sites-enabled directory.
    For example, if you have a file named example.com in sites-available, you'd create a symbolic link in sites-enabled: mklink example.com example.com 

4. Test and restart Nginx: 

    Use the command nginx -t to test the configuration for syntax errors.
    If the test is successful, restart Nginx using nginx -s reload or by restarting the Nginx service. 

5. Test in your browser:

    Open your browser and navigate to the domain names you configured (e.g., http://example.com, http://www.example.com, http://sub.example.com).
    Nginx should now serve the content from the corresponding root directories based on the domain name. 

Key points:

    server_name:
    This directive tells Nginx which domain names this server block should handle. You can use wildcards (e.g., *.example.com) to match multiple subdomains.
    root:
    This directive specifies the root directory for the website's files.
    index:
    This directive specifies the default file to serve when a directory is requested.
    hosts file:
    This file is crucial for mapping domain names to your local machine's IP address for testing purposes.
    Restart/Reload:
    Always restart or reload Nginx after making changes to the configuration file. 

By following these steps, you can configure Nginx on your Windows machine to serve multiple domains locally, making it easier to develop and test websites with different domain names.
=========================================


To host an ASP.NET Core application behind Nginx on Windows, you'll need to configure Nginx as a reverse proxy to forward requests to your .NET application, which will be running using the Kestrel web server.
Here's a step-by-step guide:
1. Install and Configure Nginx:

    Download the latest stable version of Nginx for Windows from the official Nginx website.
    Extract the downloaded archive to a directory of your choice (e.g., C:\nginx).
    Navigate to the Nginx directory in your command prompt and start the server using nginx.exe.
    Verify the installation by accessing http://localhost in your browser. You should see the default Nginx welcome page. 

2. Configure Nginx as a Reverse Proxy:

    Open the nginx.conf file located in the conf directory of your Nginx installation (e.g., C:\nginx\conf\nginx.conf). 

Modify the http block to include a server block for your ASP.NET Core application.
Within the server block, define a location block to handle incoming requests.
Use proxy_pass to forward requests to your Kestrel server, which typically listens on http://localhost:5000 (or another port you've configured). 
Consider adding configurations for proxy_set_header to forward headers like X-Forwarded-For and X-Forwarded-Proto for proper request handling in your .NET application. 



http {
    server {
        listen 80;
        server_name your_domain.com; # Replace with your domain

        location / {
            proxy_pass http://localhost:5000;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }
}

    Save the nginx.conf file and reload Nginx for the changes to take effect. 

3. Configure Your ASP.NET Core Application (Kestrel):

    Your ASP.NET Core application should be configured to listen on the specified port (e.g., 5000). 

You may need to configure the application to handle forwarded headers correctly using the Forwarded Headers Middleware. 
Consider using a launchSettings.json file to manage environment variables and application settings, especially for development and testing. 

4. Start and Manage Your Application:

    Start your ASP.NET Core application. It will be listening on the configured port. 

Nginx will act as a reverse proxy, forwarding requests to your application. 
You can configure your application to start automatically on system startup using various methods (e.g., Windows services, task scheduler). 

5. Testing and Troubleshooting:

    Access your application through http://your_domain.com (or http://localhost if you're testing locally). 

Check Nginx logs (usually located in logs/error.log and logs/access.log within the Nginx directory) for any errors. 
Inspect your .NET application's logs for any issues related to request handling or processing. 
If you encounter issues, ensure that the application is running, the port is correct, and Nginx is configured to forward requests properly. 

By following these steps, you can successfully host your ASP.NET Core application behind Nginx on a Windows server, taking advantage of Nginx's reverse proxy capabilities for improved performance and security.

=============================
To set up a simple HTTPS site with Nginx on Windows, you'll need to download Nginx, obtain or generate SSL certificates, configure Nginx to use the certificates, and then test the configuration. 
Here's a step-by-step guide:
1. Download and Install Nginx:

    Download the latest mainline version of Nginx for Windows from the official website.
    Extract the downloaded archive to a directory of your choice (e.g., C:\nginx).
    Open a command prompt, navigate to the Nginx directory, and start the server by running nginx.exe.
    Verify the installation by opening http://localhost in your browser. You should see the Nginx welcome page. 

2. Obtain or Generate SSL Certificates:

    Option 1: Generate a self-signed certificate (for testing purposes):
        Download and install OpenSSL for Windows from the official website.
        Use OpenSSL to generate a private key and a certificate signing request (CSR): 



        openssl req -new -newkey rsa:2048 -nodes -keyout localhost.key -out localhost.csr

    Use the CSR to generate a self-signed certificate: 



        openssl x509 -req -in localhost.csr -signkey localhost.key -out localhost.crt -days 365

    Move the generated localhost.key and localhost.crt files to a safe location (e.g., C:\nginx\conf\ssl).
    Option 2: Obtain a certificate from a Certificate Authority (for production):
    Follow the instructions provided by your chosen CA (e.g., Let's Encrypt through Certbot) to generate a CSR and obtain the certificate files (certificate and optionally intermediate certificates/CA bundle). 

Place the certificate files in a safe location (e.g., C:\nginx\conf\ssl). 

3. Configure Nginx for HTTPS:

    Open the nginx.conf file (usually located in C:\nginx\conf) in a text editor. 

Locate the http block and add a server block for HTTPS (port 443) within it. 
Configure the server block with the following:

    listen 443 ssl; - Listen for HTTPS connections on port 443. 

server_name your_domain.com; - Replace your_domain.com with your domain name or localhost. 
ssl_certificate path/to/your_certificate.crt; - Specify the path to your SSL certificate file. 
ssl_certificate_key path/to/your_private.key; - Specify the path to your private key file. 
Optionally, include ssl_protocols and ssl_ciphers directives to configure supported TLS versions and ciphers for stronger security. 
Optionally, include ssl_prefer_server_ciphers on; to prefer server's cipher preferences. 
For a more secure configuration, consider adding secure headers as described in. 

Save the nginx.conf file. 

4. Test and Start Nginx:

    Open a command prompt, navigate to the Nginx directory, and run nginx.exe -s reload to reload the configuration. 

Open your browser and go to https://your_domain.com (or https://localhost if using a self-signed certificate). 
If you see a security warning when using a self-signed certificate, you can proceed by adding an exception in your browser. This is expected for self-signed certificates. 
===========================
TASKKILL /F /IM nginx.exe
tasklist /fi "imagename eq nginx.exe"

py -m pip install certbot
py -m pip install certbot-dns-desec

nginx -s quit
curl -v https://acme-v02.api.letsencrypt.org/
./nginx -s reload
start nginx
tasklist.exe /FI "IMAGENAME EQ nginx.exe"

certbot certonly --webroot -w C:\websites\qaz.ir -d qazanfari.ir --non-interactive --agree-tos --email c3d_lover@yahoo.com --logs-dir C:\websites\qaz.ir\certbot\log  --debug
certbot certonly --webroot -w C:\nginx-1.27.5\www\certbot -d crm.devolutions.net --non-interactive --agree-tos --email c3d_lover@yahoo.com --logs-dir C:\nginx-1.27.5\www\certbot\log  --debug