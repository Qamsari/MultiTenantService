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