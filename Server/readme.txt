dotnet ef migrations add create_db --project server --context AdminAppDbContext
dotnet ef database update --project server --context AdminAppDbContext



openssl req -x509 -newkey rsa:4096 -keyout codementors.ir.key -out codementors.ir.pem -sha256 -days 3650 -nodes -subj "/CN=codementors.ir"

openssl req -x509 -newkey rsa:4096 -sha256 -days 3650 -nodes -keyout localhost.key -out localhost.crt -subj "/CN=localhost"  -addext "subjectAltName=DNS:example.com,DNS:*.example.com,IP:10.0.0.1"

openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -sha256 -days 3650 -nodes -subj "/CN=localhost"

openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -sha256 -days 3650 -nodes -subj "/C=XX/ST=StateName/L=CityName/O=CompanyName/OU=CompanySectionName/CN=CommonNameOrHostname"