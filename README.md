# azure-serverless-web-application

**Serverless web application**

![](https://docs.microsoft.com/azure/architecture/reference-architectures/serverless/_images/serverless-web-app.png)

https://github.com/mspnp/serverless-reference-implementation

# Command

```
# List commands
dotnet --list-sdks
dotnet --version
dotnet new globaljs
dotnet new globaljson --sdk-version 2.1.0
dotnet new classlib -f netcoreapp2.1
func init --docker
func new

# Run with Azure Functions Core Tools
cd Web && func host start

# Run with Docker
docker build -t <imageName> .
# Example: docker build -t sample .
docker run -p <port> <imageName>
# Example: docker run -p 8080:80 sample

```
