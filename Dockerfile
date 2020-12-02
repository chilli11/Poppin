FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /poppin-app
COPY dockerpub . 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "Poppin.dll"]
